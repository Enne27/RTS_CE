using UnityEngine;

public class GenerateMapResources : MonoBehaviour
{
    #region VARIABLES
    [Header("Prefabs to instantiate")]
    [SerializeField] GameObject resourcesZone;
    [SerializeField] GameObject antHill;

    [Header("Quantity")]
    [Tooltip("Cantidad de zonas de recursos iniciales en el mapa.")]
    [SerializeField] int resourcesQuantity;

    [Tooltip("Cantidad de hormigueros a instanciar al comienzo.")]
    [SerializeField] int antHillQuantity;

    [Header("Parameters")]
    [Tooltip("Distancia mínima entre estructuras.")]
    [SerializeField] float minDistance;
    #endregion

    private void Awake()
    {
        // antHillQuantity = GAME_MANAGER CANTIDAD DE PLAYERS

        InstantiateStartingZones();
    }

    private void InstantiateStartingZones()
    {
        if(antHill != null) 
        { 
            for (int i = 0; i < antHillQuantity; i++) {
                Vector3 pos = new Vector3();

                Instantiate(antHill, pos, new Quaternion(0, 0, 0, 0));
            }
        }

        if (resourcesZone != null) 
        {
            for (int i = 0; i < resourcesQuantity; i++) {
                Vector3 pos = new Vector3();

                Instantiate(resourcesZone, pos, new Quaternion(0,0,0,0));
            }
        }
    }
}
