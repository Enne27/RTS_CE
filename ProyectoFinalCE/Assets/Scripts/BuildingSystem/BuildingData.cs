using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "BuildingData")]
public class BuildingData : ScriptableObject
{
    [field: SerializeField] public LocalizedString Description {  get; set; }
    [field: SerializeField] public int cost { get; set; }
    [field: SerializeField] public BuildingModel model { get; private set; }

    [field: SerializeField] public float constructionTime;

    [field: SerializeField] public int maxQuantity;

    [Tooltip("Coste de huevas para construir.")]
    [field: SerializeField] public int costHV;

    [Tooltip("Coste de materiales de construcción para construir.")]
    [field: SerializeField] public int costMC;
}
