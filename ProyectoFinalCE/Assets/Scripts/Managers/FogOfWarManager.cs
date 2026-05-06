using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarManager : MonoBehaviour
{
    #region Variables
    public static FogOfWarManager Instance { get; private set; }

    [Header("Configuración del Mapa")]
    [Tooltip("Tamaño del mapa en unidades de Unity (ej. 100x100)")]
    public float mapSize = 100f;
    [Tooltip("Resolución de la textura (A mayor resolución, bordes más suaves pero más coste de CPU)")]
    public int textureRes = 256;
    [Tooltip("Frecuencia de actualización en segundos (0.1s = 10 FPS)")]
    public float updateInterval = 0.1f;

    [Header("Materiales")]
    [Tooltip("El material que tiene el Shader de la Niebla")]
    public Material fogMaterial2;

    private Texture2D fogTexture;
    private Color32[] pixels;
    private List<FogRevealer> activeRevealers = new List<FogRevealer>();

    // Almacenamos el offset para centrar el mapa (asumiendo que el centro del mundo es 0,0,0)
    private float mapOriginOffset;
    #endregion

    private void Awake()
    {
        // Inicializar el Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializeFogTexture();
    }

    private void Start()
    {
        StartCoroutine(UpdateFogRoutine());
    }

    private void InitializeFogTexture()
    {
        // Calcular el offset. Si mapSize es 100, el origen (0 en textura) será -50 en el mundo.
        mapOriginOffset = mapSize / 2f;

        fogTexture = new Texture2D(textureRes, textureRes, TextureFormat.RGBA32, false);
        fogMaterial2.SetTexture("_Texture2D", fogTexture);
        // Desactivar el wrap para evitar que la niebla se repita en los bordes
        fogTexture.wrapMode = TextureWrapMode.Clamp;
        // Filtro bilineal para que el Shader mezcle los colores suavemente
        fogTexture.filterMode = FilterMode.Bilinear;

        pixels = new Color32[textureRes * textureRes];

        // Inicializar todo el mapa en negro (No explorado)
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color32(0, 0, 0, 255);
        }

        fogTexture.SetPixels32(pixels);
        fogTexture.Apply();
    }


    public void RegisterRevealer(FogRevealer revealer)
    {
        if (!activeRevealers.Contains(revealer))
            activeRevealers.Add(revealer);
    }

    public void UnregisterRevealer(FogRevealer revealer)
    {
        if (activeRevealers.Contains(revealer))
            activeRevealers.Remove(revealer);
    }

    /// <summary>
    /// Permite a las entidades preguntar el estado de visibilidad en su posición.
    /// Retorna un Color32 donde R=Visible actual, G=Explorado.
    /// </summary>
    public Color32 GetVisibilityAtPosition(Vector3 worldPos)
    {
        Vector2Int texCoords = WorldToFogCoords(worldPos);

        // Evitar errores si la entidad sale de los límites del mapa
        if (texCoords.x < 0 || texCoords.x >= textureRes || texCoords.y < 0 || texCoords.y >= textureRes)
            return new Color32(0, 0, 0, 255); // Considerar negro fuera del mapa

        int index = texCoords.y * textureRes + texCoords.x;
        return pixels[index];
    }

    // --- LÓGICA PRINCIPAL ---

    private IEnumerator UpdateFogRoutine()
    {
        WaitForSeconds waitTime = new WaitForSeconds(updateInterval);

        while (true)
        {
            UpdateFogLogic();
            yield return waitTime;
        }
    }

    private void UpdateFogLogic()
    {
        // 1. LIMPIAR VISIBILIDAD ACTUAL (Solo el canal Rojo)
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].r = 0; // Apagamos la visión en tiempo real
            // Nota: No tocamos el canal Verde (g) porque es el historial permanente.
        }

        // 2. CALCULAR VISIÓN DE CADA REVELADOR
        foreach (var revealer in activeRevealers)
        {
            Vector2Int center = WorldToFogCoords(revealer.transform.position);

            // Convertir el radio del mundo (unidades) a radio de textura (píxeles)
            int radiusInPixels = Mathf.RoundToInt((revealer.visionRadius / mapSize) * textureRes);
            int radiusSqr = radiusInPixels * radiusInPixels; // Usamos distancia al cuadrado por rendimiento

            // Limitar la caja delimitadora (Bounding Box) a los bordes de la textura
            int minX = Mathf.Max(0, center.x - radiusInPixels);
            int maxX = Mathf.Min(textureRes - 1, center.x + radiusInPixels);
            int minY = Mathf.Max(0, center.y - radiusInPixels);
            int maxY = Mathf.Min(textureRes - 1, center.y + radiusInPixels);

            // Bucle solo sobre la caja delimitadora de la unidad (Optimización)
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    // Calcular distancia al cuadrado desde el centro
                    int dx = x - center.x;
                    int dy = y - center.y;
                    int distSqr = (dx * dx) + (dy * dy);

                    // Si está dentro del círculo de visión
                    if (distSqr <= radiusSqr)
                    {
                        int index = y * textureRes + x;
                        pixels[index].r = 255; // Visible ahora
                        pixels[index].g = 128; // Explorado permanente
                    }
                }
            }
        }

        // 3. APLICAR CAMBIOS A LA GPU
        fogTexture.SetPixels32(pixels);
        fogTexture.Apply();
    }

    // --- UTILIDADES ---

    private Vector2Int WorldToFogCoords(Vector3 worldPos)
    {
        // Mapear posición del mundo (ejes X y Z) a coordenadas de textura 2D (0 a textureRes)
        float mappedX = (worldPos.x + mapOriginOffset) / mapSize;
        float mappedY = (worldPos.z + mapOriginOffset) / mapSize; // Usamos Z porque el mapa está en el plano XZ

        int texX = Mathf.RoundToInt(mappedX * textureRes);
        int texY = Mathf.RoundToInt(mappedY * textureRes);

        return new Vector2Int(texX, texY);
    }
}