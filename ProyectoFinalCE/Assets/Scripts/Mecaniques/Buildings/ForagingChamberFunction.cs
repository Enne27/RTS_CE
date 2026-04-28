using UnityEngine;

public class ForagingChamberFunction : StructuresPlayer
{
    [Header("Characteristics by level")]
    [Tooltip("Costes en huevas de las mejoras de cada nivel.")]
    int[] costsUpgradeHV_ = { 0, 10, 20, 40, 60 };

    [Tooltip("Costes en materiales de construcción de las mejoras de cada nivel.")]
    int[] costsUpgradeMC_ = { 0, 15, 25, 30, 45 };

    [Tooltip("Tiempo que tarda el edificio en mejorarse en cada nivel.")]
    int[] timeUpgrade_ = { 0, 60, 90, 90, 120 };

    // El límite de huevas está en playerConstants
    [Tooltip("Nivel máximo que puede alcanzar la construcción por cada era.")]
    int[] maxLevelByEra_ = { 1, 2, 4, 5 };

    [Header("ParentClass variables")]
    public override int[] costsUpgradeHV => costsUpgradeHV_;

    public override int[] costsUpgradeMC => costsUpgradeMC_;

    public override int[] timeUpgrade => timeUpgrade_;

    public override int[] maxLevelByEra => maxLevelByEra_;

    public override void OnConstructionFinished()
    {
        return;
    }
}
