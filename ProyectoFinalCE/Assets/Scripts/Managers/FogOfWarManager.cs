using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogoOfWarManager : MonoBehaviour
{
    [SerializeField]
    Texture2D fogTexture;
    [SerializeField]
    Color32[] pixels;
    [SerializeField]
    List<FogRevealer> activeRevealers;

    float mapSize;
    int textureRes;
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
