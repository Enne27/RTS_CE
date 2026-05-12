using System;
using UnityEngine;
using static PlayerConstants;

[Serializable]
public class AntSaveData
{
    public ANT_TYPES type;
    public Vector3 position;
    public float hp;
}