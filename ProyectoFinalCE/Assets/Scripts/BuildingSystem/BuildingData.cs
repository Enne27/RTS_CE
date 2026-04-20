using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "BuildingData")]
public class BuildingData : ScriptableObject
{
    [field: SerializeField] public LocalizedString Description {  get; set; }

    [Tooltip("Coste de construcción, a nivel grid.")]
    [field: SerializeField] public int cost { get; set; }
    [field: SerializeField] public BuildingModel model { get; private set; }

    [Tooltip("Tiempo en segundos que tarda en construirse.")]
    [field: SerializeField] public float constructionTime;

    [Tooltip("Cantidad máxima que se pueden construir hasta la era final.")]
    [field: SerializeField] public int maxQuantity;

    [Header("Level gestion")]
    [Tooltip("Nivel máximo al que se puede mejorar la construcción.")]
    [field: SerializeField] public int max_lvl;

    [Header("Construction Costs")]
    [Tooltip("Coste de huevas para construir.")]
    [field: SerializeField] public int costHV;

    [Tooltip("Coste de materiales de construcción para construir.")]
    [field: SerializeField] public int costMC;
}