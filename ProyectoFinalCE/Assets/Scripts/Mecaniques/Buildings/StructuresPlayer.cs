using UnityEngine;
public enum StructureState
{
    OnConstruction,
    OnUpdate,
    Idle,
}

public abstract class StructuresPlayer : MonoBehaviour
{
    #region VARIABLES

    [HideInInspector] public Vector2 positionAntHill;
    public abstract int[] costsUpgradeHV { get; }
    public abstract int[] costsUpgradeMC { get; }
    public abstract int[] timeUpgrade { get; }
    public abstract int[] maxLevelByEra { get; }

    public int currentCostsUpgradeHV;
    public int currentCostsUpgradeMC;
    public int currentTimeUpgrade;

    public StructureState currentStructureState = StructureState.OnConstruction;
    public int currentLevel = 1;

    public AntWorkerBehaviour workerWhoBuildThis;
    #endregion

    /// <summary>
    /// Mientras se está actualizando la construcción.
    /// </summary>
    public void UpgradeStructure()
    {
        currentStructureState = StructureState.OnUpdate;
        OnUpgradeFinished();
    }

    /// <summary>
    /// Cuando termina de construirse al inicio.
    /// </summary>
    public abstract void OnConstructionFinished();

    /// <summary>
    /// Cuando termina de actualizarse la construcción.
    /// </summary>
    public virtual void OnUpgradeFinished() 
    { 
        currentStructureState = StructureState.Idle;
        currentLevel++;
        currentCostsUpgradeHV = costsUpgradeHV[currentLevel-1];
        currentCostsUpgradeMC = costsUpgradeMC[currentLevel-1];
        currentTimeUpgrade = timeUpgrade[currentLevel-1];
    }
    
}