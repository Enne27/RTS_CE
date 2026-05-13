using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingManager : MonoBehaviour
{
    #region SINGLETON
    public static BuildingManager Instance;
    private void Awake()
    {
        keyboard = Keyboard.current;
        mouse = Mouse.current;
        Instance = this;
    }
    #endregion
    
    public const float CELL_SIZE = 1f;
    [Header("Chambers Data")]
    [SerializeField] public BuildingData queenChamberData;
    [SerializeField] public BuildingData broodChamberData;
    [SerializeField] public BuildingData storageChamberData;
    [SerializeField] public BuildingData tunnelChamberData;

    [Header("Building References")]
    [SerializeField] private BuildingPreview previewPrefab;
    [SerializeField] private Building buildingPrefab;
    [SerializeField] private BuildingGrid grid;

    [HideInInspector] public BuildingPreview preview;

    private Keyboard keyboard;
    private Mouse mouse;
    [HideInInspector] public Vector3 mousePos;

    [Header("Builds")]
    public List<Building> constructionsBuilt;
    public List<TunnelPath> pathsBuilt;

    [Header("Build Counts")]
    public int queenChambersCount;
    public int broodChambersCount;
    public int storageChambersCount;
    public int pathsCount;

    [Header("BuildingMaterial")]
    [SerializeField] Material ConstructionMaterial;

    [SerializeField] public Material QueenChamberMaterial;
    [SerializeField] public Material BroodChamberMaterial;
    [SerializeField] public Material StorageChamberMaterial;
    private void Update()
    {
        mousePos = GetMouseWorldPosition();
        //Debug.Log(GetMouseWorldPosition());

        if (preview != null)
        {
            HandlePreview(preview.data ,mousePos);
        }
        else
        {
            if (keyboard.digit1Key.wasPressedThisFrame && queenChambersCount < queenChamberData.maxQuantityByEra[(int)GameManager.instance.player.currentEra])
            {
                preview = CreatePreview(queenChamberData, mousePos);
            }
            else if (keyboard.digit2Key.wasPressedThisFrame && broodChambersCount < broodChamberData.maxQuantityByEra[(int)GameManager.instance.player.currentEra])
            {
                preview = CreatePreview(broodChamberData, mousePos);
            }
            else if (keyboard.digit3Key.wasPressedThisFrame && storageChambersCount < storageChamberData.maxQuantityByEra[(int)GameManager.instance.player.currentEra])
            {
                preview = CreatePreview(storageChamberData, mousePos);
            }
            else if (keyboard.digit4Key.wasPressedThisFrame )
            {
                preview = CreatePreview(tunnelChamberData, mousePos);
            }
        }
    }

    private void HandlePreview(BuildingData data, Vector3 mouseWorldPosition)
    {
        preview.transform.position = mouseWorldPosition;

        List<Vector3> buildPosition = preview.model.GetAllBuilddingPositions();
        bool canBuild = grid.CanBuild(buildPosition);

        if (canBuild)
        {
            preview.transform.position = GetSnappedCenterPosition(buildPosition);

            Inventory inventory = GameManager.instance.player.inventory;
            ForagingChamberFunction foragingChamber = ForagingChamberFunction.Instance;

            // Recursos totales (storage + foraging)
            int totalMaterials = inventory.materials + inventory.materialsInForaging;

            bool hasResources = totalMaterials >= data.costMC;

            if (hasResources)
            {
                preview.ChangeState(BuildingPreview.BuildingPreviewState.POSITIVE);

                if (mouse.leftButton.wasPressedThisFrame)
                {
                    // =========================
                    // MATERIALES
                    // =========================

                    int materialsNeeded = data.costMC;

                    // Primero gastar foraging
                    if (inventory.materialsInForaging >= materialsNeeded)
                    {
                        inventory.materialsInForaging -= materialsNeeded;

                        // Actualizar chamber visual
                        foragingChamber.RemoveResource(ResourceType.material, materialsNeeded);
                    }
                    else
                    {
                        int fromForaging = inventory.materialsInForaging;

                        // Vaciar foraging
                        inventory.materialsInForaging = 0;

                        // Actualizar chamber visual
                        if (fromForaging > 0)
                        {
                            foragingChamber.RemoveResource(ResourceType.material, fromForaging);
                        }

                        // Lo restante desde storage
                        materialsNeeded -= fromForaging;

                        inventory.materials -= materialsNeeded;
                    }

                    
                    foragingChamber.UpdateUI();

                    PlaceBuilding(buildPosition);
                }

                if (mouse.middleButton.wasPressedThisFrame)
                {
                    CancelPreview();
                    return;
                }
            }
            else
            {
                preview.ChangeState(BuildingPreview.BuildingPreviewState.NEGATIVE);

                if (mouse.middleButton.wasPressedThisFrame)
                {
                    CancelPreview();
                    return;
                }
            }
        }
        else
        {
            preview.ChangeState(BuildingPreview.BuildingPreviewState.NEGATIVE);

            if (mouse.middleButton.wasPressedThisFrame)
            {
                CancelPreview();
                return;
            }
        }
    }

    public void CancelPreview()
    {
        if (preview != null)
        {
            Destroy(preview.gameObject);
            preview = null;
        }
    }

    private void PlaceBuilding(List<Vector3> buildingPositions)
    {
        Building building = Instantiate(buildingPrefab, preview.transform.position, Quaternion.identity);
        building.Setup(preview.data, preview.model.Rotation);
        grid.SetBuilding(building, buildingPositions);

        building.gameObject.GetComponentInChildren<Renderer>().material = ConstructionMaterial;
        //VFXManager.Instance.PlayConstructionParticles(preview.transform.position, building.data.constructionTime);

        switch (preview.data.buildingType)
        {
            case BuildingType.QueenChamber:

                if (TimeManager.Instance)
                {
                    TimeManager.Instance.OneShotTimer(building.data.constructionTime,
                        () => building.gameObject.GetComponentInChildren<QueenChamberFunction>().OnConstructionFinished()
                    );
                    queenChambersCount++;
                    constructionsBuilt.Add(building);
                }
                break;

            case BuildingType.BroodChamber:

                if (TimeManager.Instance)
                {
                    TimeManager.Instance.OneShotTimer(building.data.constructionTime,
                        () => building.gameObject.GetComponentInChildren<BroodChamberFunction>().OnConstructionFinished()
                    );
                    broodChambersCount++;
                    constructionsBuilt.Add(building);
                }
                break;

            case BuildingType.StorageChamber:
                //building.gameObject.GetComponentInChildren<Renderer>().material = ConstructionMaterial;

                if (TimeManager.Instance)
                {
                    TimeManager.Instance.OneShotTimer(building.data.constructionTime,
                        () => building.gameObject.GetComponentInChildren<StorageChamberFunction>().OnConstructionFinished()
                    );
                }
                storageChambersCount++;
                constructionsBuilt.Add(building);
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

                    return;
                }

                if (neighborPathIDs.Count == 1)
                {
                    int id = neighborPathIDs.First();

                    tunnel.pathID = id;

                    TunnelPath path = pathsBuilt.Find(p => p.pathID == id);

                    if (!path.TunnelPieces.Contains(tunnel))
                        path.TunnelPieces.Add(tunnel);

                    return;
                }


                int mainID = neighborPathIDs.First();
                TunnelPath mainPath = pathsBuilt.Find(p => p.pathID == mainID);

                // Añadir el nuevo túnel al principal
                tunnel.pathID = mainID;
                mainPath.TunnelPieces.Add(tunnel);

                // Fusionar los demás
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

                
                
                break;
            default:
                break;
        }
        
        Destroy(preview.gameObject);
        preview = null;
    }

    private Vector3 GetSnappedCenterPosition(List<Vector3> allbuildingPositions)
    {
        List<int> xs = allbuildingPositions.Select(p => Mathf.FloorToInt(p.x)).ToList();
        List<int> ys = allbuildingPositions.Select(p => Mathf.FloorToInt(p.y)).ToList();
        float centerx = (xs.Min() + xs.Max()) / 2f + CELL_SIZE / 2f;
        float centery = (ys.Min() + ys.Max()) / 2f + CELL_SIZE / 2f;
        return new(centerx, centery, 45f);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreen = mouse.position.ReadValue();
        mouseScreen.z = Mathf.Abs(grid.transform.position.z);
        return Camera.main.ScreenToWorldPoint(mouseScreen);
    }

    public BuildingPreview CreatePreview(BuildingData data, Vector3 position)
    {
        BuildingPreview buildingPreview = Instantiate(previewPrefab, position, Quaternion.identity);
        buildingPreview.Setup(data);
        return buildingPreview;
    }
}