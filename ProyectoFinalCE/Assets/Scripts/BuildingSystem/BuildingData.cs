using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "BuildingData")]
public class BuildingData : ScriptableObject
{
    [field: SerializeField] public LocalizedString Description {  get; set; }
    [field: SerializeField] public int cost { get; set; }
    [field: SerializeField] public BuildingModel model { get; private set; }
}
