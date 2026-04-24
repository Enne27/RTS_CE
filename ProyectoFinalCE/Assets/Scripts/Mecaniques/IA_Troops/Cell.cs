using UnityEngine;

[System.Serializable]
public class Cell
{
    //public Quaternion rotation;///
    [SerializeField]public Vector3 worldPos;
    [SerializeField]public Vector2 gridIndex;
    [SerializeField]public byte cost;
    [SerializeField]public ushort bestCost;
    [SerializeField]public GridDirection bestDirection;

    public Cell(Vector3 _worldPos, /*Quaternion _rotation,*/Vector2 _gridIndex)
    {
        //rotation = _rotation;///
        worldPos = _worldPos;
        gridIndex = _gridIndex;
        cost = 1;
        bestCost = ushort.MaxValue;
        bestDirection = GridDirection.None;
    }

    public void IncreaseCost(int amnt)
    {
        if (cost == byte.MaxValue) { return; }
        if (amnt + cost >= 255) { cost = byte.MaxValue; }
        else { cost += (byte)amnt; }
    }
}

