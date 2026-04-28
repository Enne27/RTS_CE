using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    #region Variables
    private InputSystem_Actions cameraActions;
    private InputAction movement;
    private Transform cameraTransform;

    [SerializeField]
    private float maxSpeed = 5f;
    private float speed;
    [SerializeField]
    private float acceleration = 10f;
    [SerializeField]
    private float damping = 15f;
    [SerializeField]
    private float stepSize = 6f;
    [SerializeField]
    private float zoomDampening = 7.5f;
    [SerializeField]
    private float minHeight = 3f;
    [SerializeField]
    private float maxHeight = 30f;

    [SerializeField]
    private float maxRotationSpeed = 1f;

    [SerializeField]
    [Range(0f, 0.1f)]
    private float edgeTolerance = 0.05f;

    [SerializeField]
    [Tooltip("Distancia en el eje Z a la que se aleja la cámara cuando está al máximo de zoom (cerca del suelo)")]
    private float maxZoomDistance = 5f;

    [SerializeField]
    private float zoomMultiplier = 0.5f;

    //value set in various functions 
    //used to update the position of the camera base object.
    private Vector3 targetPosition;

    private float zoomHeight;

    //used to track and maintain velocity w/o a rigidbody
    private Vector3 horizontalVelocity;
    private Vector3 lastPosition;

    //tracks where the dragging action started
    Vector3 startDrag;

    private bool cameraCanMove = true;
    #endregion

    private void Awake()
    {
        cameraActions = new InputSystem_Actions();
        cameraTransform = GetComponentInChildren<CinemachineCamera>().transform;
    }

    private void OnEnable()
    {
        EnableCameraInput();
    }

    private void OnDisable()
    {
        DisableCameraInput();
    }

    private void Update()
    {
        if (cameraCanMove) 
        {
            //inputs
            GetKeyboardMovement();
            CheckMouseAtScreenEdge();
            DragCamera();

            //move base and camera objects
            UpdateVelocity();
            UpdateBasePosition();
            UpdateCameraPosition();
        }
    }

    #region Inputs
    private void GetKeyboardMovement()
    {
        Vector3 inputValue = movement.ReadValue<Vector2>().x * GetCameraRigRight()
                    + movement.ReadValue<Vector2>().y * GetCameraRigForward();

        inputValue = inputValue.normalized;

        if (inputValue.sqrMagnitude > 0.1f)
            targetPosition += inputValue;
    }

    private void CheckMouseAtScreenEdge()
    {
        if (!Application.isFocused) return;

        //mouse position is in pixels
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 moveDirection = Vector3.zero;

        //horizontal scrolling
        if (mousePosition.x < edgeTolerance * Screen.width)
            moveDirection += -GetCameraRigRight();
        else if (mousePosition.x > (1f - edgeTolerance) * Screen.width)
            moveDirection += GetCameraRigRight();

        //vertical scrolling
        if (mousePosition.y < edgeTolerance * Screen.height)
            moveDirection += -GetCameraRigForward();
        else if (mousePosition.y > (1f - edgeTolerance) * Screen.height)
            moveDirection += GetCameraRigForward();

        targetPosition += moveDirection;
    }

    private void DragCamera()
    {
        if (!Mouse.current.rightButton.isPressed)
            return;

        //create plane to raycast to
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (plane.Raycast(ray, out float distance))
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
                startDrag = ray.GetPoint(distance);
            else
                targetPosition += startDrag - ray.GetPoint(distance);
        }
    }
    #endregion

    #region Movement
    private void UpdateVelocity()
    {
        horizontalVelocity = (transform.position - lastPosition) / Time.deltaTime;
        horizontalVelocity.y = 0f;
        lastPosition = transform.position;
    }

    private void UpdateBasePosition()
    {
        if (targetPosition.sqrMagnitude > 0.1f)
        {
            //create a ramp up or acceleration
            speed = Mathf.Lerp(speed, maxSpeed, Time.deltaTime * acceleration);
            transform.position += targetPosition * speed * Time.deltaTime;
        }
        else
        {
            //create smooth slow down
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.deltaTime * damping);
            transform.position += horizontalVelocity * Time.deltaTime;
        }

        //reset for next frame
        targetPosition = Vector3.zero;
    }

    private void ZoomCamera(InputAction.CallbackContext obj)
    {
        Vector2 scrollValue = obj.ReadValue<Vector2>();
        float inputValue = -scrollValue.y;

        if (Mathf.Abs(inputValue) > 0.01f)
        {
            zoomHeight = cameraTransform.localPosition.y + (inputValue * stepSize * zoomMultiplier);

            if (zoomHeight < minHeight)
                zoomHeight = minHeight;
            else if (zoomHeight > maxHeight)
                zoomHeight = maxHeight;
        }
    }

    private void UpdateCameraPosition()
    {
        // 1. Calculamos el porcentaje del zoom (0 a 1).
        // 0 = Estamos en minHeight (muy cerca del suelo).
        // 1 = Estamos en maxHeight (muy alto).
        float zoomPercent = (zoomHeight - minHeight) / (maxHeight - minHeight);

        // 2. Calculamos la posición Z deseada usando ese porcentaje.
        // Si el porcentaje es 1 (lejos), Z será 0 (justo encima, visión vertical).
        // Si el porcentaje es 0 (cerca), Z será -maxZoomDistance (alejado, visión horizontal).
        float targetZ = Mathf.Lerp(-maxZoomDistance, 0f, zoomPercent);

        // 3. Creamos el target de posición local usando X=0 para mantenerla centrada.
        Vector3 zoomTarget = new Vector3(0f, zoomHeight, targetZ);

        // 4. Suavizamos la transición hacia ese punto local, usando tu variable zoomDampening.
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, zoomTarget, Time.deltaTime * zoomDampening);

        // 5. Mirar al CameraRig. Al cambiar la posición en Z e Y, LookAt ajustará el ángulo X automáticamente[cite: 1].
        cameraTransform.LookAt(transform);
    }

    private void RotateCamera(InputAction.CallbackContext obj)
    {
        if (!Mouse.current.middleButton.isPressed)
            return;

        float inputValue = obj.ReadValue<Vector2>().x;
        transform.rotation = Quaternion.Euler(0f, inputValue * maxRotationSpeed + transform.rotation.eulerAngles.y, 0f);
    }
    #endregion

    #region Input
    public void EnableCameraInput()
    {
        cameraCanMove = true;

        zoomHeight = cameraTransform.localPosition.y;
        cameraTransform.LookAt(transform);

        lastPosition = transform.position;

        movement = cameraActions.CameraControls.Movement;
        cameraActions.CameraControls.RotateCamera.performed += RotateCamera;
        cameraActions.CameraControls.ZoomCamera.performed += ZoomCamera;
        cameraActions.CameraControls.Enable();
    }

    public void DisableCameraInput()
    {
        cameraCanMove = false;

        cameraActions.CameraControls.RotateCamera.performed -= RotateCamera;
        cameraActions.CameraControls.ZoomCamera.performed -= ZoomCamera;
        cameraActions.CameraControls.Disable();
    }
    #endregion
    private Vector3 GetCameraRigForward()
    {
        Vector3 forward = transform.forward;
        forward.y = 0f;
        return forward;
    }

    private Vector3 GetCameraRigRight()
    {
        Vector3 right = transform.right;
        right.y = 0f;
        return right;
    }
}