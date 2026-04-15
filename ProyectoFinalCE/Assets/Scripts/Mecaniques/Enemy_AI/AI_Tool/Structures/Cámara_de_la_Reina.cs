using UnityEngine;
public class Cámara_de_la_Reina : Structure
{
    private static readonly int levels = 5;
    private static readonly int[] costs = { 0, 100, 200, 300, 400 };
    private static readonly double[] timeCosts = { 3, 5, 6, 7, 9 };
    private static readonly int[] benefit = {1, 2, 3, 4, 5 };
    private static readonly int maxAmount = 1;

    public override int Levels => levels;
    public override int[] Costs => costs;
    public override double[] TimeCosts => timeCosts;
    public override int[] Benefit => benefit;
    public override int MaxAmount => maxAmount;


    public override void OnConstructionFinished()
    {
    }

    public override void update()
    {
        if (anthill.resources.eggs + benefit[level-1] > anthill.resources.eggCapacity)
        {
            anthill.resources.eggs = anthill.resources.eggCapacity;
            return;
        }
        anthill.resources.eggs += benefit[level-1];
    }
}



