using UnityEngine;

public class SkillUIBuilder : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private SkillUIItem skillItemPrefab;

    private void OnEnable()
    {
        if (SkillManager.Instance != null)
            SkillManager.Instance.OnSkillsChanged += RefreshAll;
    }

    private void OnDisable()
    {
        if (SkillManager.Instance != null)
            SkillManager.Instance.OnSkillsChanged -= RefreshAll;
    }

    private void Start()
    {
        BuildUI();
    }

    public void BuildUI()
    {
        var skills = SkillManager.Instance.GetAllSkills();

        if (skills == null || skills.Count == 0)
        {
            Debug.LogWarning("No skills found in SkillManager!");
            return;
        }

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var skill in skills)
        {
            var item = Instantiate(skillItemPrefab, contentParent);
            item.Setup(skill);
        }

        Debug.Log("Skill UI built: " + skills.Count);
    }

    public void RefreshAll()
    {
        foreach (Transform child in contentParent)
        {
            if (child.TryGetComponent(out SkillUIItem item))
            {
                item.Refresh();
            }
        }
    }
}