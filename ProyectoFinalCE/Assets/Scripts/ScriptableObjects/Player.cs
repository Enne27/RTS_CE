using System.Collections.Generic;
using UnityEngine;
using static PlayerConstants;

public class Player
{
    #region Variables
    protected int id;
    protected string name;
    //protected string color;
    protected HIVE_ERAS currentEra;
    #endregion

    #region Methods
    public int AddEggs(int playerEggs, int eggsToAdd)
    {
        return playerEggs + eggsToAdd;
    }
    public int RemoveEggs(int playerEggs, int eggsToRemove)
    {
        return playerEggs - eggsToRemove;
    }

    public int AddFood(int playerFood, int foodToRemove)
    {
        return playerFood - foodToRemove;
    }
    public int RemoveFood(int playerFood, int foodToRemove)
    {
        return playerFood - foodToRemove;
    }

    public int AddUpgradePoints(int playerUpgradePoints, int upgradePointsToAdd)
    {
        return playerUpgradePoints - upgradePointsToAdd;
    }
    public int RemoveUpgradePoints(int playerUpgradePoints, int upgradePointsToRemove)
    {
        return playerUpgradePoints - upgradePointsToRemove;
    }
    #endregion
}
