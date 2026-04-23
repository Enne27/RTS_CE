using System.Collections.Generic;

public static class PlayerConstants
{
    public enum HIVE_ERAS
    {
        BROTE,
        NIDO,
        COLONIA,
        IMPERIO
    }

    public enum ANT_TYPES
    {
        ACID,
        BERSERKER,
        EXPLORER, 
        SOLDIER,
        CRAZY,
        KAMIKAZE,
        WORKER
    }

    //Fog entity tags
    public enum ENTITY_TYPE
    {
        ENEMY,
        RESOURCE
    }

    public static readonly Dictionary<HIVE_ERAS, int> EGG_CAPACITIES = new() 
    {
        { HIVE_ERAS.BROTE , 500 },
        { HIVE_ERAS.NIDO , 625 },
        { HIVE_ERAS.COLONIA , 750 },
        { HIVE_ERAS.IMPERIO , 1000 }
    };

    public static readonly int FOOD_CAPACITY = 100;
    public static readonly int MC_CAPACITY = 100;
    public static readonly int FORRAJEO_STORAGE_CAPACITY = 5;
}