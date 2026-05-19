using UnityEngine;

public class SkillUIBuilder : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private SkillUIItem skillItemPrefab;

    [Header("Manual Layout")]
    [SerializeField] private float spacing = 125f;

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

        // Limpiar hijos
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Crear items manualmente
        for (int i = 0; i < skills.Count; i++)
        {
            var item = Instantiate(skillItemPrefab, contentParent);

            RectTransform rt = item.GetComponent<RectTransform>();

            if (i > 11)
            {
                if (i == 12)
                {
                    rt.anchoredPosition = new Vector2(580, (-i * spacing) + (spacing * 12) - 125);
                }
                else { 
                    rt.anchoredPosition = new Vector2(580, (-i * spacing) + (spacing * 12) - 165);
                }
            }
            else if (i > 5)
            {
                rt.anchoredPosition = new Vector2(390, ((-i * spacing) + (spacing * 6))/1.7f - 125/2);
            }
            else if (i > 2){
                rt.anchoredPosition = new Vector2(200, (-i * spacing) + (spacing * 3) - 125);
            }
            else  
            {
                rt.anchoredPosition = new Vector2(0, (-i * spacing) - 125);
            }

            item.Setup(skills[i]);
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