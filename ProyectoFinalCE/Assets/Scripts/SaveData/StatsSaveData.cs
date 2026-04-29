using System;
using System.Collections.Generic;

[Serializable]
public class StatsSaveData
{
    public List<StatEntry> stats;
}

[Serializable]
public class StatEntry
{
    public StatType type;
    public float value;
}