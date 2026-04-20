using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WarManager : MonoBehaviour
{
    public WarState state = WarState.Neutral;
    public enum WarState
    {
        Neutral,
        Tense,
        War
    }

    private void Update()
    {
        switch (state)
        {
            case WarState.Neutral:
                // logica
                break;

            case WarState.Tense:
                // logica
                break;

            case WarState.War:
                // logica
                break;
        }
    }
}
