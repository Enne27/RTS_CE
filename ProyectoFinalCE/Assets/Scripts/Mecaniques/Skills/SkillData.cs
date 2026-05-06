using UnityEngine;

/// <summary>
/// ScriptableObject que defineix una skill de l'arbre
/// Conté dades, prerequisits i efectes
/// </summary>
[CreateAssetMenu(menuName = "Skills/SkillData")]
public class SkillData : ScriptableObject
{
    #region Variables
    [Header("Info")]
    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string description;

    [Header("Config")]
    [SerializeField] private SkillType skillType;
    [SerializeField] private int cost;

    [Header("Progression")]
    [SerializeField] private SkillData[] prerequisites;

    [Header("Effects")]
    [SerializeField] private SkillEffect[] effects;
    #endregion

    #region Getters
    // Nom de la skill
    public string SkillName => skillName;

    // Descripció de la skill
    public string Description => description;

    // Tipus de skill (branca)
    public SkillType SkillType => skillType;

    // Cost per desbloquejar
    public int Cost => cost;

    // Skills necessàries abans de desbloquejar aquesta
    public SkillData[] Prerequisites => prerequisites;

    // Efectes que aplica la skill
    public SkillEffect[] Effects => effects;
    #endregion
}