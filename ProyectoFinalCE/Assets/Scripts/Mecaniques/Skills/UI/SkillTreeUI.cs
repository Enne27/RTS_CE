using UnityEngine;
using TMPro;

public class SkillTreeUI : MonoBehaviour
{
    [Header("UI")]
    public Transform contentParent;
    public SkillUIItem itemPrefab;
    public TextMeshProUGUI statsText;

    [Header("Data")]
    public SkillData[] allSkills;

    private void Start()
    {
        InvokeRepeating(nameof(UpdateStats), 0f, 0.5f);
        Debug.Log("SkillTreeUI STARTED");

        BuildUI();
    }

void BuildUI()
{
    Debug.Log("Building UI...");

    foreach (var skill in allSkills)
    {
        Debug.Log("Creating: " + skill.SkillName);

        var item = Instantiate(itemPrefab, contentParent);
        item.Setup(skill);
    }
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