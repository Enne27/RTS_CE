using System;
using System.Collections.Generic;
using UnityEngine;

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

    public const Dictionary<HIVE_ERAS, int> EGG_CAPACITIES = new()
    {
        { HIVE_ERAS.BROTE , 100 },
        { HIVE_ERAS.NIDO , 100 },
        { HIVE_ERAS.COLONIA , 100 },
        { HIVE_ERAS.DOMINIO , 100 },
        { HIVE_ERAS.IMPERIO , 100 }
    };
}
