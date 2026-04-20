using UnityEngine;
public class Monticulo : Structure
{
    private static readonly int levels = 1;
    private static readonly int[] costs = { 0 };
    private static readonly double[] timeCosts = { 0 };
    private static readonly int[] benefit = { 0 };
    private static readonly int maxAmount = 1;
    private static int amount = 0;


    public override int Levels => levels;
    public override int[] Costs => costs;
    public override double[] TimeCosts => timeCosts;
    public override int[] Benefit => benefit;
    public override int MaxAmount => maxAmount;


    public override void OnConstructionFinished()
    {
        //throw new System.NotImplementedException();
    }

    public override void update()
    {
        //throw new System.NotImplementedException();
    }
}



