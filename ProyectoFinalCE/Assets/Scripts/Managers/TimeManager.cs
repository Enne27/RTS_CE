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

    private Dictionary<float, TimerGroup> timers = new Dictionary<float, TimerGroup>();

    private void Update()
    {
        float dt = Time.deltaTime;

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