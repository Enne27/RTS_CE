using UnityEngine;
using System;
using System.Collections.Generic;

// Manager principal del sistema d'habilitats
public class SkillManager : MonoBehaviour
{
    #region Singleton
    public static SkillManager Instance { get; private set; }
    #endregion

    #region Variables
    [Header("Skills Database")]
    [SerializeField] private List<SkillData> allSkills;
    private Skills playerSkills;

    private readonly Dictionary<StatType, float> flatStatModifiers = new();
    private readonly Dictionary<StatType, float> percentageStatModifiers = new();
    
    private float specialDamageBonus = 0f;
    #endregion

    #region Events
    public static event Action<SkillData> OnSkillUnlocked;
    public event Action OnSkillsChanged;
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        playerSkills = new Skills();
        playerSkills.Initialize(allSkills);
        //Debug.Log("SkillManager READY");
    }

    #region Methods

    public bool IsSkillUnlocked(SkillData skill)
    {
        return playerSkills != null && playerSkills.IsUnlocked(skill);
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
        if (playerSkills == null) return false;
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
        if (skill == null)
        {
            Debug.LogWarning("Skill is NULL");
            return;
        }

        if (!CanUnlock(skill))
        {
            Debug.Log("Cannot unlock skill: " + skill.SkillName);
            return;
        }
        playerSkills.UnlockSkill(skill);
        ApplyEffects(skill);
        OnSkillUnlocked?.Invoke(skill);
        OnSkillsChanged?.Invoke();
        Debug.Log("Unlocked skill: " + skill.SkillName);
    }

    public void ForceUnlockSkill(SkillData skill)
    {
        if (skill == null || playerSkills == null) return;
        if (!playerSkills.IsUnlocked(skill))
        {
            playerSkills.UnlockSkill(skill);
            ApplyEffects(skill);
            OnSkillsChanged?.Invoke();
        }
    }

    #endregion
    /// <summary>
    /// Aplica tots els afectas de la skill
    /// </summary>
    #region Effects
    private void ApplyEffects(SkillData skill)
    {
        foreach (var effect in skill.Effects)
        {
            switch (effect.effectType)
            {
                case EffectType.StatModifier:
                    AccumulateFlatModifier(effect);
                    ApplyModifiersToAllAnts();
                    break;

                case EffectType.UnlockMechanic:
                    GameManager.instance.UnlockMechanic(effect.specialID);
                    break;

                case EffectType.PercentageModifier:
                    AccumulatePercentageModifier(effect);
                    ApplyModifiersToAllAnts();
                    break;

                case EffectType.Special:
                    ApplySpecialEffect(effect.specialID, effect.value);
                    break;
            }
        }
    }

    public void ApplyModifiersToAnt(Ant ant)
    {
        if (ant == null)
            return;

        ant.ResetToBaseStats();
        ApplyFlatModifiersToAnt(ant);
        ApplyPercentageModifiersToAnt(ant);
    }

    private void ApplyModifiersToAllAnts()
    {
        foreach (Ant ant in FindObjectsOfType<Ant>())
        {
            ApplyModifiersToAnt(ant);
        }
    }

    private void ApplyFlatModifiersToAnt(Ant ant)
    {
        foreach (var modifier in flatStatModifiers)
        {
            ApplyStatModifier(ant, modifier.Key, modifier.Value, false);
        }
    }

    private void ApplyPercentageModifiersToAnt(Ant ant)
    {
        foreach (var modifier in percentageStatModifiers)
        {
            ApplyStatModifier(ant, modifier.Key, modifier.Value, true);
        }
    }

    private void AccumulateFlatModifier(SkillEffect effect)
    {
        if (!flatStatModifiers.ContainsKey(effect.statType))
            flatStatModifiers[effect.statType] = 0f;

        flatStatModifiers[effect.statType] += effect.value;
        StatManager.Instance?.ModifyStat(effect.statType, effect.value);
    }

    private void AccumulatePercentageModifier(SkillEffect effect)
    {
        float normalizedValue = NormalizePercentageValue(effect.value);

        if (!percentageStatModifiers.ContainsKey(effect.statType))
            percentageStatModifiers[effect.statType] = 0f;

        percentageStatModifiers[effect.statType] += normalizedValue;
        StatManager.Instance?.ModifyStat(effect.statType, normalizedValue);
    }

    private float NormalizePercentageValue(float value)
    {
        return Mathf.Abs(value) > 1f ? value / 100f : value;
    }

    private void ApplyStatModifier(Ant ant, StatType statType, float value, bool isPercentage)
    {
        if (ant == null)
            return;

        float modifier = isPercentage ? 1f + value : value;

        switch (statType)
        {
            case StatType.Damage:
            case StatType.Strength:
                ant.strength = isPercentage ? ant.strength * modifier : ant.strength + modifier;
                break;
            case StatType.HP:
                ant.HP = isPercentage ? ant.HP * modifier : ant.HP + modifier;
                break;
            case StatType.Armor:
                ant.armor = isPercentage ? ant.armor * modifier : ant.armor + modifier;
                break;
            case StatType.Speed:
                ant.speed = isPercentage ? ant.speed * modifier : ant.speed + modifier;
                break;
            case StatType.Vision:
                ant.vision = isPercentage ? Mathf.RoundToInt(ant.vision * modifier) : ant.vision + Mathf.RoundToInt(modifier);
                break;
            case StatType.Reach:
                ant.reach = isPercentage ? Mathf.RoundToInt(ant.reach * modifier) : ant.reach + Mathf.RoundToInt(modifier);
                break;
            case StatType.SpecialAbility:
                break;
        }
    }

    private void ApplySpecialEffect(string id, float value)
    {
        switch (id)
        {
            case "InvisibleExplorers":
                GameManager.instance.explorersInvisible = true;
                break;

            case "WorkerBonusPer10":
                GameManager.instance.workerBonusPer10 += value;
                break;

            case "+50% Damage":
            case "DamageBonus50":
                specialDamageBonus += 0.5f;
                ApplyModifiersToAllAnts();
                break;
        }
    }

    /// <summary>
    /// Obtiene el bonificador de daño especial actual
    /// </summary>
    public float GetSpecialDamageBonus()
    {
        return specialDamageBonus;
    }

    /// <summary>
    /// Obtiene el bonificador de daño por cantidad de hormigas (cada 10 hormigas = bonus)
    /// </summary>
    public float GetDamageByAntQuantity()
    {
        int totalAnts = GameManager.instance.player.ants.Count;
        return (totalAnts / 10) * GameManager.instance.workerBonusPer10;
    }

    /// <summary>
    /// Obtiene el bonificador total de daño (especial + por cantidad)
    /// </summary>
    public float GetTotalDamageBonus()
    {
        return specialDamageBonus + GetDamageByAntQuantity();
    }
    #endregion
}