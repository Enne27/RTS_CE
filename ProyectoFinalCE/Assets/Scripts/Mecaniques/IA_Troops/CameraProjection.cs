using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CameraProjection : MonoBehaviour
{

    [SerializeField] Camera mainCamera;
    [SerializeField] Camera minimapCamera;
    [SerializeField] GameObject plane;
    [SerializeField] GameObject cameraRig;
    [SerializeField] GraphicRaycaster m_Raycaster;
    [SerializeField] InputActionAsset inputAsset;

    [SerializeField] Color playerAntColor = Color.green;
    [SerializeField] Color enemyAntColor = Color.red;
    [SerializeField] float antSize = 0.005f;

    private InputActionMap general;
    private InputAction leftClickAction;

    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    Vector3 bottomLeftPosition;
    Vector3 topLeftPosition;
    Vector3 topRightPosition;
    Vector3 bottomRightPosition;

    bool leftclickStarted = false;

    static Material lineMaterial;

    private void Awake()
    {
        general = inputAsset.FindActionMap("Gameplay");
        // leftClickAction = general.FindAction("leftClick");
    }
    private void OnEnable()
    {
        general.FindAction("leftClick").started += (InputAction.CallbackContext ctx) => { leftclickStarted = true; };
        general.FindAction("leftClick").canceled += (InputAction.CallbackContext ctx) => { leftclickStarted = false; };
    }
    void Start()
    {
        SetRenderingEnabled(true);
        m_EventSystem = GetComponent<EventSystem>();
    }

    void OnDestroy()
    {
        SetRenderingEnabled(false);
    }

    // private void OnDisable()
    // {
    //     leftClickAction.started -= OnLeftClickStarted;
    //     leftClickAction.canceled -= OnLeftClickCanceled;

    //     SetRenderingEnabled(false);
    // }

    // private void Start()
    // {
    //     m_EventSystem = EventSystem.current;
    // }

    // private void OnLeftClickStarted(InputAction.CallbackContext ctx)
    // {
    //     leftclickStarted = true;
    // }

    // private void OnLeftClickCanceled(InputAction.CallbackContext ctx)
    // {
    //     leftclickStarted = false;
    // }

    void Update()
    {
        if (mainCamera == null || plane == null) return;

        float planeY = plane.transform.position.y;

        Ray bottomLeft = mainCamera.ViewportPointToRay(new Vector3(0, 0, 0));
        Ray topLeft = mainCamera.ViewportPointToRay(new Vector3(0, 1, 0));
        Ray topRight = mainCamera.ViewportPointToRay(new Vector3(1, 1, 0));
        Ray bottomRight = mainCamera.ViewportPointToRay(new Vector3(1, 0, 0));

        bottomLeftPosition = GetPointAtHeight(bottomLeft, planeY);
        topLeftPosition = GetPointAtHeight(topLeft, planeY);
        topRightPosition = GetPointAtHeight(topRight, planeY);
        bottomRightPosition = GetPointAtHeight(bottomRight, planeY);

        if (leftclickStarted)
        {
            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            m_Raycaster.Raycast(m_PointerEventData, results);

            Vector2 localPoint;
            foreach (RaycastResult result in results)
            {
                RectTransform rect = result.gameObject.GetComponent<RectTransform>();
                if (rect == null) continue;
                Vector2 size = rect.sizeDelta;
                if (result.gameObject.GetComponent<RawImage>())
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        rect,
                        m_PointerEventData.position,
                        null,
                        out localPoint
                    );

                    Vector2 normalizedPoint = new Vector2(
                        localPoint.x / (size.x * 0.5f),
                        localPoint.y / (size.y * 0.5f)
                    );

                    cameraRig.transform.position = new Vector3(normalizedPoint.x * 500, cameraRig.transform.position.y, normalizedPoint.y * 500);
                    //Debug.Log("Local: " + localPoint);
                    //Debug.Log("Normalized [-1,1]: " + normalizedPoint);
                }
            }
        }
    }

    public void SetRenderingEnabled(bool enabled)
    {
        if (enabled) RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
        else RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
    }
    void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera != minimapCamera) return;

        if (minimapCamera == null) return;

        CreateMaterial();
        lineMaterial.SetPass(0);

        Vector3 tl = minimapCamera.WorldToViewportPoint(topLeftPosition);
        Vector3 tr = minimapCamera.WorldToViewportPoint(topRightPosition);
        Vector3 br = minimapCamera.WorldToViewportPoint(bottomRightPosition);
        Vector3 bl = minimapCamera.WorldToViewportPoint(bottomLeftPosition);

        GL.PushMatrix();
        GL.LoadOrtho();

        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        
        GL.Vertex(new Vector3(tl.x, tl.y, 0));
        GL.Vertex(new Vector3(tr.x, tr.y, 0));

        GL.Vertex(new Vector3(tr.x, tr.y, 0));
        GL.Vertex(new Vector3(br.x, br.y, 0));

        GL.Vertex(new Vector3(br.x, br.y, 0));
        GL.Vertex(new Vector3(bl.x, bl.y, 0));

        GL.Vertex(new Vector3(bl.x, bl.y, 0));
        GL.Vertex(new Vector3(tl.x, tl.y, 0));

        GL.End();

        lineMaterial.SetPass(0);

        GL.Begin(GL.QUADS);

        GL.Color(playerAntColor);

        foreach (Ant ant in GameManager.instance.player.ants)
        {
            if (ant == null) continue;
            DrawAntOnMinimap(ant);
        }

        GL.Color(enemyAntColor);

        foreach (Ant ant in GameManager.instance.playerIA.ants)
        {
            if (ant == null) continue;
            DrawAntOnMinimap(ant);
        }

        GL.End();

        GL.PopMatrix();
    }

    static void CreateMaterial()
    {
        if (lineMaterial != null) return;

        Shader shader = Shader.Find("Hidden/Internal-Colored");
        lineMaterial = new Material(shader);
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;

        lineMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        lineMaterial.SetInt("_Cull", (int)CullMode.Off);
        lineMaterial.SetInt("_ZWrite", 0);
    }

    public static Vector3 GetPointAtHeight(Ray ray, float height)
    {
        return ray.origin + (((ray.origin.y - height) / -ray.direction.y) * ray.direction);
    }

    private void DrawAntOnMinimap(Ant ant)
    {
        Vector3 vp = minimapCamera.WorldToViewportPoint(ant.transform.position);

        if (vp.z < 0 ||
            vp.x < 0 || vp.x > 1 ||
            vp.y < 0 || vp.y > 1)
            return;

        float s = antSize;

        GL.Vertex(new Vector3(vp.x - s, vp.y - s, 0));
        GL.Vertex(new Vector3(vp.x - s, vp.y + s, 0));
        GL.Vertex(new Vector3(vp.x + s, vp.y + s, 0));
        GL.Vertex(new Vector3(vp.x + s, vp.y - s, 0));
    }

    // public void SetRenderingEnabled(bool enabled)
    // {
    //     if (enabled)
    //         RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
    //     else
    //         RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
    // }
}
