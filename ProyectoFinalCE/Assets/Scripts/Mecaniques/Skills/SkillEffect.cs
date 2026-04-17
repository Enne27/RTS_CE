using System;
[System.Serializable] //Se aplica cuando no heredan de MonoBehaviour ni ScriptableObject. para guardarlo en escenas, prefabs etc
// Representa un efecte individual d'una skill
public class SkillEffect
{
    public EffectType effectType;
    // Stats
    public StatType statType;
    public float value;
    // Efectos especiales
    public string specialID;
}

public enum EffectType
{
    StatModifier,
    UnlockMechanic,
    PercentageModifier,
    Special
}