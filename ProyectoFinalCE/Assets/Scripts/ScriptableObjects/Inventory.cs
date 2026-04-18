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
        SetFoodCapacity(100); // En era brote solo hay una cámara de almacenamiento y esta tiene un límite de 100 de capacidad.
        SetMaterialsCapacity(100);
    }

    #region Methods
    #region Capacities
    public int SetEggCapacity(HIVE_ERAS playerEra)
    {
        return eggCapacity = EGG_CAPACITIES[playerEra];
    }

    public int SetFoodCapacity(int calculatedMaxStorage)
    {
        return foodCapacity = calculatedMaxStorage;
    }

    public int SetMaterialsCapacity(int calculatedMaxStorage)
    {
        return materialsCapacity = calculatedMaxStorage;
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

    public int AddFood(int foodToRemove)
    {
        return food += foodToRemove;
    }
    public int RemoveFood(int foodToRemove)
    {
        return food -= foodToRemove;
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
        //foodCapacity = EGG_CAPACITIES[HIVE_ERAS.BROTE];
        //materialsCapacity = EGG_CAPACITIES[HIVE_ERAS.BROTE];
    }
    #endregion
}
