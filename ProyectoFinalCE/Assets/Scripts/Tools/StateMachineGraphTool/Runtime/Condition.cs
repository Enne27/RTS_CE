using UnityEngine;

/// <summary>
/// Condiciones que puede evaluar la IA.
/// </summary>
public enum Conditions
{
   //TODO: Make Conditions
   isHP0,
}

/// <summary>
/// Contiene todos los datos relevantes para evaluar las condiciones.
/// </summary>
[System.Serializable]
public class Context
{
    //TODO: Make context booleans for conditions
    public int HP = 0;
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
        Context = context;
    }

    public bool GetConditionValue(Conditions condition)
    {
        switch (condition)
        {
            case Conditions.isHP0:
                return IsHP0();
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
        return false;
    }
    //TODO: Make functions for checking conditions booleans
    
}
