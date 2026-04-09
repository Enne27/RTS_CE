using UnityEngine;
using UnityEngine.Rendering;

public class CameraProjection : MonoBehaviour
{

    [SerializeField] Camera mainCamera;
    [SerializeField] Camera minimapCamera;
    [SerializeField] GameObject plane;

    Vector3 bottomLeftPosition;
    Vector3 topLeftPosition;
    Vector3 topRightPosition;
    Vector3 bottomRightPosition;

    static Material lineMaterial;

    void Start()
    {
        RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
    }

    void OnDestroy()
    {
        RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
    }

    private void OnValidate()
    {
        minimapCamera.orthographicSize = 5 * plane.transform.lossyScale.x;
    }

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


    /*

    [Header("Minimap Settings")]
    [SerializeField] float mapOriginX = -50f;
    [SerializeField] float mapOriginZ = -50f;
    [SerializeField] float mapWidth = 100f;
    [SerializeField] float mapHeight = 100f;
    [SerializeField] int texWidth = 256;
    [SerializeField] int texHeight = 256;

    [SerializeField]private Texture2D minimapTex;


    void Update()
    {
        if (Camera.main == null || plane == null) return;

        Camera camera = Camera.main;
        float planeY = plane.transform.position.y;

        Ray bottomLeft = camera.ViewportPointToRay(new Vector3(0, 0, 0));
        Ray topLeft = camera.ViewportPointToRay(new Vector3(0, 1, 0));
        Ray topRight = camera.ViewportPointToRay(new Vector3(1, 1, 0));
        Ray bottomRight = camera.ViewportPointToRay(new Vector3(1, 0, 0));

        bottomLeftPosition = GetPointAtHeight(bottomLeft, planeY);
        topLeftPosition    = GetPointAtHeight(topLeft, planeY);
        topRightPosition   = GetPointAtHeight(topRight, planeY);
        bottomRightPosition= GetPointAtHeight(bottomRight, planeY);

        //Vector3 bl = GetPointAtHeight(bottomLeft, planeY);
        //Vector3 tl = GetPointAtHeight(topLeft, planeY);
        //Vector3 tr = GetPointAtHeight(topRight, planeY);
        //Vector3 br = GetPointAtHeight(bottomRight, planeY);

        //Vector2 worldMin = new Vector2(mapOriginX, mapOriginZ);
        //Vector2 worldMax = new Vector2(mapOriginX + mapWidth, mapOriginZ + mapHeight);

        //Vector2 blUV = WorldToUV(bl, worldMin, worldMax);
        //Vector2 tlUV = WorldToUV(tl, worldMin, worldMax);
        //Vector2 trUV = WorldToUV(tr, worldMin, worldMax);
        //Vector2 brUV = WorldToUV(br, worldMin, worldMax);

        //Vector2 blPixel = new Vector2(blUV.x * texWidth, blUV.y * texHeight);
        //Vector2 tlPixel = new Vector2(tlUV.x * texWidth, tlUV.y * texHeight);
        //Vector2 trPixel = new Vector2(trUV.x * texWidth, trUV.y * texHeight);
        //Vector2 brPixel = new Vector2(brUV.x * texWidth, brUV.y * texHeight);

        //if (minimapTex == null || minimapTex.width != texWidth || minimapTex.height != texHeight)
        //    minimapTex = new Texture2D(texWidth, texHeight);

        //Color clear = Color.black;
        //Color lineColor = Color.yellow;
        //for (int x = 0; x < texWidth; x++)
        //    for (int y = 0; y < texHeight; y++)
        //        minimapTex.SetPixel(x, y, clear);

        //DrawLine(minimapTex, blPixel, tlPixel, lineColor);
        //DrawLine(minimapTex, tlPixel, trPixel, lineColor);
        //DrawLine(minimapTex, trPixel, brPixel, lineColor);
        //DrawLine(minimapTex, brPixel, blPixel, lineColor);

        //minimapTex.Apply();
    }


    void Start()
    {
        RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
    }

    void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        GL.PushMatrix();
        {
            GL.LoadOrtho();
            GL.Begin(GL.LINES);
            {
                GL.Color(Color.red);
                GL.Vertex(topLeftPosition);
                GL.Vertex(topRightPosition);
                GL.Vertex(topRightPosition);
                GL.Vertex(bottomRightPosition);
                GL.Vertex(bottomRightPosition);
                GL.Vertex(bottomLeftPosition);
                GL.Vertex(bottomLeftPosition);
                GL.Vertex(topLeftPosition);
            }
            //GL.Begin(GL.TRIANGLES);
            //{
            //    GL.Color(Color.red);
            //    GL.Vertex(new Vector3(-1f, 1f, -1f));
            //    GL.Vertex(new Vector3(1f, 1f, 1f));
            //    GL.Vertex(new Vector3(1f, 1f, -1f));
            //}
            GL.End();
        }
        GL.PopMatrix();
    }

    public static Vector3 GetPointAtHeight(Ray ray, float height)
    {
        return ray.origin + (((ray.origin.y - height) / -ray.direction.y) * ray.direction);
    }

    Vector2 WorldToUV(Vector3 worldPos, Vector2 worldMin, Vector2 worldMax)
    {
        float u = (worldPos.x - worldMin.x) / (worldMax.x - worldMin.x);
        float v = (worldPos.z - worldMin.y) / (worldMax.y - worldMin.y);
        return new Vector2(u, v);
    }

    void DrawLine(Texture2D tex, Vector2 p1, Vector2 p2, Color col)
    {
        int x0 = Mathf.RoundToInt(p1.x);
        int y0 = Mathf.RoundToInt(p1.y);
        int x1 = Mathf.RoundToInt(p2.x);
        int y1 = Mathf.RoundToInt(p2.y);

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            if (x0 >= 0 && x0 < tex.width && y0 >= 0 && y0 < tex.height)
                tex.SetPixel(x0, y0, col);

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
    }
    */
}