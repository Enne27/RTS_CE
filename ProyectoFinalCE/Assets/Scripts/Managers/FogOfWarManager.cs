using System.Collections;
using System.Collections.Generic;
using UnityEngine;
struct RevealerData
{
    public Vector2 position;
    public float radius;
}

public class FogOfWarManager : MonoBehaviour
{
    #region Variables
    public static FogOfWarManager Instance { get; private set; }

    [Header("Configuraci�n del Mapa")]
    [Tooltip("Tama�o del mapa en unidades de Unity (ej. 100x100)")]
    public float mapSize = 100f;
    [Tooltip("Resoluci�n de la textura (A mayor resoluci�n, bordes m�s suaves pero m�s coste de CPU)")]
    public int textureRes = 256;
    //[Tooltip("Frecuencia de actualizaci�n en segundos (0.1s = 10 FPS)")]
    //public float updateInterval = 0.1f;

    [Header("Materiales")]
    [Tooltip("El material que tiene el Shader de la Niebla")]
    public Material fogMaterial;

    [SerializeField]private RenderTexture fogTexture;
    private Color32[] pixels;
    //This can be player.ants
    [SerializeField]
    public List<FogRevealer> activeRevealers;
    
    private float mapSize;
    private int textureRes;

    public ComputeShader cs;

    // Almacenamos el offset para centrar el mapa (asumiendo que el centro del mundo es 0,0,0)
    private float mapOriginOffset;
    #endregion

    private void Awake()
    {
        
    }

    private void OnDestroy()
    {
        revealerBuffer?.Release();
    }

    private void Update()
    {
        UpdateFogLogic();
    }
    private void InitializeFogTexture()
    {
        mapOriginOffset = mapSize / 2f;

        fogTexture = new RenderTexture(textureRes, textureRes, 0, RenderTextureFormat.ARGB32);
        fogTexture.enableRandomWrite = true;
        fogTexture.wrapMode = TextureWrapMode.Clamp;
        fogTexture.filterMode = FilterMode.Trilinear;
        fogTexture.anisoLevel = 4;
        fogTexture.Create();

        fogMaterial.SetTexture("_Texture2D", fogTexture);
        //fogMaterial.SetFloat("_Size", mapSize);
    }


    public void RegisterRevealer(FogRevealer revealer)
    {
        activeRevealers.Add(revealer);
    }

    public void UnregisterRevealer(FogRevealer revealer)
    {
        activeRevealers.Remove(revealer);
    }

    /* TBI
    public Color GetVisibilityAtPosition(Vector3 worldPos)
    {

    }

    private ComputeBuffer revealerBuffer;

    public Vector2Int WorldToFogCoords(Vector3 worldPos)
    {
        if (activeRevealers.Count == 0)
            return;

        RevealerData[] data = new RevealerData[activeRevealers.Count];

        for (int i = 0; i < activeRevealers.Count; i++)
        {
            Vector2Int coords = WorldToFogCoords(activeRevealers[i].transform.position);

            data[i] = new RevealerData
            {
                position = new Vector2(coords.x, coords.y),
                radius = (activeRevealers[i].visionRadius / mapSize) * textureRes
            };
        }

        revealerBuffer?.Release();

        revealerBuffer = new ComputeBuffer(
            data.Length,
            sizeof(float) * 3
        );

        revealerBuffer.SetData(data);

        int kernel = cs.FindKernel("CSMain");

        cs.SetTexture(kernel, "Result", fogTexture);

        cs.SetInt("_TextureSize", textureRes);
        cs.SetInt("_RevealerCount", data.Length);

        cs.SetBuffer(kernel, "_Revealers", revealerBuffer);

        int groups = Mathf.CeilToInt(textureRes / 8.0f);

        cs.Dispatch(kernel, groups, groups, 1);
    }

    // --- UTILIDADES ---

    private Vector2Int WorldToFogCoords(Vector3 worldPos)
    {
        // Mapear posici�n del mundo (ejes X y Z) a coordenadas de textura 2D (0 a textureRes)
        float mappedX = (worldPos.x + mapOriginOffset) / mapSize;
        float mappedY = (worldPos.z + mapOriginOffset) / mapSize; // Usamos Z porque el mapa est� en el plano XZ

        int texX = Mathf.RoundToInt(mappedX * textureRes);
        int texY = Mathf.RoundToInt(mappedY * textureRes);

        return new Vector2Int(texX, texY);
    }
}
