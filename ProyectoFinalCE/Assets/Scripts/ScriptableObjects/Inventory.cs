using UnityEngine;
using static PlayerConstants;

[System.Serializable]
public class Inventory
{
    #region Variables
    public int eggs;
    public int food;
    public int materials;
    [Tooltip("Maximum egg capacity for each era")]
    public int eggCapacity;
    [Tooltip("Maximum Food capacity")]
    public int foodCapacity;
    [Tooltip("Maximum Construction Materials capacity")]
    public int materialsCapacity;
    public int upgradePoints;
    [Tooltip("The nummber of worker ants the player has")]
    public int workerAnts;
    #endregion
        
    public Inventory()
    {
        SetEggCapacity(HIVE_ERAS.BROTE);
        //UpdateFoodCapacity(FOOD_CAPACITY); // En era brote solo hay una cámara de almacenamiento y esta tiene un límite de 100 de capacidad.
        //UpdateMC_Capacity(MC_CAPACITY);
        if(GameManager.instance != null)
            workerAnts = GameManager.instance.startingWorkerAnts;
    }

    #region Methods
    #region Capacities
    public int SetEggCapacity(HIVE_ERAS playerEra)
    {
        return eggCapacity = EGG_CAPACITIES[playerEra];
    }

    public int UpdateFoodCapacity(int calculatedMaxStorage)
    {
        return foodCapacity += calculatedMaxStorage;
    }

    public int UpdateMC_Capacity(int calculatedMaxStorage)
    {
        return materialsCapacity += calculatedMaxStorage;
    }

    #endregion

    public int AddEggs(int eggsToAdd)
    {
        return eggs += eggsToAdd;
    }
    public int RemoveEggs(int eggsToRemove)
    {
        return eggs -= eggsToRemove;
    }

    public int AddFood(int foodToAdd)
    {
        food += foodToAdd;
        if(food > foodCapacity) food = foodCapacity;
        return food;
    }
    public int RemoveFood(int foodToRemove)
    {
        food -= foodToRemove;
        if (food < 0) food = 0;
        return food;
    }
    public int AddMC(int mcToAdd)
    {
        materials += mcToAdd;
        if (materials > materialsCapacity) materials = materialsCapacity;
        return materials;
    }
    public int RemoveMC(int mcToRemove)
    {
        materials -= mcToRemove;
        if (materials < 0) materials = 0;
        return materials;
    }

    public int AddUpgradePoints(int upgradePointsToAdd)
    {
        return upgradePoints += upgradePointsToAdd;
    }
    public int RemoveUpgradePoints(int upgradePointsToRemove)
    {
        return upgradePoints -= upgradePointsToRemove;
    }

    /// <summary>
    /// Devolver todos los valores a su estado inicial.
    /// </summary>
    public void ResetAllVariables()
    {
        eggs = GameManager.instance.startingEggs;
        food = GameManager.instance.startingFood;
        materials = GameManager.instance.startingMC;
        upgradePoints = 0;

        workerAnts = GameManager.instance.startingWorkerAnts;

        eggCapacity = EGG_CAPACITIES[HIVE_ERAS.BROTE];
        foodCapacity = FOOD_CAPACITY;
        materialsCapacity = MC_CAPACITY;
    }
    #endregion
}
