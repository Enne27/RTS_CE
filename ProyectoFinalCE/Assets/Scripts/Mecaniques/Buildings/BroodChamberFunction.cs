using UnityEngine;

public class BroodChamberFunction : StructuresPlayer
{
    #region VARIABLES
    [Header("Building parameters")]
    [Tooltip("Scriptable construction info")]
    [SerializeField] BuildingData broodBuildingScriptable;


    [Header("Characteristics by level")]
    [Tooltip("Costes en huevas de las mejoras de cada nivel.")]
    int[] costsUpgradeHV_ = { 50, 50, 70, 90, 110, 170 };

    [Tooltip("Costes en materiales de construcción de las mejoras de cada nivel.")]
    int[] costsUpgradeMC_ = { 5, 20, 30, 35, 55, 60 };

    [Tooltip("Tiempo que tarda el edificio en mejorarse en cada nivel.")]
    int[] timeUpgrade_ = { 30, 60, 60, 70, 90, 120 };

    [Tooltip("Cantidad de hormigas que puede generar por nivel.")]
    int[] broodingCapacity = { 1, 2, 3, 4, 5, 6 };

    [Tooltip("Tiempo que tarda en crear una hormiga base.")]
    int timeGeneratingAnt = 60;

    [Tooltip("Nivel máximo que puede alcanzar la construcción por cada era.")]
    int[] maxLevelByEra_ = { 1, 3, 5, 6 };

    [Tooltip("Cantidad máxima que se puede construir por cada era.")]
    int[] maxQuantityByEra_ = { 1, 3, 4, 6 };


    [Header("ParentClass variables")]
    public override int[] costsUpgradeHV => costsUpgradeHV_;
    public override int[] costsUpgradeMC => costsUpgradeMC_;
    public override int[] timeUpgrade => timeUpgrade_;
    public override int[] maxLevelByEra => maxLevelByEra_;
    #endregion


    public override void OnConstructionFinished()
    {
        
    }
}
