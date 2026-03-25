using UnityEngine;
using static PlayerConstants;

public class Inventory : MonoBehaviour
{
    #region Variables
    public int eggs;
    public int food;
    [Tooltip("Maximum egg capacity for each era")]
    public int eggCapacity;
    [Tooltip("Maximum Food capacity for each era")]
    public int foodCapacity;
    public int upgradePoints;
    #endregion
        
    #region Methods
    public int SetEggCapacity(HIVE_ERAS playerEra)
    {
        return eggCapacity = EGG_CAPACITIES[playerEra];
    }

    public int AddEggs(int eggsToAdd)
    {
        return eggs + eggsToAdd;
    }
    public int RemoveEggs(int eggsToRemove)
    {
        return eggs - eggsToRemove;
    }

    public int AddFood(int foodToRemove)
    {
        return food - foodToRemove;
    }
    public int RemoveFood(int foodToRemove)
    {
        return food - foodToRemove;
    }

    public int AddUpgradePoints(int upgradePointsToAdd)
    {
        return upgradePoints - upgradePointsToAdd;
    }
    public int RemoveUpgradePoints(int upgradePointsToRemove)
    {
        return upgradePoints - upgradePointsToRemove;
    }
    #endregion
}
