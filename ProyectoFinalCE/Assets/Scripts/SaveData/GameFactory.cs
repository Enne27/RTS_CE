using UnityEngine;
using static PlayerConstants;

public static class GameFactory
{
    public static Ant CreateAnt(ANT_TYPES type, Vector3 pos)
    {
        string prefabName = type switch
        {
            ANT_TYPES.EXPLORER => "CH_ScoutAnt",
            ANT_TYPES.WORKER => "CH_WorkerAnt",
            ANT_TYPES.SOLDIER => "CH_SoldierAnt",
            ANT_TYPES.BERSERKER => "CH_BerserkerAnt",
            ANT_TYPES.ACID => "CH_AcidThrowerAnt",
            ANT_TYPES.CRAZY => "CH_CrazyAnt",
            ANT_TYPES.KAMIKAZE => "CH_KamikazeAnt",
            _ => null
        };

        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError($"No prefab mapping for ant type: {type}");
            return null;
        }

        GameObject prefab = Resources.Load<GameObject>("../Prefabs/Ants/" + prefabName);

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

    public static Building CreateBuilding(string type, Vector3 pos, float rotation = 0f)
    {
        GameObject prefab = Resources.Load<GameObject>("../Prefabs/Buildings /" + type);

        if (prefab == null)
        {
            Debug.LogError($"Building prefab not found: {type}");
            return null;
        }

        Building building = Object.Instantiate(prefab, pos, Quaternion.identity)
            .GetComponent<Building>();
        
        if (building != null)
        {
            building.Setup(building.data, rotation);
        }

        return building;
    }
}