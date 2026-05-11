using UnityEngine;
using static PlayerConstants;

public static class GameFactory
{
    public static Ant CreateAnt(ANT_TYPES type, Vector3 pos)
    {
        string prefabName = type switch
        {
            ANT_TYPES.EXPLORER => "AntExplorer",
            ANT_TYPES.WORKER => "AntWorker",
            ANT_TYPES.SOLDIER => "AntSoldier",
            ANT_TYPES.BERSERKER => "AntBerserker",
            ANT_TYPES.ACID => "AntAcid",
            ANT_TYPES.CRAZY => "AntCrazy",
            ANT_TYPES.KAMIKAZE => "AntKamikaze",
            _ => null
        };

        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError($"No prefab mapping for ant type: {type}");
            return null;
        }

        GameObject prefab = Resources.Load<GameObject>("Ants/" + prefabName);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: Ants/{prefabName}");
            return null;
        }

        return Object.Instantiate(prefab, pos, Quaternion.identity)
            .GetComponent<Ant>();
    }

    public static Building CreateBuilding(string type, Vector3 pos)
    {
        GameObject prefab = Resources.Load<GameObject>("Buildings/" + type);

        if (prefab == null)
        {
            Debug.LogError($"Building prefab not found: {type}");
            return null;
        }

        return Object.Instantiate(prefab, pos, Quaternion.identity)
            .GetComponent<Building>();
    }
}