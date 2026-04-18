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
    #endregion

    public override void OnConstructionFinished()
    {
        //TimeManager.Instance.Unregister(timeToProduceFood, ProduceFood);
    }

    private void OnEnable()
    {
        TimeManager.Instance.Register(timeToProduceFood, ProduceFood);
    }

    private void OnDisable()
    {
        TimeManager.Instance.Unregister(timeToProduceFood, ProduceFood);
    }

    private void ProduceFood()
    {

    }
}
