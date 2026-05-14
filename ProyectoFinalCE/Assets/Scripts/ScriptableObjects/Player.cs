using System;
using System.Collections.Generic;
using UnityEngine;
using static PlayerConstants;

[System.Serializable]
public class Player
{
    public int id;
    public string playerName;
    [Tooltip("The outline color that this player's ants will have")]
    public Color playerColor;
    [Tooltip("An inventory GameObject will be created at runtime")]
    public Inventory inventory;
    [Tooltip("The era at wich the player's hive is at")]
    public HIVE_ERAS currentEra;
    public Dictionary<Type, int> structuresCount;
    public List<GameObject> structures;
    public List<Ant> ants;
    public List<AntWorkerBehaviour> workers;

    public Player()
    {
        inventory = new Inventory();
        structuresCount = new Dictionary<Type, int>();
        structures = new List<GameObject>();
        ants = new List<Ant>();
        
        /* TBI
        foreach (var type in TypeCache.GetTypesDerivedFrom<Structure>())
        {
            if (type.IsAbstract) continue;
            structuresCount[type] = 0;
        }
        */

        currentEra = HIVE_ERAS.BROTE;
    }
}
