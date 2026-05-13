using System.Collections.Generic;
using UnityEngine;
using static PlayerConstants;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
public class GameFactory : MonoBehaviour
{
    #region SINGLETON
    public static GameFactory Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Inicializar diccionarios inmediatamente
        InitializeAntPrefabs();
        InitializeBuildingPrefabs();
    }
    #endregion

    #region ANT PREFABS
    [Header("Ants Prefabs")]
    [SerializeField] private GameObject soldierAnt;
    [SerializeField] private GameObject explorerAnt;
    [SerializeField] private GameObject workerAnt;
    [SerializeField] private GameObject berserkerAnt;
    [SerializeField] private GameObject acidAnt;
    [SerializeField] private GameObject crazyAnt;
    [SerializeField] private GameObject kamikazeAnt;

    private Dictionary<ANT_TYPES, GameObject> antPrefabs;
    #endregion

    #region BUILDING PREFABS
    [Header("Building Prefabs")]
    [SerializeField] private GameObject queenChamberPrefab;
    [SerializeField] private GameObject broodChamberPrefab;
    [SerializeField] private GameObject storageChamberPrefab;
    [SerializeField] private GameObject tunnelPrefab;
    [SerializeField] private GameObject entrancePrefab;
    [SerializeField] private GameObject moundPrefab;

    private Dictionary<string, GameObject> buildingPrefabs;
    #endregion

    private void InitializeAntPrefabs()
    {
        antPrefabs = new Dictionary<ANT_TYPES, GameObject>
        {
            { ANT_TYPES.SOLDIER, soldierAnt },
            { ANT_TYPES.EXPLORER, explorerAnt },
            { ANT_TYPES.WORKER, workerAnt },
            { ANT_TYPES.BERSERKER, berserkerAnt },
            { ANT_TYPES.ACID, acidAnt },
            { ANT_TYPES.CRAZY, crazyAnt },
            { ANT_TYPES.KAMIKAZE, kamikazeAnt }
        };
    }

    private void InitializeBuildingPrefabs()
    {
        buildingPrefabs = new Dictionary<string, GameObject>
        {
            // Nombres limpios (usando enum)
            { "QueenChamber", queenChamberPrefab },
            { "BroodChamber", broodChamberPrefab },
            { "StorageChamber", storageChamberPrefab },
            { "Tunnel", tunnelPrefab },
            { "Entrance", entrancePrefab },
            { "Mound", moundPrefab },

            // Compatibilidad con nombres antiguos que terminan en "Data"
            { "QueenChamberData", queenChamberPrefab },
            { "BroodChamberData", broodChamberPrefab },
            { "StorageChamberData", storageChamberPrefab },
            { "TunnelData", tunnelPrefab },
            { "MoundData", moundPrefab }
        };
    }

    public Ant CreateAnt(ANT_TYPES type, Vector3 pos)
    {
        if (antPrefabs == null) { Debug.LogError("Ant prefabs dictionary not initialized."); return null; }
        if (!antPrefabs.TryGetValue(type, out GameObject prefab) || prefab == null)
        { Debug.LogError($"Prefab not assigned for ant type: {type}"); return null; }

        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        Ant ant = obj.GetComponent<Ant>();
        if (ant != null) ant.antType = type;
        return ant;
    }
    private void PlaceBuilding(List<Vector3> buildingPositions)
    {
        Building building = Instantiate(buildingPrefab, preview.transform.position, Quaternion.identity);
        GameManager.instance.player.structures.Add(building.gameObject);
        building.Setup(preview.data, preview.model.Rotation);
        grid.SetBuilding(building, buildingPositions);
        switch (preview.data.buildingType)
        {
            case BuildingType.QueenChamber:
                queenChambersCount++;
                constructionsBuilt.Add(building);
                // Llamar directamente a la función de construcción terminada si es necesario
                building.gameObject.GetComponentInChildren<QueenChamberFunction>()?.OnConstructionFinished();
                break;

            case BuildingType.BroodChamber:
                broodChambersCount++;
                constructionsBuilt.Add(building);
                building.gameObject.GetComponentInChildren<BroodChamberFunction>()?.OnConstructionFinished();
                break;

            case BuildingType.StorageChamber:
                storageChambersCount++;
                constructionsBuilt.Add(building);
                building.gameObject.GetComponentInChildren<StorageChamberFunction>()?.OnConstructionFinished();
                break;

            case BuildingType.Tunnel:
                TunnelFunction tunnel = building.GetComponentInChildren<TunnelFunction>();
                tunnel.DetectTunnels();

                HashSet<int> neighborPathIDs = new();

                foreach (TunnelFunction connection in tunnel.TunnelConnections)
                {
                    if (connection.pathID != 0)
                        neighborPathIDs.Add(connection.pathID);
                }

                if (neighborPathIDs.Count == 0)
                {
                    pathsCount++;
                    tunnel.pathID = pathsCount;

                    TunnelPath newPath = new(pathsCount, tunnel);
                    pathsBuilt.Add(newPath);
                }
                else if (neighborPathIDs.Count == 1)
                {
                    int id = neighborPathIDs.First();
                    tunnel.pathID = id;

                    TunnelPath path = pathsBuilt.Find(p => p.pathID == id);

                    if (!path.TunnelPieces.Contains(tunnel))
                        path.TunnelPieces.Add(tunnel);
                }
                else
                {
                    int mainID = neighborPathIDs.First();
                    TunnelPath mainPath = pathsBuilt.Find(p => p.pathID == mainID);

                    tunnel.pathID = mainID;
                    mainPath.TunnelPieces.Add(tunnel);

                    foreach (int otherID in neighborPathIDs)
                    {
                        if (otherID == mainID) continue;

                        TunnelPath otherPath = pathsBuilt.Find(p => p.pathID == otherID);

                        foreach (TunnelFunction piece in otherPath.TunnelPieces)
                        {
                            piece.pathID = mainID;

                            if (!mainPath.TunnelPieces.Contains(piece))
                                mainPath.TunnelPieces.Add(piece);
                        }

                        pathsBuilt.Remove(otherPath);
                        pathsCount--;
                    }
                }
                break;
            default:
                break;
        }
        
        Destroy(preview.gameObject);
        preview = null;
    }
    public Building CreateBuilding(string type, Vector3 pos, float rotation = 0f)
    {
        if (buildingPrefabs == null) { Debug.LogError("Building prefabs dictionary not initialized."); return null; }
        if (!buildingPrefabs.TryGetValue(type, out GameObject prefab) || prefab == null)
        { Debug.LogError($"Building prefab not assigned for type: {type}. Check GameFactory inspector."); return null; }

        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        Building building = obj.GetComponent<Building>();
        if (building == null)
        {
            Debug.LogError($"Prefab {type} has no Building component.");
            Destroy(obj);
            return null;
        }
        if (building.data == null)
        {
            Debug.LogError($"BuildingData is missing on prefab {type}. Assign a BuildingData ScriptableObject.");
            Destroy(obj);
            return null;
        }
        building.Setup(building.data, rotation);
        return building;
    }
}