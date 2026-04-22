#if UNITY_EDITOR
using UnityEngine;

public abstract class Structure : ScriptableObject
{
    public enum state
    {
        OnConstruction,
        OnUpdate,
        Idle,
    }
    [HideInInspector]public string guid;
    [HideInInspector]public Vector2 position;
    //[HideInInspector]public AntHillResources antHillResources;
    [HideInInspector]public Anthill anthill;

    [HideInInspector] public abstract int Levels { get; }
    [HideInInspector] public abstract int[] Costs { get; }
    [HideInInspector] public abstract double[] TimeCosts { get; }
    [HideInInspector] public abstract int[] Benefit { get; }
    [HideInInspector] public abstract int MaxAmount { get; }




    public state structureState = state.OnConstruction;
    public int level;
    //public int cost;
    public double remainingUpgradeTime = 0;
    public double remainingUpgradeTime_100;
    public abstract void update();
    public void UpgradeStructure()
    {
        structureState = state.OnConstruction;
        remainingUpgradeTime = TimeCosts[level];
    }
    public abstract void OnConstructionFinished();
    public void OnConstruction()
    {
        remainingUpgradeTime -= AI_Tool.deltaTime;
        remainingUpgradeTime_100 = remainingUpgradeTime / TimeCosts[level] * 100;
        if (remainingUpgradeTime <= 0)
        {
            remainingUpgradeTime = 0;
            level++;
            structureState = state.OnUpdate;
            OnConstructionFinished();
        }
    }
}
#endif


