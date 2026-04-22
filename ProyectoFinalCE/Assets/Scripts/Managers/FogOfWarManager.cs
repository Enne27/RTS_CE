using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarManager : MonoBehaviour
{
    public static FogOfWarManager instance;

    [SerializeField]
    private Texture2D fogTexture;
    [SerializeField]
    private Color32[] pixels;
    //This can be player.ants
    [SerializeField]
    public List<FogRevealer> activeRevealers;
    
    private float mapSize;
    private int textureRes;

    void Start()
    {
        
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

    public IEnumerator UpdateFogRoutine()
    {

    }

    public Vector2Int WorldToFogCoords(Vector3 worldPos)
    {

    }*/
}
