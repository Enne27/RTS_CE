using UnityEngine;

public class Camara_de_Almacenamiento : Structure
{
    private static readonly int levels = 5;
    private static readonly int[] costs = { 0, 100, 200, 300, 400 };
    private static readonly double[] timeCosts = { 3, 5, 6, 7, 9 };
    private static readonly int[] benefit = {100, 200, 300, 400, 500 };
    private static readonly int maxAmount = 1;

    public override int Levels => levels;
    public override int[] Costs => costs;
    public override double[] TimeCosts => timeCosts;
    public override int[] Benefit => benefit;
    public override int MaxAmount => maxAmount;

    public override void OnConstructionFinished()
    {
        if(level == 0)
            anthill.resources.CMaterialsCapacity += benefit[level];
        
        anthill.resources.CMaterialsCapacity += -benefit[level-1] + benefit[level];
    }

    public override void update()
    {
    }
}



