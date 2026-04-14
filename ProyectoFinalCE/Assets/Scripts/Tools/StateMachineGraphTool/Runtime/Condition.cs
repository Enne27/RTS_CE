using UnityEngine;

/// <summary>
/// Condiciones que puede evaluar la IA.
/// </summary>
public enum Conditions
{
   //TODO: Make Conditions
}

/// <summary>
/// Contiene todos los datos relevantes para evaluar las condiciones.
/// </summary>
[System.Serializable]
public class Context
{
    //TODO: Make context booleans for conditions
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
            default:
                Debug.LogError($"Unhandled Condition: {condition}");
                return false;
        }
    }

    //TODO: Make functions for checking conditions booleans
    
}
