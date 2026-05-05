using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Gestiona totes les estadístiques del jugador
/// </summary>
public class StatManager : MonoBehaviour
{
    #region Singleton
    public static StatManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    #endregion

    #region Events
    public event Action OnStatsChanged;
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
        foreach (StatType type in Enum.GetValues(typeof(StatType)))
        {
            stats[type] = 0f;
        }

        OnStatsChanged?.Invoke();
    }

    /// <summary>
    /// Guardar Stats
    /// </summary>
    public void SetStat(StatType type, float value)
    {
        stats[type] = value;
        OnStatsChanged?.Invoke();
    }

    /// <summary>
    /// Modify a stat value
    /// </summary>
    public void ModifyStat(StatType type, float value)
    {
        if (!stats.ContainsKey(type)) stats[type] = 0f;
        stats[type] += value;
        Debug.Log($"Stat {type} modified by {value}. Total: {stats[type]}");
        OnStatsChanged?.Invoke();
    }

    public float GetStat(StatType type)
    {
        if (!stats.ContainsKey(type))
            return 0f;

        return stats[type];
    }
    #endregion
}