using UnityEngine;

public abstract class StructuresPlayer : MonoBehaviour
{
    #region VARIABLES
    public enum StructureState
    {
        OnConstruction,
        OnUpdate,
        Idle,
    }

    [HideInInspector] public Vector2 positionAntHill;
    public abstract int[] costsUpgradeHV { get; }
    public abstract int[] costsUpgradeMC { get; }
    public abstract int[] timeUpgrade { get; }
    public abstract int[] maxLevelByEra { get; }


    public StructureState currentStructureState = StructureState.OnConstruction;
    public int currentLevel = 1;

    #endregion


    public void UpgradeStructure()
    {
        currentStructureState = StructureState.OnUpdate;
        OnUpgradeFinished();
    }
    public abstract void OnConstructionFinished();
    public virtual void OnUpgradeFinished() 
    { 
        currentStructureState = StructureState.Idle;
        currentLevel++;
    }
    
}