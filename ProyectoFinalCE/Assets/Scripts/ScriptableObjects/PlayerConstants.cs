using System.Collections.Generic;

public static class PlayerConstants
{
    public enum HIVE_ERAS
    {
        BROTE,
        NIDO,
        COLONIA,
        DOMINIO,
        IMPERIO
    }

    public static readonly Dictionary<HIVE_ERAS, int> EGG_CAPACITIES = new() 
    {
        { HIVE_ERAS.BROTE , 500 },
        { HIVE_ERAS.NIDO , 625 },
        { HIVE_ERAS.COLONIA , 750 },
        { HIVE_ERAS.DOMINIO , 875 },
        { HIVE_ERAS.IMPERIO , 1000 }
    };
}