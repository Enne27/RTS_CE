using System.Collections.Generic;
using UnityEngine;

public class WorldUIManager : MonoBehaviour
{
    #region VARIABLES
    public static WorldUIManager Instance;

    [SerializeField] private TimerUI timerPrefab;
    [SerializeField] private TimerUI timerAntsPrefab;

    private List<TimerUI> poolUI = new();
    private List<TimerUI> poolUIAnts = new();

    #endregion

    private void Awake()
    {
        Instance = this;
    }

    private TimerUI GetTimerAnts()
    {
        foreach (var t in poolUIAnts)
        {
            if (!t.gameObject.activeSelf)
                return t;
        }

        var newTimer = Instantiate(timerAntsPrefab);
        poolUIAnts.Add(newTimer);
        return newTimer;
    }

    private TimerUI GetTimer()
    {
        foreach (var t in poolUI)
        {
            if (!t.gameObject.activeSelf)
                return t;
        }

        var newTimer = Instantiate(timerPrefab);
        poolUI.Add(newTimer);
        return newTimer;
    }

    public void ShowTimer(Vector3 position, float duration)
    {
        var timer = GetTimer();

        timer.transform.position = new Vector3(position.x, position.y, 5);
        timer.StartTimer(duration);
    }

    public void ShowTimerAnts(Vector3 position, float duration)
    {
        var timer = GetTimerAnts();

        timer.transform.position = new Vector3(position.x, position.y, 5);
        timer.StartTimer(duration);
    }

}
