using UnityEngine;

/// <summary>
/// Condiciones que puede evaluar la IA.
/// </summary>
public enum Conditions
{
    isInCombatRange,
    whatEnemyTypeIs,
    wantsToChangeMode,
    hasBeenInAttackMode,
    hasBeenHit
}

public enum EnemyType
{
    Attacker,
    Defenser,
    Crazy
}

/// <summary>
/// Contiene todos los datos relevantes para evaluar las condiciones.
/// </summary>
[System.Serializable]
public class Context
{
    //enemy mode variables
    public bool isInRange = false;
    public bool AttackMode = false;
    public bool DefenseMode = false;

    //Enemy type variables
    public bool isAttacker;
    public bool isDefenser;

    //context data variables
    public bool wantsToChangeMode;
    
    public bool hasBeenInAttackMode;

    public bool hasBeenHit;

    public Context(EnemyType type) 
    {
        switch (type)
        {
            case EnemyType.Attacker:
                isAttacker = true;
                break;
            case EnemyType.Defenser:
                isDefenser = true;
                break;
            case EnemyType.Crazy:
                isAttacker = true;
                isDefenser = true;
                break;
        }
    }
}

/// <summary>
/// La clase Condition evalúa las condiciones de IA.
/// </summary>
public class Condition
{
    public Context Context;

    public Condition(Context context)
    {
        Context = context;
    }

    public bool GetConditionValue(Conditions condition)
    {
        switch (condition)
        {
            case Conditions.isInCombatRange:
                return IsInCombatRange();
            case Conditions.whatEnemyTypeIs:
                return WhatEnemyTypeIs();
            case Conditions.hasBeenInAttackMode:
                return HasBeenInAttackMode();
            case Conditions.wantsToChangeMode:
                return WantsToChangeMode();
            case Conditions.hasBeenHit:
                return HasBeenHit();
            default:
                Debug.LogError($"Unhandled Condition: {condition}");
                return false;
        }
    }

    private bool IsInCombatRange() => Context.isInRange;

    private bool WhatEnemyTypeIs()
    {
        if(Context.isAttacker == true && Context.isDefenser == false)
        {
            Debug.Log("Attacker");
            Context.AttackMode = true;
            return true;
        }
        else if(Context.isDefenser == true && Context.isAttacker == false)
        {
            Debug.Log("Defenser");
            Context.DefenseMode = true;
            return false;
        }
        else if(Context.isAttacker == true && Context.isDefenser == true)
        {
            Debug.Log("Crazy");
            if (Random.Range(0, 2) == 1)
            {
                Context.AttackMode = true;
                return true;
            }
            else
            { 
                Context.DefenseMode = true;
                return false;
            }
        }
        else return false;
    }

    private bool HasBeenInAttackMode() => Context.hasBeenInAttackMode;

    private bool WantsToChangeMode() 
    { 
        if (Context.wantsToChangeMode)
        {
            Context.wantsToChangeMode = false;
            if(Context.AttackMode == true)
            {
                Context.hasBeenInAttackMode = true;
                Context.AttackMode = false;
                Context.DefenseMode = true;
            }
            else if(Context.DefenseMode == true)
            {
                Context.hasBeenInAttackMode = false;
                Context.AttackMode = true;
                Context.DefenseMode = false;
            }
            return true;
        }
        else
            return false;

    }

    private bool HasBeenHit() => Context.hasBeenHit;
}
