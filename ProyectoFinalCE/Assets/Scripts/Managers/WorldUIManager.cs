using System.Collections.Generic;
using UnityEngine;

public class WorldUIManager : MonoBehaviour
{
    #region VARIABLES
    public static WorldUIManager Instance;

    [SerializeField] private TimerUI timerPrefab;

    private List<TimerUI> poolUI = new();

    #endregion

    private void Awake()
    {
        Instance = this;
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

        timer.transform.position = position;
        timer.StartTimer(duration);
    }
}
