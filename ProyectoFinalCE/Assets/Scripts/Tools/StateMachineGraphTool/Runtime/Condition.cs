using UnityEngine;

/// <summary>
/// Condiciones que puede evaluar la IA.
/// </summary>
public enum BattleCondition
{
    //Health Conditions
    LostHealth,
    LowHealth,
    Healed,

    //Magic Conditions
    LowMagic,
    NoMagic,
    RegainedMagic,

    //Status Effects Conditions
    HasPositiveStatus,
    HasNegativeStatus,
    StatusExpired,

    //Partner
    PartnerHasLostHealth,
    PartnerLowHealth,
    PartnerLowMagic,
    PartnerNoMagic,
    PartnerHasPositiveStatus,
    PartnerHasNegativeStatus,
    PartnerDown,

    //Target Conditions
    TargetLostHealth,
    TargetLowHealth,
    TargetHasPositiveStatus,
    TargetHasNegativeStatus,
    TargetDefending,
    TargetDown
}

/// <summary>
/// Contiene todos los datos relevantes para evaluar las condiciones.
/// </summary>
[System.Serializable]
public class BattleContext
{
    public BattleContext(int HP, int MP)
    {
        MaxHP = HP;
        MaxMP = MP;
        SetValues();
    }

    public BattleContext(int HP, int MP, BattleContext partner)
    {
        MaxHP = HP;
        MaxMP = MP;
        Partner = partner;
        SetValues();
    }

    void SetValues()
    {
        CurrentHP = MaxHP;
        PreviousHP = MaxHP;
        CurrentMP = MaxMP;
        PreviousMP = MaxMP;

        HasPositiveStatus = false;
        HasNegativeStatus = false;
        StatusExpired = false;
        IsDown = false;

    }

    // Datos propios
    public int MaxHP;
    public int CurrentHP;
    public int PreviousHP;

    public int MaxMP;
    public int CurrentMP;
    public int PreviousMP;

    public bool HasPositiveStatus;
    public bool HasNegativeStatus;

    public bool StatusExpired;
    public bool IsDown;

    // Partner
    public BattleContext Partner;

    // Target
    public BattleContext Target;

    public bool TargetDefending;
    public bool TargetDown;

    public void Hurt(int Damage)
    {
        PreviousHP = CurrentHP;
        CurrentHP -= Damage;
    }

    public void Heal(int Healing)
    {
        PreviousHP = CurrentHP;
        CurrentHP += Healing;
        if(CurrentHP > MaxHP)
            CurrentHP = MaxHP;
    }
}

/// <summary>
/// La clase Condition evalúa las condiciones de IA.
/// </summary>
public class Condition
{
    public BattleContext Context;

    public Condition(BattleContext context)
    {
        Context = context;
    }

    public bool GetConditionValue(BattleCondition condition)
    {
        switch (condition)
        {
            case BattleCondition.LostHealth: 
                return LostHealth();
            case BattleCondition.LowHealth: 
                return LowHealth();
            case BattleCondition.Healed: 
                return Healed();

            case BattleCondition.LowMagic: 
                return LowMagic();
            case BattleCondition.NoMagic: 
                return NoMagic();
            case BattleCondition.RegainedMagic: 
                return RegainedMagic();

            case BattleCondition.HasPositiveStatus: 
                return HasPositiveStatus();
            case BattleCondition.HasNegativeStatus: 
                return HasNegativeStatus();
            case BattleCondition.StatusExpired: 
                return StatusExpired();

            case BattleCondition.PartnerHasLostHealth: 
                return PartnerHasLostHealth();
            case BattleCondition.PartnerLowHealth: 
                return PartnerLowHealth();
            case BattleCondition.PartnerLowMagic: 
                return PartnerLowMagic();
            case BattleCondition.PartnerNoMagic: 
                return PartnerNoMagic();
            case BattleCondition.PartnerHasPositiveStatus: 
                return PartnerHasPositiveStatus();
            case BattleCondition.PartnerHasNegativeStatus: 
                return PartnerHasNegativeStatus();
            case BattleCondition.PartnerDown: 
                return PartnerIsDown();

            case BattleCondition.TargetLostHealth: 
                return TargetLostHealth();
            case BattleCondition.TargetLowHealth: 
                return TargetLowHealth();
            case BattleCondition.TargetHasPositiveStatus: 
                return TargetHasPositiveStatus();
            case BattleCondition.TargetHasNegativeStatus: 
                return TargetHasNegativeStatus();
            case BattleCondition.TargetDefending: 
                return TargetIsDefending();
            case BattleCondition.TargetDown: 
                return TargetIsDown();
            default:
                Debug.LogError($"Unhandled Condition: {condition}");
                return false;
        }
    }
    //SELF

    private bool LostHealth() =>
        Context.CurrentHP < Context.PreviousHP;

    private bool LowHealth() =>
        Context.CurrentHP <= Context.MaxHP * 0.25f;

    private bool Healed() =>
        Context.CurrentHP > Context.PreviousHP;

    private bool LowMagic() =>
        Context.CurrentMP <= Context.MaxMP * 0.25f;

    private bool NoMagic() =>
        Context.CurrentMP <= 0;

    private bool RegainedMagic() =>
        Context.CurrentMP > Context.PreviousMP;

    private bool HasPositiveStatus() =>
        Context.HasPositiveStatus;

    private bool HasNegativeStatus() =>
        Context.HasNegativeStatus;

    private bool StatusExpired() =>
        Context.StatusExpired;

    //PARTNER

    private bool PartnerHasLostHealth() =>
        Context.Partner != null &&
        Context.Partner.CurrentHP < Context.Partner.PreviousHP;

    private bool PartnerLowHealth() =>
        Context.Partner != null &&
        Context.Partner.CurrentHP <= Context.Partner.MaxHP * 0.25f;

    private bool PartnerLowMagic() =>
        Context.Partner != null &&
        Context.Partner.CurrentMP <= Context.Partner.MaxMP * 0.25f;

    private bool PartnerNoMagic() =>
        Context.Partner != null &&
        Context.Partner.CurrentMP <= 0;

    private bool PartnerHasPositiveStatus() =>
        Context.Partner != null &&
        Context.Partner.HasPositiveStatus;

    private bool PartnerHasNegativeStatus() =>
        Context.Partner != null &&
        Context.Partner.HasNegativeStatus;

    private bool PartnerIsDown() =>
        Context.Partner != null &&
        Context.Partner.IsDown;

    //TARGET

    private bool TargetLostHealth() =>
        Context.Target != null &&
        Context.Target.CurrentHP < Context.Target.PreviousHP;

    private bool TargetLowHealth() =>
        Context.Target != null &&
        Context.Target.CurrentHP <= Context.Target.MaxHP * 0.25f;

    private bool TargetHasPositiveStatus() =>
        Context.Target != null &&
        Context.Target.HasPositiveStatus;

    private bool TargetHasNegativeStatus() =>
        Context.Target != null &&
        Context.Target.HasNegativeStatus;

    private bool TargetIsDefending() =>
        Context.Target != null &&
        Context.TargetDefending;

    private bool TargetIsDown() =>
        Context.Target != null &&
        Context.TargetDown;
}
