using UnityEngine;
using TMPro;

public class SkillTreeUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI statsText;

    private void OnEnable()
    {
        if (StatManager.Instance != null)
            StatManager.Instance.OnStatsChanged += UpdateStats;
    }

    private void OnDisable()
    {
        if (StatManager.Instance != null)
            StatManager.Instance.OnStatsChanged -= UpdateStats;
    }

    private void Start()
    {
        UpdateStats();
    }

    void UpdateStats()
    {
        statsText.text =
            $"Damage: {StatManager.Instance.GetStat(StatType.Damage)}\n" +
            $"MoveSpeed: {StatManager.Instance.GetStat(StatType.MoveSpeed)}\n" +
            $"BuildSpeed: {StatManager.Instance.GetStat(StatType.BuildSpeed)}\n" +
            $"ResourceGain: {StatManager.Instance.GetStat(StatType.ResourceGain)}";
    }
}