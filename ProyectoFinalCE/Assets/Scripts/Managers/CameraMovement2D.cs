using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CameraMovement2D : MonoBehaviour
{
    #region Variables
    private InputSystem_Actions cameraActions;
    private InputAction movement;

    [SerializeField] private CinemachineCamera virtualCamera;
    private Camera mainCamera;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 15f;

    [Header("Zoom")]
    [SerializeField] private float zoomStep = 5f;
    [SerializeField] private float zoomDampening = 10f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 20f;

    [Header("Mouse Edge")]
    [SerializeField] private bool useEdgeMovement = true;
    [SerializeField][Range(0f, 0.1f)] private float edgeTolerance = 0.05f;
    [SerializeField] private float edgeSpeed = 15f;

    [Header("Bounds")]
    [SerializeField] private Vector2 minBounds = new Vector2(-20, -20);
    [SerializeField] private Vector2 maxBounds = new Vector2(20, 20);

    private Vector3 targetPosition;
    private float targetZoom;
    private Vector3 startDrag;

    private Vector3 cameraInitialPosition;

    private Vector3 focusTarget;
    private bool isFocusing = false;

    [SerializeField] private float focusSpeed = 10f;
    #endregion

    private void Awake()
    {
        cameraActions = new InputSystem_Actions();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        movement = cameraActions.CameraControls.Movement;
        cameraActions.CameraControls.ZoomCamera.performed += ZoomCamera;
        cameraActions.CameraControls.Enable();

        targetZoom = virtualCamera.Lens.OrthographicSize;
    }

    private void Start()
    {
        cameraInitialPosition = virtualCamera.transform.position;
    }

    private void OnDisable()
    {
        cameraActions.CameraControls.ZoomCamera.performed -= ZoomCamera;
        cameraActions.CameraControls.Disable();
    }

    private void Update()
    {
        HandleKeyboardMovement();
        HandleEdgeMovement();
        HandleDrag();

        ApplyMovement();
        ApplyZoom();
        ClampPosition();

        if (isFocusing)
        {
            Vector3 direction = focusTarget - transform.position;

            if (direction.magnitude < 0.01f)
            {
                transform.position = focusTarget;
                isFocusing = false;
            }
            else
            {
                targetPosition += direction * focusSpeed * Time.deltaTime;
            }
        }
    }

    #region Movement
    private void HandleKeyboardMovement()
    {
        Vector2 input = movement.ReadValue<Vector2>();
        Vector3 dir = new Vector3(input.x, input.y, 0);

        targetPosition += dir * moveSpeed * Time.deltaTime;
    }

    private void HandleEdgeMovement()
    {
        if (!useEdgeMovement || !Application.isFocused) return;

        Vector2 mouse = Mouse.current.position.ReadValue();
        Vector3 dir = Vector3.zero;

        if (mouse.x < edgeTolerance * Screen.width)
            dir += Vector3.left;
        else if (mouse.x > (1f - edgeTolerance) * Screen.width)
            dir += Vector3.right;

        if (mouse.y < edgeTolerance * Screen.height)
            dir += Vector3.down;
        else if (mouse.y > (1f - edgeTolerance) * Screen.height)
            dir += Vector3.up;

        targetPosition += dir * edgeSpeed * Time.deltaTime;
    }

    private void HandleDrag()
    {
        if (!Mouse.current.rightButton.isPressed)
            return;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            startDrag = mouseWorld;
        }
        else
        {
            Vector3 delta = startDrag - mouseWorld;
            targetPosition += delta;
        }
    }

    private void ApplyMovement()
    {
        transform.position += targetPosition;
        targetPosition = Vector3.zero;
    }
    #endregion

    #region Zoom (tipo Unity editor)
    private void ZoomCamera(InputAction.CallbackContext ctx)
    {
        float scroll = -ctx.ReadValue<Vector2>().y;

        if (Mathf.Abs(scroll) < 0.01f) return;

        Vector3 mouseWorldBefore = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        targetZoom += scroll * zoomStep;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

        float newZoom = Mathf.Lerp(
            virtualCamera.Lens.OrthographicSize,
            targetZoom,
            Time.deltaTime * zoomDampening
        );

        virtualCamera.Lens.OrthographicSize = newZoom;

        Vector3 mouseWorldAfter = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Esto hace el zoom hacia el cursor (clave estilo Unity)
        Vector3 offset = mouseWorldBefore - mouseWorldAfter;
        transform.position += offset;
    }

    private void ApplyZoom()
    {
        float current = virtualCamera.Lens.OrthographicSize;

        float newZoom = Mathf.Lerp(
            current,
            targetZoom,
            Time.deltaTime * zoomDampening
        );

        virtualCamera.Lens.OrthographicSize = newZoom;
    }
    #endregion

    #region Bounds
    private void ClampPosition()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);

        transform.position = pos;
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (virtualCamera == null) return;

        Gizmos.color = Color.green;

        Vector3 center = cameraInitialPosition;

        Vector3 size = new Vector3(
            maxBounds.x - minBounds.x,
            maxBounds.y - minBounds.y,
            0f
        );

        Gizmos.DrawWireCube(center, size);
    }
    #endregion

    #region Outside Controll
    public void ZoomOnBuilding(Transform building)
    {
        if (building == null) return;

        focusTarget = new Vector3(
            building.position.x,
            building.position.y,
            transform.position.z
        );

        targetZoom = minZoom;
        isFocusing = true;
    }
    #endregion
}