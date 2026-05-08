using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public enum BuildingType
{
    QueenChamber,
    BroodChamber,
    StorageChamber,
    Tunnel,
    Entrance,
    Mound
}

[CreateAssetMenu(menuName = "BuildingData")]
public class BuildingData : ScriptableObject
{
    [Header("General Data")]
    [field: SerializeField] public BuildingType buildingType;
    [field: SerializeField] public LocalizedString buildName {  get; set; }
    [field: SerializeField] public LocalizedString buildDescription {  get; set; }
    [field: SerializeField] public BuildingModel buildModel { get; private set; }

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

    [Header("UI")]
    [Tooltip("Sprite segun cámara")]
    [field: SerializeField] public Sprite previewSprite { get; private set; }
}