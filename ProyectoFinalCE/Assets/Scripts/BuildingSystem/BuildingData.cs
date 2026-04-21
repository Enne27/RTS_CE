using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public enum BuildingType
{
    QueenChamber,
    BroodChamber,
    StorageChamber
}

[CreateAssetMenu(menuName = "BuildingData")]
public class BuildingData : ScriptableObject
{
    [field: SerializeField] public BuildingType buildingType;

    [field: SerializeField] public LocalizedString Description {  get; set; }

    [field: SerializeField] public BuildingModel model { get; private set; }

    [Tooltip("Tiempo en segundos que tarda en construirse.")]
    [field: SerializeField] public float constructionTime;


    [Header("Construction Costs")]
    [Tooltip("Coste de huevas para construir.")]
    [field: SerializeField] public int costHV;

    [Tooltip("Coste de materiales de construcción para construir.")]
    [field: SerializeField] public int costMC;


    [Header("Upgrades")]
    [Tooltip("Cantidad máxima que se puede construir por cada era.")]
    [field: SerializeField]public int[] maxQuantityByEra = new int[4];

    [Tooltip("Nivel máximo que puede alcanzar la construcción por cada era.")]
    [field: SerializeField]public int[] maxLevelByEra = new int[4];
}