using UnityEngine;
using UnityEngine.Localization;

public abstract class Building
{
    public abstract LocalizedString BuildingName { get; set;}
    public abstract LocalizedString BuildingDescription { get; set;}
    public abstract int buildPrice { get; set;}
    public abstract int buildTime { get; set;}
    public abstract int buildQuantity { get; set;}
}
