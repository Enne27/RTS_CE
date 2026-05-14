using UnityEngine;

public class StorageChamberFunction : StructuresPlayer
{
    #region VARIABLES
    [Header("Building parameters")]
    [Tooltip("Tiempo que tarda en producir comida (en segundos).")]
    [SerializeField] float timeToProduceFood = 60f;

    [Tooltip("Scriptable construction info")]
    [SerializeField] BuildingData storageBuildingScriptable;


    [Header("Characteristics by level")]
    [Tooltip("Costes en huevas de las mejoras de cada nivel.")]
    int[] costsUpgradeHV_ = { 50, 50, 70, 90, 110, 170 };

    [Tooltip("Costes en materiales de construcción de las mejoras de cada nivel.")]
    int[] costsUpgradeMC_ = { 5, 20, 30, 35, 55, 60 };

    [Tooltip("Tiempo que tarda el edificio en mejorarse en cada nivel.")]
    int[] timeUpgrade_ = { 30, 60, 75, 80, 90, 120 };

    [Tooltip("Cantidad de comida que produce por cada burst de producción.")]
    int[] quantityProduction = { 10, 20, 30, 50, 75, 100 }; 
    
    [Tooltip("Cantidad de comida que puede almacenar.")]
    int[] quantityStorage = { 100, 100, 100, 150, 200, 250 };

    [Tooltip("Nivel máximo que puede alcanzar la construcción por cada era.")]
    int[] maxLevelByEra_ = { 1, 3, 5, 6 };
    
    [Tooltip("Cantidad máxima que se puede construir por cada era.")]
    int[] maxQuantityByEra_ = { 1, 2, 3, 5 };

    // FALTAN LOS LÍMITES

    [Header("ParentClass variables")]
    public override int[] costsUpgradeHV => costsUpgradeHV_;
    public override int[] costsUpgradeMC => costsUpgradeMC_;
    public override int[] timeUpgrade => timeUpgrade_;
    public override int[] maxLevelByEra => maxLevelByEra_;

    [Header("Visual player")]
    GameHUDView hudView;
    #endregion

    public override void OnConstructionFinished()
    {
        GetComponentInChildren<Renderer>().material = BuildingManager.Instance.StorageChamberMaterial;
        TimeManager.Instance.Register(timeToProduceFood, ProduceFood);
        UpdateCapacityLimits();        
        currentStructureState = StructureState.Idle;
        workerWhoBuildThis.HasFinishedWork();
        workerWhoBuildThis = null;
    }
    public override void OnUpgradeFinished()
    {
        base.OnUpgradeFinished();
        UpdateCapacityLimits();
    }

    /* private void Start()  // OnEnable realmente, pero a veces decide ejecutar en otro orden. PRUEBASSS
     {
         TimeManager.Instance.Register(timeToProduceFood, ProduceFood);
         UpdateCapacityLimits();
     }*/

    private void Awake()
    {
        hudView = FindFirstObjectByType<GameHUDView>();
    }

    private void OnDisable()
    {
        TimeManager.Instance.Unregister(timeToProduceFood, ProduceFood);
    }

    private void ProduceFood()
    {
        FoodAcquired(quantityProduction[currentLevel - 1]);
    }

    /// <summary>
    /// Cuando la comida llega de la cámara de forrajeo a las de almacenamiento.
    /// También se llama mediante la generación que da la "granja de hongos".
    /// </summary>
    void FoodAcquired(int foodToAdd)
    {
        int currentFood = GameManager.instance.player.inventory.food;
        int currentFoodCapacity = GameManager.instance.player.inventory.foodCapacity;

        if (currentFood + foodToAdd < currentFoodCapacity)
            GameManager.instance.player.inventory.AddFood(foodToAdd);
        else
        {
            GameManager.instance.player.inventory.food = currentFoodCapacity;
        }

        if(hudView != null) hudView.UpdateFoodText();
    }

    /// <summary>
    /// Cuando llegan los materiales de la cámara de forrajeo. (Se obtiene la cantidad de las exploraciones.)
    /// </summary>
    /// <param name="mcToAdd"></param>
    void MC_Acquired(int mcToAdd)
    {
        int currentMC = GameManager.instance.player.inventory.materials;
        int currentMC_Capacity = GameManager.instance.player.inventory.materialsCapacity;

        if (currentMC + mcToAdd < currentMC_Capacity)
            GameManager.instance.player.inventory.AddMC(mcToAdd);
        else
        {
            GameManager.instance.player.inventory.materials = currentMC_Capacity;
        }

        if (hudView != null) hudView.UpdateMCText();
    }

    /// <summary>
    /// Método interno para actualizar los valores límite de los almacenes cuando estos se actualizan o construyen.
    /// </summary>
    private void UpdateCapacityLimits()
    {
        GameManager.instance.player.inventory.UpdateFoodCapacity(quantityStorage[currentLevel-1]);
        GameManager.instance.player.inventory.UpdateMC_Capacity(quantityStorage[currentLevel-1]);
    }
}
