using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSaveData
{
    public int id;
    public string playerName;
    public float[] color;
    public HIVE_ERAS currentEra;

    public InventorySaveData inventory;
    public List<StructureSaveData> structures;
    public List<AntSaveData> ants;
}