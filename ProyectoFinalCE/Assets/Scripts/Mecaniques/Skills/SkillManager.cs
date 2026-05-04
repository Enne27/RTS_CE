using UnityEngine;
using System;
using System.Collections.Generic;

// Manager principal del sistema d'habilitats
public class SkillManager : MonoBehaviour
{
    #region Singleton
    public static SkillManager Instance;
    private void Awake()
    {
        Instance = this;
        Debug.Log("SkillManager READY");
    }
    #endregion

    #region Variables
    [Header("Skills Database")]
    [SerializeField] private List<SkillData> allSkills;
    private Skills playerSkills;
    #endregion

    #region Events
    public static event Action<SkillData> OnSkillUnlocked;
    #endregion

    private void Start()
    {
        playerSkills = new Skills();
        playerSkills.Initialize(allSkills);
    }

    #region Methods

    public bool IsSkillUnlocked(SkillData skill)
    {
        return playerSkills.IsUnlocked(skill);
    }

    /// <summary>
    /// Recoger todas las skills
    /// </summary>
    public List<SkillData> GetAllSkills()
    {
        return allSkills;
    }

    /// <summary>
    /// Revisa si una skill se puede desbloquear
    /// </summary>
    public bool CanUnlock(SkillData skill)
    {
        if (playerSkills.IsUnlocked(skill))
            return false;

        foreach (var prereq in skill.Prerequisites)
        {
            if (!playerSkills.IsUnlocked(prereq))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Desbloquea una skill
    /// </summary>
    public void UnlockSkill(SkillData skill)
    {
        if (!CanUnlock(skill))
        {
            Debug.Log("Cannot unlock skill: " + skill.SkillName);
            return;
        }
        playerSkills.UnlockSkill(skill);
        ApplyEffects(skill);
        OnSkillUnlocked?.Invoke(skill);
        Debug.Log("Unlocked skill: " + skill.SkillName);
    }

    public void ForceUnlockSkill(SkillData skill)
    {
        if (!playerSkills.IsUnlocked(skill))
        {
            playerSkills.UnlockSkill(skill);
        }
    }

    /// <summary>
    /// Aplica tots els afectas de la skill
    /// </summary>
    private void ApplyEffects(SkillData skill)
    {
        foreach (var effect in skill.Effects)
        {
            switch (effect.effectType)
            {
                case EffectType.StatModifier:
                    StatManager.Instance.ModifyStat(effect.statType, effect.value);
                    break;

                case EffectType.UnlockMechanic:
                    GameManager.instance.UnlockMechanic(effect.specialID);
                    break;

                case EffectType.PercentageModifier:
                    StatManager.Instance.ModifyStat(effect.statType, effect.value);
                    break;

                case EffectType.Special:
                    ApplySpecialEffect(effect.specialID, effect.value);
                    break;
            }
        }
    }

    private void ApplySpecialEffect(string id, float value)
    {
        switch (id)
        {
            case "EggsAsFood":
                GameManager.instance.canUseEggsAsFood = true;
                break;

            case "InvisibleExplorers":
                GameManager.instance.explorersInvisible = true;
                break;

            case "WorkerBonusPer10":
                GameManager.instance.workerBonusPer10 += value; 
                break;

            case "RecoverMaterials":
                GameManager.instance.recoverMaterialsPercent += value;
                break;
        }
    }
    #endregion
}