using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingManager : MonoBehaviour
{
    public const float CELL_SIZE = 1f;
    [Header("Chambers Data")]
    [SerializeField] private BuildingData queenChamberData;
    [SerializeField] private BuildingData broodChamberData;
    [SerializeField] private BuildingData storageChamberData;
    [SerializeField] private BuildingData tunnelChamberData;

    [Header("Building References")]
    [SerializeField] private BuildingPreview previewPrefab;
    [SerializeField] private Building buildingPrefab;
    [SerializeField] private BuildingGrid grid;

    private BuildingPreview preview;

    private Keyboard keyboard;
    private Mouse mouse;

    [Header("Builds")]
    public List<Building> constructionsBuilt;

    [Header("Build Counts")]
    public int queenChambersCount;
    public int broodChambersCount;
    public int storageChambersCount;

    private void Awake()
    {
        keyboard = Keyboard.current;
        mouse = Mouse.current;
    }

    private void Update()
    {

        Vector3 mousePos = GetMouseWorldPosition();
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

    private void HandlePreview(BuildingData data ,Vector3 mouseWorldPosition)
    {
        preview.transform.position = mouseWorldPosition;
        List<Vector3> buildPosition = preview.model.GetAllBuilddingPositions();
        bool canBuild = grid.CanBuild(buildPosition);

        if (canBuild)
        {
            preview.transform.position = GetSnappedCenterPosition(buildPosition);
            if((GameManager.instance.player.inventory.materials >= data.costMC) && (GameManager.instance.player.inventory.eggs >= data.costHV))
            {
                preview.ChangeState(BuildingPreview.BuildingPreviewState.POSITIVE);

                if (mouse.leftButton.wasPressedThisFrame)
                {
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
    private void CancelPreview()
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
        
        switch (preview.data.buildingType)
        {
            case BuildingType.QueenChamber:
                building.gameObject.GetComponentInChildren<QueenChamberFunction>().OnConstructionFinished();
                queenChambersCount++;
                break;
            case BuildingType.BroodChamber:
                building.gameObject.GetComponentInChildren<BroodChamberFunction>().OnConstructionFinished();
                broodChambersCount++;
                break;
            case BuildingType.StorageChamber:
                building.gameObject.GetComponentInChildren<StorageChamberFunction>().OnConstructionFinished();
                storageChambersCount++;
                break;
            default:
                break;
        }
        constructionsBuilt.Add(building);
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

    private BuildingPreview CreatePreview(BuildingData data, Vector3 position)
    {
        BuildingPreview buildingPreview = Instantiate(previewPrefab, position, Quaternion.identity);
        buildingPreview.Setup(data);
        return buildingPreview;
    }
}