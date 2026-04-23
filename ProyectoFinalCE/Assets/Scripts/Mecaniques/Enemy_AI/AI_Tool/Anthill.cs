#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Anthill : ScriptableObject
{
    public Dictionary<Type, int> structureCounts = new Dictionary<Type, int>();
    public List<Structure> structures = new List<Structure>();
    public AntHillResources resources = new AntHillResources();

    private void OnEnable()
    {
        structureCounts.Clear();

        foreach (var type in TypeCache.GetTypesDerivedFrom<Structure>())
        {
            if (type.IsAbstract)
                continue;

            structureCounts[type] = 0;
        }
    }
    public Structure CreateStructure(System.Type type)
    {
        Structure structure = ScriptableObject.CreateInstance(type) as Structure;
        structure.name = type.Name;
        structure.guid = GUID.Generate().ToString();
        structure.anthill = this;
        structures.Add(structure);
        return structure;
    }
    public Structure CreateStructure(System.Type type, Rect pos)
    {
        Structure structure = ScriptableObject.CreateInstance(type) as Structure;
        structure.name = type.Name;
        structure.guid = GUID.Generate().ToString();
        structure.anthill = this;
        structures.Add(structure);
        structure.position.x = pos.xMin;
        structure.position.y = pos.yMin;
        return structure;
    }

    public void DeleteStrucutre(Structure structure)
    {
        structures.Remove(structure);
    }

    public List<Structure> GetStructuresToUpgrade()
    {
        return structures
            .Where(s => s.level < s.Levels && s.structureState != Structure.state.OnConstruction)
            .ToList();
    }
}
#endif