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

    public void Setup(SkillData data)
    {
        skill = data;

        Debug.Log("Setup: " + data.SkillName);

        unlockButton.onClick.RemoveAllListeners();

        unlockButton.onClick.AddListener(() =>
        {
            Debug.Log("BUTTON CLICKED: " + skill.SkillName);

            SkillManager.Instance.UnlockSkill(skill);
            Refresh();
        });
    }
    public void Refresh()
    {
        bool unlocked = SkillManager.Instance.IsSkillUnlocked(skill);

        statusText.text = unlocked ? "UNLOCKED" : "LOCKED";
        unlockButton.interactable = !unlocked;
    }
}