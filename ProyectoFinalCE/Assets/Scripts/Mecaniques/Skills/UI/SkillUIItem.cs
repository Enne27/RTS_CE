using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillUIItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public Button unlockButton;
    public TextMeshProUGUI statusText;
    public GameObject skillDetailsPanel;

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

        skill.SkillName.StringChanged -= OnNameChanged;
        skill.Description.StringChanged -= OnDescChanged;
        data.SkillName.StringChanged += OnNameChanged;
        data.Description.StringChanged += OnDescChanged;

        descText.gameObject.SetActive(false);
        skillDetailsPanel.SetActive(false);
        unlockButton.onClick.RemoveAllListeners();
        unlockButton.onClick.AddListener(OnClick);

        Refresh();

        Debug.Log("Setup: " + data.SkillName);
    }

    private void OnNameChanged(string value)
    {
        nameText.text = value;
    }

    private void OnDescChanged(string value)
    {
        descText.text = value;
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
    public void OnPointerEnter(PointerEventData eventData)
    {
        descText.gameObject.SetActive(true);
        skillDetailsPanel.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        descText.gameObject.SetActive(false);
        skillDetailsPanel.SetActive(false);
    }
}