using UnityEditor;
using UnityEngine;

public class CreateTexture2DAsset
{
    [MenuItem("Tools/Create Minimap Texture")]
    static void CreateTexture()
    {
        Texture2D tex = new Texture2D(256, 256);
        Color clear = Color.black;
        for (int x = 0; x < tex.width; x++)
            for (int y = 0; y < tex.height; y++)
                tex.SetPixel(x, y, clear);

        tex.Apply();

        AssetDatabase.CreateAsset(tex, "Assets/MinimapTexture.asset");
        AssetDatabase.SaveAssets();

        Debug.Log("MinimapTexture.asset created in Assets folder!");
    }
}

public class CameraProjection : MonoBehaviour
{
    [SerializeField] GameObject plane;

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

        Vector3 bl = GetPointAtHeight(bottomLeft, planeY);
        Vector3 tl = GetPointAtHeight(topLeft, planeY);
        Vector3 tr = GetPointAtHeight(topRight, planeY);
        Vector3 br = GetPointAtHeight(bottomRight, planeY);

        //Gizmos.color = Color.yellow;
        //Gizmos.DrawSphere(bl, 0.1f);
        //Gizmos.DrawSphere(tl, 0.1f);
        //Gizmos.DrawSphere(tr, 0.1f);
        //Gizmos.DrawSphere(br, 0.1f);

        Vector2 worldMin = new Vector2(mapOriginX, mapOriginZ);
        Vector2 worldMax = new Vector2(mapOriginX + mapWidth, mapOriginZ + mapHeight);

        Vector2 blUV = WorldToUV(bl, worldMin, worldMax);
        Vector2 tlUV = WorldToUV(tl, worldMin, worldMax);
        Vector2 trUV = WorldToUV(tr, worldMin, worldMax);
        Vector2 brUV = WorldToUV(br, worldMin, worldMax);

        Vector2 blPixel = new Vector2(blUV.x * texWidth, blUV.y * texHeight);
        Vector2 tlPixel = new Vector2(tlUV.x * texWidth, tlUV.y * texHeight);
        Vector2 trPixel = new Vector2(trUV.x * texWidth, trUV.y * texHeight);
        Vector2 brPixel = new Vector2(brUV.x * texWidth, brUV.y * texHeight);

        if (minimapTex == null || minimapTex.width != texWidth || minimapTex.height != texHeight)
            minimapTex = new Texture2D(texWidth, texHeight);

        Color clear = Color.black;
        Color lineColor = Color.yellow;
        for (int x = 0; x < texWidth; x++)
            for (int y = 0; y < texHeight; y++)
                minimapTex.SetPixel(x, y, clear);

        DrawLine(minimapTex, blPixel, tlPixel, lineColor);
        DrawLine(minimapTex, tlPixel, trPixel, lineColor);
        DrawLine(minimapTex, trPixel, brPixel, lineColor);
        DrawLine(minimapTex, brPixel, blPixel, lineColor);

        minimapTex.Apply();
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
}   