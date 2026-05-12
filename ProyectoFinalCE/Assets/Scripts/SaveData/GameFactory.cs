using UnityEngine;
using static PlayerConstants;

public static class GameFactory
{
    public static Ant CreateAnt(ANT_TYPES type, Vector3 pos)
    {
        string prefabName = type switch
        {
            ANT_TYPES.EXPLORER => "ExplorerAnt",
            ANT_TYPES.WORKER => "WorkerAnt",
            ANT_TYPES.SOLDIER => "SoldierAnt",
            ANT_TYPES.BERSERKER => "BerserkerAnt",
            ANT_TYPES.ACID => "AcidAnt",
            ANT_TYPES.CRAZY => "CrazyAnt",
            ANT_TYPES.KAMIKAZE => "KamikazeAnt",
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

        GameObject obj = Object.Instantiate(prefab, pos, Quaternion.identity);

        Ant ant = obj.GetComponent<Ant>();

        ant.antType = type;

        return ant;
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