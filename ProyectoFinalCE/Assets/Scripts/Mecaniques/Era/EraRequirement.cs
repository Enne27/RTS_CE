using UnityEngine;

public enum RequirementType
{
    COUNT,
    LEVEL,
    ACTION
}

public enum RequirementID
{
    // Ants
    ANT,
    ACID_ANT,
    BERSERKER_ANT,
    EXPLORER_ANT,
    KAMIKAZE_ANT,
    SOLDIER_ANT,
    CRAZY_ANT,
    WORKER_ANT,

    // Buildings
    BROOD_CHAMBER,
    QUEEN_CHAMBER,
    STORAGE_CHAMBER,

    // Exploration
    EXPLORATION
}

[System.Serializable]
public class EraRequirement
{
    #region VARIABLES
    public RequirementID id;
    public RequirementType type;
    public int requiredLevel;

    public int currentQuantity;
    public int targetQuantity;

    public bool IsCompleted => currentQuantity >= targetQuantity;
    #endregion

    #region EVENTS
    public event System.Action OnChanged;
    #endregion

    public EraRequirement(RequirementID id, int targetQuantity, RequirementType type, int requiredLevel = 0)
    {
        this.id = id;
        this.targetQuantity = targetQuantity;
        this.type = type;
        this.requiredLevel = requiredLevel;

        currentQuantity = 0;
    }

    public void AddProgress(int amount)
    {
        if (type != RequirementType.COUNT) return;

        if (IsCompleted) return;

        currentQuantity = Mathf.Min(currentQuantity + amount, targetQuantity);
        OnChanged?.Invoke();
    }

    public void SetProgress(int value)
    {
        currentQuantity = Mathf.Min(value, targetQuantity);
        OnChanged?.Invoke();
    }
}