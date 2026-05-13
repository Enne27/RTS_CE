using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillUIItem : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public Button unlockButton;
    public TextMeshProUGUI statusText;

    private SkillData skill;

    private void OnEnable()
    {
        if (SkillManager.Instance != null)
            SkillManager.Instance.OnSkillsChanged += Refresh;
    }

    private void OnDisable()
    {
        if (SkillManager.Instance != null)
            SkillManager.Instance.OnSkillsChanged -= Refresh;
    }

    public void Setup(SkillData data)
    {
        skill = data;

        nameText.text = data.SkillName;
        descText.text = data.Description;

        unlockButton.onClick.RemoveAllListeners();
        unlockButton.onClick.AddListener(OnClick);
        Refresh();

        Debug.Log("Setup: " + data.SkillName);
    }
    private void OnClick()
    {
        if (skill == null) return;
        Debug.Log("BUTTON CLICKED: " + skill.SkillName);

        SkillManager.Instance.UnlockSkill(skill);
    }
    public void Refresh()
    {
        if (skill == null || SkillManager.Instance == null) return;
        bool unlocked = SkillManager.Instance.IsSkillUnlocked(skill);

        statusText.text = unlocked ? "UNLOCKED" : "LOCKED";
        unlockButton.interactable = !unlocked;
    }
}