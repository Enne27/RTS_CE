using System;
using UnityEngine;
using static PlayerConstants;

[Serializable]
public class AntSaveData
{
    public ANT_TYPES type;
    public Vector3 position;
    public float hp;
    public Owner owner;
    public float armor;
    public float speed;
    public float strength;
    public int reach;
    public int vision;
    public int linePriority;
    public int[] breedingCost;
    public bool acidBased;
    public int food;
    public int MC;
    public Vector3 assignedResourceZonePosition;
}