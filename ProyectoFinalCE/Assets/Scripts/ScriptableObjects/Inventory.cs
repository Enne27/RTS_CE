using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerConstants;

public class Inventory
{
    #region Variables
    protected int eggs;
    protected int food;
    [Tooltip("Maximum egg capacity for each era")]
    public HIVE_ERAS currentEra;
    protected int eggCapacity;
    protected int foodCapacity;
    protected int upgradePoints;
    #endregion

    #region Methods
    public int SetEggCapacity()
    {
        return eggCapacity = EGG_CAPACITIES[currentEra];
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
