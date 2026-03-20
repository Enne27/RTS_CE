using System.Collections.Generic;
using static PlayerConstants;
using UnityEngine;

public class Inventory
{
    protected int eggs;
    protected int food;
    //Key-Value pair is CurrentEra - EggQuantity
    protected Dictionary<HIVE_ERAS, int> eggCapacity;
    protected int foodCapacity;
}
