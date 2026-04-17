using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gestiona totes les estadístiques del jugador
/// </summary>
public class StatManager : MonoBehaviour
{
    #region Singleton
    public static StatManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    #region Variables
    private Dictionary<StatType, float> stats = new Dictionary<StatType, float>();
    #endregion

    private void Start()
    {
        InitializeStats();
    }

    #region Methods
    /// <summary>
    /// Inicialitza totes les stats a 0
    /// </summary>
    private void InitializeStats()
    {
        foreach (StatType type in System.Enum.GetValues(typeof(StatType)))
        {
            stats[type] = 0f;
        }
    }

    /// <summary>
    /// Modify a stat value
    /// </summary>
    public void ModifyStat(StatType type, float value)
    {
        stats[type] += value;
        Debug.Log($"Stat {type} modified by {value}. Total: {stats[type]}");
    }

    public float GetStat(StatType type)
    {
        return stats[type];
    }
    #endregion
}