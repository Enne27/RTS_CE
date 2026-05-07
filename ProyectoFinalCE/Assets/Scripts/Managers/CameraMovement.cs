using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    #region Variables
    [SerializeField] InputActionAsset inputAsset;
    private InputActionMap cameraActions;
    private InputAction movement;
    private Transform cameraTransform;

    [SerializeField] private float maxSpeed = 5f;
    private float speed;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float damping = 15f;

    [Header("Zoom Settings")]
    [SerializeField] private float stepSize = 2f;
    [SerializeField] private float zoomDampening = 7.5f;
    [SerializeField] private float minZoom = 0f;      // Punto más cercano (hacia adelante)
    [SerializeField] private float maxZoom = 40f;     // Punto más lejano (hacia atrás)
    [SerializeField] private float zoomSpeed = 2f;

    private float targetZoom;                     // El valor de desplazamiento objetivo
    private float currentZoom;                    // El valor de desplazamiento suavizado actual
    private Vector3 cameraInitialLocalPos;        // Posición base de la cámara en el Rig


    [SerializeField] private float maxRotationSpeed = 1f;


    [SerializeField] [Range(0f, 0.1f)] private float edgeTolerance = 0.05f;

    [Header("Bounds")]
    [SerializeField] private Vector2 minBounds = new Vector2(-400, -400);
    [SerializeField] private Vector2 maxBounds = new Vector2(400, 400);

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
        cameraActions = inputAsset.FindActionMap("General");
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
            ClampPosition();
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
        horizontalVelocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        horizontalVelocity.y = 0f;
        lastPosition = transform.position;
    }

    private void UpdateBasePosition()
    {
        if (targetPosition.sqrMagnitude > 0.1f)
        {
            speed = Mathf.Lerp(speed, maxSpeed, Time.fixedDeltaTime * acceleration);
            transform.position += targetPosition * speed * Time.fixedDeltaTime;
        }
        else
        {
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.fixedDeltaTime * damping);
            transform.position += horizontalVelocity * Time.fixedDeltaTime;
        }

        targetPosition = Vector3.zero;
    }

    private void ZoomCamera(InputAction.CallbackContext obj)
    {
        float scrollValue = -obj.ReadValue<Vector2>().y;

        if (Mathf.Abs(scrollValue) > 0.01f)
        {
            targetZoom += (scrollValue * stepSize);
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }
    }

    private void UpdateCameraPosition()
    {
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomDampening);
        Vector3 zoomOffset = cameraTransform.localRotation * Vector3.forward * -currentZoom;

        cameraTransform.localPosition = cameraInitialLocalPos + zoomOffset;
    }
    #endregion

    #region Bounds
    private void ClampPosition()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.z = Mathf.Clamp(pos.z, minBounds.y, maxBounds.y);

        transform.position = pos;
    }
    #endregion

    #region Input
    public void EnableCameraInput()
    {
        cameraCanMove = true;

        cameraInitialLocalPos = cameraTransform.localPosition;

        targetZoom = 0f;
        currentZoom = 0f;

        movement = cameraActions.FindAction("Movement");
        cameraActions.FindAction("ZoomCamera").performed += ZoomCamera;
        cameraActions.Enable();
    }

    public void DisableCameraInput()
    {
        cameraCanMove = false;

        cameraActions.FindAction("ZoomCamera").performed -= ZoomCamera;
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