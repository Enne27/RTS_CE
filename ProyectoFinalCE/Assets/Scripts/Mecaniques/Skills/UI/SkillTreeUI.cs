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

    void Update()
    {
        UpdateStats();
    }

    void UpdateStats()
    {
        statsText.text =
            $"Damage: {StatManager.Instance.GetStat(StatType.Damage)}\n" +
            $"Speed: {StatManager.Instance.GetStat(StatType.Speed)}\n" +
            $"HP: {StatManager.Instance.GetStat(StatType.HP)}\n" +
            $"Armor: {StatManager.Instance.GetStat(StatType.Armor)}\n" +
            $"AcidBased: {StatManager.Instance.GetStat(StatType.AcidBased)}\n" +
            $"Vision: {StatManager.Instance.GetStat(StatType.Vision)}\n" +
            $"Reach: {StatManager.Instance.GetStat(StatType.Reach)}\n" +
            $"Strength: {StatManager.Instance.GetStat(StatType.Strength)}";
    }
}