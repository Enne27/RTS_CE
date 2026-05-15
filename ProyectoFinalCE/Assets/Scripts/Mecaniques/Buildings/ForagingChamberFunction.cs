using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum ResourceType
{
    food,
    material
}

public class ForagingChamberFunction : StructuresPlayer
{

    #region SINGLETON
    public static ForagingChamberFunction Instance;
    #endregion

    [Header("BuildingParameters")]
    [SerializeField] private int slots;
    [SerializeField] private int slotsOccupied;

    [Header("Inventory")]
    [SerializeField] public int foods;
    [SerializeField] public int materials;

    [Header("UI References")]
    [SerializeField] Slider capacitySlider; 
    [SerializeField] TextMeshProUGUI capacityText;
    [SerializeField] TextMeshProUGUI fullAlertText;
    [SerializeField] TextMeshProUGUI foodCountText;
    [SerializeField] TextMeshProUGUI materialsCountText;


    [Header("Characteristics by level")]
    [Tooltip("Costes en huevas de las mejoras de cada nivel.")]
    int[] costsUpgradeHV_ = { 0, 10, 20, 40, 60 };

    [Tooltip("Costes en materiales de construcción de las mejoras de cada nivel.")]
    int[] costsUpgradeMC_ = { 0, 15, 25, 30, 45 };

    [Tooltip("Tiempo que tarda el edificio en mejorarse en cada nivel.")]
    int[] timeUpgrade_ = { 0, 60, 90, 90, 120 };

    int[] slotsUpgrade_ = { 50, 60, 70, 80, 90 };

    [Tooltip("Nivel máximo que puede alcanzar la construcción por cada era.")]
    int[] maxLevelByEra_ = { 1, 2, 4, 5 };

    [Header("VisualResources")]
    [SerializeField] RandomChildrenActivator foodVisual;
    [SerializeField] RandomChildrenActivator MCVisual;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        slots = slotsUpgrade_[currentLevel - 1];
        UpdateUI();
    }

    private void Update()
    {
        if (slotsOccupied <= 0)
            return;

        foreach (AntWorkerBehaviour worker in GameManager.instance.player.workers)
        {
            if (worker == null)
                continue;

            if (worker.storageChamber == null)
                continue;

            if (worker.stateMachineManager.GetCurrentStateName() != "Wander")
                continue;

            StorageChamberFunction storage =
                worker.storageChamber;

            bool canTransportFood =
                foods > 0 &&
                storage.FreeFoodSpace() > 0;

            bool canTransportMaterials =
                materials > 0 &&
                storage.FreeMaterialSpace() > 0;

            // No hay espacio para nada
            if (!canTransportFood && !canTransportMaterials)
                continue;

            worker.CallToTransport();
        }
    }

    public bool AddResource(ResourceType resource, int quantity)
    {
        if (quantity <= 0)
            return false;

        int availableSlots = slots - slotsOccupied;

        if (availableSlots <= 0)
            return false;

        int amountToAdd = Mathf.Min(quantity, availableSlots);

        switch (resource)
        {
            case ResourceType.food:
                foods += amountToAdd;
                break;

            case ResourceType.material:
                materials += amountToAdd;
                break;

            default:
                return false;
        }

        slotsOccupied += amountToAdd;

        UpdateUI();

        return amountToAdd == quantity;
    }

    public bool RemoveResource(ResourceType resource, int quantity)
    {
        // Cantidad inválida
        if (quantity <= 0)
            return false;

        switch (resource)
        {
            case ResourceType.food:

                // No hay suficiente comida
                if (foods < quantity)
                    return false;

                foods -= quantity;
                break;

            case ResourceType.material:

                // No hay suficientes materiales
                if (materials < quantity)
                    return false;

                materials -= quantity;
                break;

            default:
                return false;
        }

        // Evitar negativos
        slotsOccupied = Mathf.Max(0, slotsOccupied - quantity);

        UpdateUI();
        return true;
    }

    public override int[] costsUpgradeHV => costsUpgradeHV_;

    public override int[] costsUpgradeMC => costsUpgradeMC_;

    public override int[] timeUpgrade => timeUpgrade_;

    public override int[] maxLevelByEra => maxLevelByEra_;

    public override void OnConstructionFinished()
    {
        return;
    }

    public void UpdateUI()
    {
        capacitySlider.value = ((float)slotsOccupied / slots) * 100;
        capacityText.text = $"{slotsOccupied}\n-\n{slots}";

        foodCountText.text = $"{foods}";
        materialsCountText.text = $"{materials}";

        bool isFull = slotsOccupied >= slots;
        fullAlertText.enabled = isFull;

        float foodPercent;
        float materialPercent;

        if (isFull)
        {
            foodPercent = 100f;
            materialPercent = 100f;
        }
        else
        {
            foodPercent = (slots == 0) ? 0 : ((float)foods / slots) * 100f;
            materialPercent = (slots == 0) ? 0 : ((float)materials / slots) * 100f;
        }

        if (foodVisual != null)
            foodVisual.SetPercentage(foodPercent);

        if (MCVisual != null)
            MCVisual.SetPercentage(materialPercent);
    }
}
