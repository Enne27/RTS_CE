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

        foreach (var b in BuildingManagerInstance.Instance.constructionsBuilt)
        {
            list.Add(new StructureSaveData
            {
                type = b.name,
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
            list.Add(new AntSaveData
            {
                type = ant.GetType().Name,
                position = ant.transform.position,
                hp = 100f // ideal: getter real
            });
        }

        return list;
    }

    // Placeholder stats/skills
    public static StatsSaveData GetStats()
    {
        return new StatsSaveData
        {
            stats = new List<StatEntry>()
        };
    }

    public static SkillsSaveData GetSkills()
    {
        return new SkillsSaveData
        {
            unlockedSkills = new List<string>()
        };
    }
}