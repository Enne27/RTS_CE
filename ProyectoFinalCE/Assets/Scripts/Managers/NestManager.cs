using System;
using System.Collections.Generic;
using UnityEngine;

public enum NestSlotType
{
    Empty,
    Occupied
}

[Serializable]
public class NestSlot
{
    public NestSlotType slotType;
    public Vector2Int gridPosition;
}

public class NestManager : MonoBehaviour
{
    [SerializeField] private int NestSize = 20;
    [SerializeField] private List<NestSlot> slots = new List<NestSlot>();

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        slots.Clear();

        int half = NestSize / 2;

        int min = -half;
        int max = half;

        // Si es par, quitamos una columna para evitar contar el 0 como centro
        if (NestSize % 2 == 0)
        {
            max -= 1;
        }

        for (int x = min; x <= max; x++)
        {
            for (int y = min; y <= max; y++)
            {
                NestSlot newSlot = new NestSlot
                {
                    slotType = NestSlotType.Empty,
                    gridPosition = new Vector2Int(x, y)
                };

                slots.Add(newSlot);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Vector3 origin = transform.position;

        int half = NestSize / 2;

        int min = -half;
        int max = half;

        if (NestSize % 2 == 0)
        {
            max -= 1;
        }

        // Líneas verticales
        for (int x = min; x <= max + 1; x++)
        {
            Vector3 start = origin + new Vector3(x, min, 0);
            Vector3 end = origin + new Vector3(x, max + 1, 0);
            Gizmos.DrawLine(start, end);
        }

        // Líneas horizontales
        for (int y = min; y <= max + 1; y++)
        {
            Vector3 start = origin + new Vector3(min, y, 0);
            Vector3 end = origin + new Vector3(max + 1, y, 0);
            Gizmos.DrawLine(start, end);
        }

        // Dibujar slots ocupados
        foreach (var slot in slots)
        {
            if (slot.slotType == NestSlotType.Occupied)
            {
                Vector3 pos = origin + new Vector3(
                    slot.gridPosition.x + 0.5f,
                    slot.gridPosition.y + 0.5f,
                    0
                );

                Gizmos.color = Color.red;
                Gizmos.DrawCube(pos, Vector3.one * 0.9f);
                Gizmos.color = Color.white;
            }
        }
    }
}