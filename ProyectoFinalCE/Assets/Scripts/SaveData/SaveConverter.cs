using System.Collections.Generic;
using UnityEngine;

public static class SaveConverter
{
    public static PlayerSaveData ToSaveData(Player player)
    {
        return new PlayerSaveData
        {
            id = player.id,
            playerName = player.playerName,
            currentEra = player.currentEra,

            color = new float[]
            {
                player.playerColor.r,
                player.playerColor.g,
                player.playerColor.b,
                player.playerColor.a
            },

            inventory = ToSaveData(player.inventory),
            structures = GetStructures(),
            ants = GetAnts(player)
        };
    }

    public static InventorySaveData ToSaveData(Inventory inv)
    {
        return new InventorySaveData
        {
            eggs = inv.eggs,
            food = inv.food,
            materials = inv.materials,
            upgradePoints = inv.upgradePoints,
            workerAnts = inv.workerAnts,

            eggCapacity = inv.eggCapacity,
            foodCapacity = inv.foodCapacity,
            materialsCapacity = inv.materialsCapacity
        };
    }

    private static List<StructureSaveData> GetStructures()
    {
        List<StructureSaveData> list = new();

        foreach (var b in BuildingManager.Instance.constructionsBuilt)
        {
            list.Add(new StructureSaveData
            {
                type = b.GetComponent<Building>().buildingID,
                position = b.transform.position,
                level = 1
            });
        }

        return list;
    }

    private static List<AntSaveData> GetAnts(Player player)
    {
        List<AntSaveData> list = new();

        foreach (var ant in player.ants)
        {
            if (ant == null)
                continue;

            list.Add(new AntSaveData
            {
                type = ant.antType,
                position = ant.transform.position,
                hp = ant.GetCurrentHP()
            });
        }

        return list;
    }

    // Placeholder stats/skills
    public static StatsSaveData GetStats()
    {
        var data = new StatsSaveData
        {
            stats = new List<StatEntry>()
        };

        if (StatManager.Instance == null)
        {
            Debug.LogWarning("StatManager not found when saving");
            return data;
        }

        foreach (StatType type in System.Enum.GetValues(typeof(StatType)))
        {
            data.stats.Add(new StatEntry
            {
                type = type,
                value = StatManager.Instance.GetStat(type)
            });
}

        return data;
    }

    public static SkillsSaveData GetSkills()
    {
        var data = new SkillsSaveData
        {
            unlockedSkills = new List<string>()
        };

        if (SkillManager.Instance == null)
        {
            Debug.LogWarning("SkillManager not found when saving");
            return data;
        }

        foreach (var skill in SkillManager.Instance.GetAllSkills())
        {
            if (SkillManager.Instance.IsSkillUnlocked(skill))
            {
                data.unlockedSkills.Add(skill.SkillName);
            }
        }

        return data;
    }
}