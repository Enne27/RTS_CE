using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum Evoluciones
{
    Brote,
    Larva,
    Pupa,
    Dominación,
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
    Resources,
    AntHill,
    PuestoVigilancia,
}
public class Enemy_AI
{
    public String name;
    Evoluciones evoluciones;
    public Context context;

    public Enemy_AI(String _name)
    {
        name = _name;
        evoluciones = Evoluciones.Brote;
    }
}

public class Context
{
    public ContextoInteriorHormigeroEnemigo hormigeroEnemigo;
    public ContextoHabilidadesEnemigo habilidadesEnemigo;
    public Vector2 pos_hormigeroEnemigo;
    public List<Vector2> pos_PuestosVigilancia;
    public List<Vector2> pos_ZonasRecursos;
    //public VisionStatus[,] mapAwarness;
    public List<VisionStatus> mapAwarness;
}

public class ContextoInteriorHormigeroEnemigo
{

}

public class ContextoHabilidadesEnemigo
{

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
