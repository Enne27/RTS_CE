using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private class TimerGroup
    {
        public float currentTime;
        public List<Action> callbacks = new List<Action>();
    }

    private class OneShotTimerData
    {
        public float remainingTime;
        public Action callback;
    }

    private List<OneShotTimerData> oneShots = new List<OneShotTimerData>();
    private Dictionary<float, TimerGroup> timers = new Dictionary<float, TimerGroup>();

    private void Update()
    {
        float dt = Time.deltaTime;

        //Repeated Timers
        foreach (var kvp in timers)
        {
            float interval = kvp.Key;
            TimerGroup group = kvp.Value;

            group.currentTime += dt;

            if (group.currentTime >= interval)
            {
                group.currentTime -= interval;

                for (int i = 0; i < group.callbacks.Count; i++)
                {
                    group.callbacks[i]?.Invoke();
                }
            }
        }

        //One shot timers
        for (int i = oneShots.Count - 1; i >= 0; i--)
        {
            oneShots[i].remainingTime -= dt;

            if (oneShots[i].remainingTime <= 0f)
            {
                oneShots[i].callback?.Invoke();
                oneShots.RemoveAt(i);
            }
        }
    }

    public void OneShotTimer(float time, Action callback)
    {
        oneShots.Add(new OneShotTimerData
        {
            remainingTime = time,
            callback = callback
        });
    }

    public void Register(float interval, Action callback)
    {
        if (!timers.TryGetValue(interval, out TimerGroup group))
        {
            group = new TimerGroup();
            timers.Add(interval, group);
        }

        group.callbacks.Add(callback);
    }

    public void Unregister(float interval, Action callback)
    {
        if (!timers.TryGetValue(interval, out TimerGroup group))
            return;

        group.callbacks.Remove(callback);

        if (group.callbacks.Count == 0)
        {
            timers.Remove(interval);
        }
    }
}