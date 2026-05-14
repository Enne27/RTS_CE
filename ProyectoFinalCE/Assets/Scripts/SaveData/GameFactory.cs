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

    [Header("Building Data")]
    [SerializeField] private BuildingData queenChamberData;
    [SerializeField] private BuildingData broodChamberData;
    [SerializeField] private BuildingData storageChamberData;
    [SerializeField] private BuildingData tunnelData;
    [SerializeField] private BuildingData entranceData;
    [SerializeField] private BuildingData moundData;

    private Dictionary<string, GameObject> buildingPrefabs;
    private Dictionary<string, BuildingData> buildingDataMap;
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

        buildingDataMap = new Dictionary<string, BuildingData>();
        
        // Mapeo por nombre del ScriptableObject
        if (queenChamberData != null)
        {
            string key = queenChamberData.buildingType.ToString();
            buildingDataMap[key] = queenChamberData;
            buildingDataMap[key + "Data"] = queenChamberData;
            buildingDataMap[queenChamberData.name] = queenChamberData;
        }
        if (broodChamberData != null)
        {
            string key = broodChamberData.buildingType.ToString();
            buildingDataMap[key] = broodChamberData;
            buildingDataMap[key + "Data"] = broodChamberData;
            buildingDataMap[broodChamberData.name] = broodChamberData;
        }
        if (storageChamberData != null)
        {
            string key = storageChamberData.buildingType.ToString();
            buildingDataMap[key] = storageChamberData;
            buildingDataMap[key + "Data"] = storageChamberData;
            buildingDataMap[storageChamberData.name] = storageChamberData;
        }
        if (tunnelData != null)
        {
            string key = tunnelData.buildingType.ToString();
            buildingDataMap[key] = tunnelData;
            buildingDataMap[key + "Data"] = tunnelData;
            buildingDataMap[tunnelData.name] = tunnelData;
        }
        if (entranceData != null)
        {
            string key = entranceData.buildingType.ToString();
            buildingDataMap[key] = entranceData;
            buildingDataMap[key + "Data"] = entranceData;
            buildingDataMap[entranceData.name] = entranceData;
        }
        if (moundData != null)
        {
            string key = moundData.buildingType.ToString();
            buildingDataMap[key] = moundData;
            buildingDataMap[key + "Data"] = moundData;
            buildingDataMap[moundData.name] = moundData;
        }

        if (buildingDataMap.Count == 0)
        {
            Debug.LogWarning("No BuildingData assigned to GameFactory. Check the Inspector.");
        }
    }

    public Ant CreateAnt(ANT_TYPES type, Vector3 pos)
    {
        if (antPrefabs == null)
        {
            Debug.LogError("GameFactory.CreateAnt: Ant prefabs dictionary not initialized.");
            return null;
        }
        if (!antPrefabs.TryGetValue(type, out GameObject prefab) || prefab == null)
        {
            Debug.LogError($"GameFactory.CreateAnt: Prefab not assigned for ant type: {type}");
            return null;
        }

        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        Ant ant = obj.GetComponent<Ant>();
        if (ant == null)
        {
            Debug.LogError($"GameFactory.CreateAnt: prefab for {type} has no Ant component.");
            Destroy(obj);
            return null;
        }

        ant.antType = type;
        Debug.Log($"GameFactory.CreateAnt: created ant type={type} at position={pos}");
        return ant;
    }
    // private void PlaceBuilding(List<Vector3> buildingPositions)
    // {
    //     Building building = Instantiate(buildingPrefab, preview.transform.position, Quaternion.identity);
    //     GameManager.instance.player.structures.Add(building.gameObject);
    //     building.Setup(preview.data, preview.model.Rotation);
    //     grid.SetBuilding(building, buildingPositions);
    //     switch (preview.data.buildingType)
    //     {
    //         case BuildingType.QueenChamber:
    //             queenChambersCount++;
    //             constructionsBuilt.Add(building);
    //             // Llamar directamente a la función de construcción terminada si es necesario
    //             building.gameObject.GetComponentInChildren<QueenChamberFunction>()?.OnConstructionFinished();
    //             break;

    //         case BuildingType.BroodChamber:
    //             broodChambersCount++;
    //             constructionsBuilt.Add(building);
    //             building.gameObject.GetComponentInChildren<BroodChamberFunction>()?.OnConstructionFinished();
    //             break;

    //         case BuildingType.StorageChamber:
    //             storageChambersCount++;
    //             constructionsBuilt.Add(building);
    //             building.gameObject.GetComponentInChildren<StorageChamberFunction>()?.OnConstructionFinished();
    //             break;

    //         case BuildingType.Tunnel:
    //             TunnelFunction tunnel = building.GetComponentInChildren<TunnelFunction>();
    //             tunnel.DetectTunnels();

    //             HashSet<int> neighborPathIDs = new();

    //             foreach (TunnelFunction connection in tunnel.TunnelConnections)
    //             {
    //                 if (connection.pathID != 0)
    //                     neighborPathIDs.Add(connection.pathID);
    //             }

    //             if (neighborPathIDs.Count == 0)
    //             {
    //                 pathsCount++;
    //                 tunnel.pathID = pathsCount;

    //                 TunnelPath newPath = new(pathsCount, tunnel);
    //                 pathsBuilt.Add(newPath);
    //             }
    //             else if (neighborPathIDs.Count == 1)
    //             {
    //                 int id = neighborPathIDs.First();
    //                 tunnel.pathID = id;

    //                 TunnelPath path = pathsBuilt.Find(p => p.pathID == id);

    //                 if (!path.TunnelPieces.Contains(tunnel))
    //                     path.TunnelPieces.Add(tunnel);
    //             }
    //             else
    //             {
    //                 int mainID = neighborPathIDs.First();
    //                 TunnelPath mainPath = pathsBuilt.Find(p => p.pathID == mainID);

    //                 tunnel.pathID = mainID;
    //                 mainPath.TunnelPieces.Add(tunnel);

    //                 foreach (int otherID in neighborPathIDs)
    //                 {
    //                     if (otherID == mainID) continue;

    //                     TunnelPath otherPath = pathsBuilt.Find(p => p.pathID == otherID);

    //                     foreach (TunnelFunction piece in otherPath.TunnelPieces)
    //                     {
    //                         piece.pathID = mainID;

    //                         if (!mainPath.TunnelPieces.Contains(piece))
    //                             mainPath.TunnelPieces.Add(piece);
    //                     }

    //                     pathsBuilt.Remove(otherPath);
    //                     pathsCount--;
    //                 }
    //             }
    //             break;
    //         default:
    //             break;
    //     }
        
    //     Destroy(preview.gameObject);
    //     preview = null;
    // }
    public Building CreateBuilding(string type, Vector3 pos, float rotation = 0f)
    {
        Debug.Log($"GameFactory.CreateBuilding: requested type={type}, position={pos}, rotation={rotation}");

        if (buildingPrefabs == null)
        {
            Debug.LogError("GameFactory.CreateBuilding: Building prefabs dictionary not initialized.");
            return null;
        }
        
        if (!buildingPrefabs.TryGetValue(type, out GameObject prefab) || prefab == null)
        {
            Debug.LogError($"GameFactory.CreateBuilding: Building prefab not assigned for type: {type}. Check GameFactory inspector.");
            return null;
        }

        if (buildingDataMap == null)
        {
            Debug.LogError("GameFactory.CreateBuilding: Building data map not initialized.");
            return null;
        }

        if (!buildingDataMap.TryGetValue(type, out BuildingData buildingData) || buildingData == null)
        {
            Debug.LogError($"GameFactory.CreateBuilding: BuildingData not assigned for type: {type}. Check GameFactory inspector.");
            Debug.Log("GameFactory.CreateBuilding: available keys = " + string.Join(",", buildingDataMap.Keys));
            return null;
        }

        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        Building building = obj.GetComponent<Building>();
        if (building == null)
        {
            building = obj.AddComponent<Building>();
            Debug.LogWarning($"GameFactory.CreateBuilding: Added Building component dynamically to prefab for type={type}");
        }

        building.data = buildingData;
        building.Setup(buildingData, rotation);

        Debug.Log($"GameFactory.CreateBuilding: created building type={type} with data={buildingData.name}");
        return building;
    }
}