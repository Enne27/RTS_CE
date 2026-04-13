using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    private InputSystem_Actions cameraActions;
    private InputAction movement;
    private Transform cameraTransform;

    [SerializeField]
    private float maxSpeed = 5f;
    [SerializeField]
    private float acceleration = 10f;
    [SerializeField]
    private float damping = 15f;

    [SerializeField]
    private float stepSize = 2f;
    [SerializeField]
    private float zoomDampening = 7.5f;
    [SerializeField]
    private float minHeight = 5f;
    [SerializeField]
    private float maxHeight = 50f;
    [SerializeField]
    private float zoomSpeed = 2f;

    [SerializeField]
    private float maxRotationSpeed = 1f;

    [SerializeField]
    [Range(0f, 0.1f)]
    private float edgeTolerance = 0.05f;

    [SerializeField]
    [Tooltip("Ángulo de la cámara al hacer zoom al máximo (más horizontal)")]
    private float zoomedInAngle = 20f;

    [SerializeField]
    [Tooltip("Ángulo de la cámara al alejarla al máximo (más vertical/picado)")]
    private float zoomedOutAngle = 80f;

    [SerializeField]
    [Tooltip("Distancia en el eje Z a la que se aleja la cámara cuando está al máximo de zoom (cerca del suelo)")]
    private float maxZoomDistance = 15f;

    //value set in various functions 
    //used to update the position of the camera base object.
    private Vector3 currentInputVector;

    private float zoomHeight;

    //used to track and maintain velocity w/o a rigidbody
    private Vector3 horizontalVelocity;
    private Vector3 lastPosition;

    //tracks where the dragging action started
    Vector3 startDrag;

    private void Awake()
    {
        cameraActions = new InputSystem_Actions();
        cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    private void OnEnable()
    {
        zoomHeight = cameraTransform.localPosition.y;
        cameraTransform.LookAt(transform);

        lastPosition = transform.position;

        movement = cameraActions.CameraControls.Movement;
        cameraActions.CameraControls.RotateCamera.performed += RotateCamera;
        cameraActions.CameraControls.ZoomCamera.performed += ZoomCamera;
        cameraActions.CameraControls.Enable();
    }

    private void OnDisable()
    {
        cameraActions.CameraControls.RotateCamera.performed -= RotateCamera;
        cameraActions.CameraControls.ZoomCamera.performed -= ZoomCamera;
        cameraActions.CameraControls.Disable();
    }

    private void Update()
    {
        // 1. Limpiamos el input del frame anterior
        currentInputVector = Vector3.zero;

        // 2. Recogemos inputs de teclado y bordes
        GetKeyboardMovement();
        CheckMouseAtScreenEdge();

        // 3. Normalizamos el input para no movernos más rápido en diagonal
        if (currentInputVector.magnitude > 1f)
            currentInputVector.Normalize();

        // 4. El drag se procesa de forma independiente
        DragCamera();

        // 5. Aplicar movimiento
        UpdateBasePosition();
        UpdateCameraPosition(); // Tu método de zoom
    }


    private void GetKeyboardMovement()
    {
        Vector2 input = movement.ReadValue<Vector2>();
        currentInputVector += (input.x * GetCameraRigRight()) + (input.y * GetCameraRigForward());
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
                currentInputVector += startDrag - ray.GetPoint(distance);
        }
    }

    private void CheckMouseAtScreenEdge()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // Evita que la cámara se mueva si estás clicando en otra ventana
        if (!Application.isFocused) return;

        if (mousePosition.x < edgeTolerance * Screen.width)
            currentInputVector += -GetCameraRigRight();
        else if (mousePosition.x > (1f - edgeTolerance) * Screen.width)
            currentInputVector += GetCameraRigRight();

        if (mousePosition.y < edgeTolerance * Screen.height)
            currentInputVector += -GetCameraRigForward();
        else if (mousePosition.y > (1f - edgeTolerance) * Screen.height)
            currentInputVector += GetCameraRigForward();
    }

    private void UpdateBasePosition()
    {
        if (currentInputVector.sqrMagnitude > 0.01f)
        {
            // Si hay input, interpolamos nuestra velocidad actual hacia la velocidad máxima
            Vector3 targetVelocity = currentInputVector * maxSpeed;
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, targetVelocity, Time.deltaTime * acceleration);
        }
        else
        {
            // Si soltamos los controles, la velocidad frena suavemente hacia cero
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.deltaTime * damping);
        }

        // Aplicamos la física limpia al transform
        transform.position += horizontalVelocity * Time.deltaTime;
    }

    private void ZoomCamera(InputAction.CallbackContext obj)
    {
        // 1. Leemos el valor puro
        Vector2 scrollValue = obj.ReadValue<Vector2>();

        // Descomenta esta línea para ver en consola qué valor exacto te manda tu ratón
        // Debug.Log($"Scroll detectado: {scrollValue.y}");

        // 2. Quitamos la división entre 100 por ahora y usamos el valor directo
        float inputValue = -scrollValue.y;

        // 3. Bajamos drásticamente el límite (threshold) para asegurarnos de que entra
        if (Mathf.Abs(inputValue) > 0.01f)
        {
            // 4. Aplicamos un multiplicador pequeño aquí en lugar de dividir antes
            float zoomMultiplier = 0.5f; // Ajusta esto si el zoom es muy rápido o muy lento
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

    //gets the horizontal forward vector of the camera
    private Vector3 GetCameraRigForward()
    {
        Vector3 forward = transform.forward;
        forward.y = 0f;
        return forward;
    }

    //gets the horizontal right vector of the camera
    private Vector3 GetCameraRigRight()
    {
        Vector3 right = transform.right;
        right.y = 0f;
        return right;
    }
}