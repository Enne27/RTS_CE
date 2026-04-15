[System.Serializable]
public class AntHillResources
{
    public int food;
    public int foodCapacity;
    public int eggs;
    public int eggCapacity;
    public int CMaterials;
    public int CMaterialsCapacity;
    public int upgradePoints;

    public AntHillResources()
    {
        food = 0;
        foodCapacity = 0;
        eggs = 0;
        eggCapacity = 30;
        CMaterials = 0;
        CMaterialsCapacity = 0;
        upgradePoints = 0;
    }

}