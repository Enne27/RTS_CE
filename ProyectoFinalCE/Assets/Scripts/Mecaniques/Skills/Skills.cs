using System.Collections.Generic;

/// <summary>
/// Gestiona totes les skills del jugador (progrés)
/// </summary>
public class Skills
{
    #region Variables
    private Dictionary<SkillData, SkillNode> skillNodes = new Dictionary<SkillData, SkillNode>();
    #endregion

    #region Methods
    /// <summary>
    /// Inicialitza totes les skills com a bloquejades
    /// </summary>
    public void Initialize(List<SkillData> allSkills)
    {
        foreach (var skill in allSkills)
        {
            if (!skillNodes.ContainsKey(skill))
                skillNodes.Add(skill, new SkillNode(skill));
        }
    }

    /// <summary>
    /// Comprova si una skill està desbloquejada
    /// </summary>
    public bool IsUnlocked(SkillData skill)
    {
        return skillNodes.ContainsKey(skill) && skillNodes[skill].isUnlocked;
    }
    /// <summary>
    /// Marca una skill com desbloquejada
    /// </summary>
    public void UnlockSkill(SkillData skill)
    {
        if (skillNodes.ContainsKey(skill))
            skillNodes[skill].isUnlocked = true;
    }
    #endregion
}