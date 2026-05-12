using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class WarManager : MonoBehaviour
{
    private bool antWasDamaged = false;
    public WarState state = WarState.Neutral;
    public enum WarState
    {
        Neutral,
        Tense,
        War
    }

    private void OnEnable()
    {
        Ant.OnAnyAntDamaged += HandleAntDamaged;
    }

    private void OnDisable()
    {
        Ant.OnAnyAntDamaged -= HandleAntDamaged;
    }

    private void HandleAntDamaged(Ant ant)
    {
        antWasDamaged = true;
    }

    private void Update()
    {
        switch (state)
        {
            case WarState.Neutral:
                if (antWasDamaged)
                {
                    antWasDamaged = false;
                    SetState(WarState.War);
                }
                break;

            case WarState.Tense:
                if (antWasDamaged)
                {
                    antWasDamaged = false;
                    SetState(WarState.War);
                }
                break;

            case WarState.War:
                if (antWasDamaged)
                {
                    antWasDamaged = false;
                }
                break;
        }
    }

    private void SetState(WarState newState)
    {
        if (state == newState) return;
    

        state = newState;

        switch (state)
        {
            case WarState.Neutral:
                Neutral();
                break;
            case WarState.Tense:
                TimeManager.Instance.OneShotTimer(120f, Tense);
                break;

            case WarState.War:
                TimeManager.Instance.OneShotTimer(120f, War);
                break;
        }
    }

    private void Neutral()
    {
        if (antWasDamaged)
        {
            state = WarState.War;
            antWasDamaged = false;
            return;
        }
    }
    private void Tense()
    {
        bool toNeutral = true;

        if (antWasDamaged)
        {
            toNeutral = false;
            state = WarState.War;
            antWasDamaged = false;
            return;
        }

        if (toNeutral == true)
        {
            state = WarState.Neutral;
        }
    }

    private void War()
    {
        bool toTense = true;
        if (antWasDamaged)
        {
            toTense = false;
            antWasDamaged = false;
            return;
        } 
        if (toTense == true)
        {
            state = WarState.Tense;
        }
    }
}
