using TMPro;
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
    [SerializeField] private int foods;
    [SerializeField] private int materials;

    [Header("UI References")]
    [SerializeField] Slider capacitySlider; 
    [SerializeField] TextMeshProUGUI capacityText; 

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

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        slots = slotsUpgrade_[currentLevel - 1];
        UpdateUI();
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

    public bool RemoveResource(ResourceType resource)
    {
        switch (resource)
        {
            case ResourceType.food:
                if (foods <= 0) return false;
                foods--;
                break;

            case ResourceType.material:
                if (materials <= 0) return false;
                materials--;
                break;
        }

        slotsOccupied--;
        UpdateUI();
        return true;
    }

    public void MoveResourceToStorageBuild(ResourceType resource)
    {

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
    }

}
