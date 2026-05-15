using System.Collections.Generic;
using UnityEngine;

public class RandomChildrenActivator : MonoBehaviour
{
    [Header("Percentage")]
    [Range(0f, 100f)]
    [SerializeField] public float activePercentage = 100f;

    [Header("Debug")]
    [SerializeField] private List<GameObject> children = new();

    private List<GameObject> inactiveChildren = new();
    private List<GameObject> activeChildren = new();

    private float lastPercentage = -1f;

    private void Start()
    {
        CacheChildren();
        InitializeState();
    }

    private void Update()
    {
        if (Mathf.Approximately(lastPercentage, activePercentage))
            return;

        lastPercentage = activePercentage;

        UpdateChildrenState();
    }

    private void CacheChildren()
    {
        children.Clear();

        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }
    }

    private void InitializeState()
    {
        activeChildren.Clear();
        inactiveChildren.Clear();

        foreach (GameObject child in children)
        {
            if (child == null)
                continue;

            child.SetActive(false);

            inactiveChildren.Add(child);
        }

        lastPercentage = -1f;

        UpdateChildrenState();
    }

    private void UpdateChildrenState()
    {
        int targetCount =
            Mathf.RoundToInt(
                (activePercentage / 100f)
                * children.Count);

        // =========================
        // ACTIVAR MÁS
        // =========================

        while (activeChildren.Count < targetCount)
        {
            if (inactiveChildren.Count <= 0)
                break;

            int randomIndex =
                Random.Range(0, inactiveChildren.Count);

            GameObject selected =
                inactiveChildren[randomIndex];

            inactiveChildren.RemoveAt(randomIndex);

            selected.SetActive(true);

            activeChildren.Add(selected);
        }

        // =========================
        // DESACTIVAR
        // =========================

        while (activeChildren.Count > targetCount)
        {
            if (activeChildren.Count <= 0)
                break;

            int randomIndex =
                Random.Range(0, activeChildren.Count);

            GameObject selected =
                activeChildren[randomIndex];

            activeChildren.RemoveAt(randomIndex);

            selected.SetActive(false);

            inactiveChildren.Add(selected);
        }
    }

    public void SetPercentage(float value)
    {
        activePercentage =
            Mathf.Clamp(value, 0f, 100f);
    }
}