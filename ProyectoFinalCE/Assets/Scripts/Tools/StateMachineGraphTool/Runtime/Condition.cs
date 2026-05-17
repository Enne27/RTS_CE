using UnityEngine;

/// <summary>
/// Condiciones que puede evaluar la IA.
/// </summary>
public enum Conditions
{
   //TODO: Make Conditions
   isHP0,
   isAttack,
   isCollect,
   isMove,
   isInRange,

   hasWork,
   workFinished
}

/// <summary>
/// Contiene todos los datos relevantes para evaluar las condiciones.
/// </summary>
[System.Serializable]
public class Context
{
    //TODO: Make context booleans for conditions
    public int HP = 0;
    public bool Range = false;
    public bool isAttack = false;
    public bool isCollect = false;
    public bool isMove = false;

    public bool hasWork = false;
    public bool workFinished = false;
}

/// <summary>
/// La clase Condition eval�a las condiciones de IA.
/// </summary>
public class Condition
{
    public Context Context;

    public Condition(Context context)
    {
        context.HP = 0;
        context.Range = false;
        context.isAttack = false;
        context.isCollect = false;
        context.isMove = false;
        context.hasWork = false;
        context.workFinished = false;
        Context = context;
    }

    public bool GetConditionValue(Conditions condition)
    {
        switch (condition)
        {
            case Conditions.isHP0:
                return IsHP0();
            case Conditions.isInRange:
                return IsInRange();
            case Conditions.isAttack:
                return IsAttack();
            case Conditions.isCollect:
                return IsCollect();
            case Conditions.isMove:
                return IsMove();
            case Conditions.hasWork:
                return HasWork();
            case Conditions.workFinished:
                return WorkFinished();
            default:
                Debug.LogError($"Unhandled Condition: {condition}");
                return false;
        }
    }

    public bool IsHP0()
    {
        if (Context.HP <= 0)
        {
            return true;
        }
        else return false;
    }

    public bool IsInRange()
    {
      if(Context.Range == true)
            return true;
      else return false;
    }

    public bool IsAttack()
    {
        if (Context.isAttack == true)
            return true;
        else return false;
    }

    public bool IsCollect()
    {
        if (Context.isCollect == true)
            return true;
        else return false;
    }

    public bool IsMove()
    {
        if (Context.isMove) 
            return true;    
        else return false;
    }

    public bool HasWork()
    {
        if (Context.hasWork)
            return true;
        else return false;
    }

    public bool WorkFinished()
    {
        if(Context.workFinished)
            return true;
        else return false;
    }
    //TODO: Make functions for checking conditions booleans
}
