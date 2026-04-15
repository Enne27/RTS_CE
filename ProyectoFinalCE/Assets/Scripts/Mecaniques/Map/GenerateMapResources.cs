using UnityEngine;

public class GenerateMapResources : MonoBehaviour
{
    #region VARIABLES
    [Header("Prefabs to instantiate")]
    [SerializeField] GameObject resourcesZone;
    [SerializeField] GameObject antHill;

    [Header("Quantity")]
    [Tooltip("Cantidad de zonas de recursos iniciales en el mapa.")]
    [SerializeField] public int resourcesQuantity;

    [Tooltip("Cantidad de hormigueros a instanciar al comienzo.")]
    [SerializeField] public int antHillQuantity;

    [Header("Parameters")]
    [Tooltip("Distancia mínima entre hormigueros (celdas).")]
    [SerializeField] public int minDistanceAntHill = 10;
    [Tooltip("Distancia mínima entre estructuras (celdas).")]
    [SerializeField] public int minDistanceResources = 3;
    #endregion

    private void Awake()
    {
        // antHillQuantity = GAME_MANAGER CANTIDAD DE PLAYERS
    }

    public void InstantiateAntHill(Vector3 pos)
    {
        if(antHill != null)
            Instantiate(antHill, pos, Quaternion.identity);
    }

    public void InstantiateResourcesZone(Vector3 pos)
    {
        if (resourcesZone != null) 
            Instantiate(resourcesZone, pos, Quaternion.identity);
    }
}
