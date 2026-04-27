using UnityEngine;

public static class GameFactory
{
    public static Ant CreateAnt(string type, Vector3 pos)
    {
        GameObject prefab = Resources.Load<GameObject>("Ants/" + type);
        return Object.Instantiate(prefab, pos, Quaternion.identity).GetComponent<Ant>();
    }

    public static Building CreateBuilding(string type, Vector3 pos)
    {
        GameObject prefab = Resources.Load<GameObject>("Buildings/" + type);
        return Object.Instantiate(prefab, pos, Quaternion.identity).GetComponent<Building>();
    }
}