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
        if(state == WarState.Neutral)
        {

        }
        else if(state == WarState.Tense)
        {

        }
        else if(state == WarState.War)
        {

        }
    }
}
