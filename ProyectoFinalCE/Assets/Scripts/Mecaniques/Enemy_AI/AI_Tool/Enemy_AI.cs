using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;


public enum Evoluciones
{
    Brote,
    Nido,
    Colonia,
    Imperio,
}
public enum VisionStatus
{
    Discovered,
    OutOfVision,
    Undiscovered,
}

public enum CellZoneType
{
    Empty,
    ResourcesFood,
    ResourcesConstruction,
    AntHill_IA1,
    AntHill_IA2,
    PuestoVigilancia,
}
public class Enemy_AI
{
    public String name;
    Evoluciones evoluciones;
    public Context context;

    public Anthill anthill;
    public Enemy_AI(String _name)
    {
        name = _name;
        context = new Context();
        evoluciones = Evoluciones.Brote;
        anthill = ScriptableObject.CreateInstance<Anthill>();
    }

    float startX = 300f;
    float startY = 50f;
    float xSpacing = 200f;
    float ySpacing = 120f;
    int index = 0;
    private Rect rect
    {
        get
        {
            int gridSize = 5;

            int col = index % gridSize;
            int row = index / gridSize;

            float x = startX + col * xSpacing;
            float y = startY + row * ySpacing;

            return new Rect(x, y, 150, 80);
        }
    }

    public void Process(Action onStructureCreated)
    {
        ConstructionAction(onStructureCreated);
    }


    private void ConstructionAction(Action onStructureCreated)
    {
        float bestScore = float.MinValue;
        Structure bestChoiceS = null;
        Type bestChoiceT = null;
        foreach (var structure in anthill.GetStructuresToUpgrade())
        {
            var cost = structure.Costs[structure.level];
            var benefit = structure.Benefit[structure.level];

            float score = (float)benefit / cost;
            if (score > bestScore)
            {
                bestScore = score;
                bestChoiceS = structure;
                bestChoiceT = null;
            }

        }

        foreach (var type in TypeCache.GetTypesDerivedFrom<Structure>())
        {
            if (type.IsAbstract) continue;

            var instance = ScriptableObject.CreateInstance(type) as Structure;

            if (anthill.structureCounts[type] >= instance.MaxAmount)
                continue;

            var cost = instance.Costs[0];
            var benefit = instance.Benefit[0];

            float score = (float)benefit / cost;
            if (score > bestScore)
            {
                bestScore = score;
                bestChoiceT = type;
                bestChoiceS = null;
            }
        }

        if (bestChoiceS != null)
        {
            if (bestChoiceS.Costs[bestChoiceS.level] > anthill.resources.eggs) return;
            bestChoiceS.UpgradeStructure();
            anthill.resources.eggs -= bestChoiceS.Costs[bestChoiceS.level];
            Debug.Log($"Upgrade choice: {bestChoiceS} with score {bestScore}");
        }
        else if (bestChoiceT != null)
        {
            if (((int[])bestChoiceT.GetField("costs", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null))[0] > anthill.resources.eggs) return;

            Structure structure = anthill.CreateStructure(bestChoiceT, rect);
            onStructureCreated?.Invoke();
            anthill.structureCounts[bestChoiceT]++;
            anthill.resources.eggs -= structure.Costs[structure.level];
            index++;
            Debug.Log($"Build choice: {bestChoiceT} with score {bestScore}");
        }
    }


    public bool HasStructure<T>() where T : Structure
    {
        return anthill.structures.Any(s => s is T);
    }
    public int HasStructureAmount<T>() where T : Structure
    {
        int amount = 0;
        anthill.structures.ForEach(s =>
        {
            if (s is T)
            {
                amount++;
            }
        });
        return amount;
    }
}

public class Context
{
    public Vector2 pos_AntHill;
    public Vector2 pos_EnemyAntHill;

    public List<ConstructionResource> constructionResources;
    public List<FoodResource> foodResources;
    public List<PuestoVigilancia> puestosVigilancia;

    public List<VisionStatus> mapAwarness;
}
public class ConstructionResource
{
    Vector2 pos;
}

public class FoodResource
{
    Vector2 pos;
}
public class PuestoVigilancia
{
    Vector2 pos;
    Enemy_AI owner;
}

public class MapCell
{
    public Vector2Int index;
    public CellZoneType cellZoneType;

    public MapCell(Vector2Int _index)
    {
        index = _index;
        cellZoneType = CellZoneType.Empty;
    }
    public MapCell(Vector2Int _index, CellZoneType _cellZoneType)
    {
        index = _index;
        cellZoneType = _cellZoneType;
    }
}


