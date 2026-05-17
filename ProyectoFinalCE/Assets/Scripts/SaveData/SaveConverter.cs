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

            inventory = ToSaveInventory(player.inventory),

            ants = ToAntSaveData(player.ants),

            structures = ToStructureSaveData(player.structures)
        };
    }

    public static List<StructureSaveData> ToStructureSaveData(List<GameObject> structures)
    {
        List<StructureSaveData> data = new();

        foreach (GameObject obj in structures)
        {
            if (obj == null) continue;

            Building building = obj.GetComponent<Building>();
            if (building == null) continue;

            // NO guardar los mounds (ant hills) porque ya existen en la escena
            if (building.data.buildingType == BuildingType.Mound)
                continue;

            StructuresPlayer structuresPlayer = obj.GetComponent<StructuresPlayer>();

            data.Add(new StructureSaveData
            {
                type = building.data.buildingType.ToString(),
                position = building.transform.position,
                level = structuresPlayer != null ? structuresPlayer.currentLevel : 1,
                state = structuresPlayer != null ? structuresPlayer.currentStructureState.ToString() : "Idle",
                rotation = building.GetComponentInChildren<BuildingModel>() != null ? building.GetComponentInChildren<BuildingModel>().Rotation : 0f
            });
        }

        return data;
    }
    
    public static InventorySaveData ToSaveInventory(Inventory inventory)
    {
        return new InventorySaveData
        {
            eggs = inventory.eggs,
            food = inventory.food,
            materials = inventory.materials,
            upgradePoints = inventory.upgradePoints,
            workerAnts = inventory.workerAnts,

            eggCapacity = inventory.eggCapacity,
            foodCapacity = inventory.foodCapacity,
            materialsCapacity = inventory.materialsCapacity
        };
    }

    public static List<AntSaveData> ToAntSaveData(List<Ant> ants)
    {
        List<AntSaveData> data = new();

        foreach (Ant ant in ants)
        {
            if (ant == null)
                continue;

            if (!ant.gameObject.activeSelf)
                continue;

            AntExlporer explorerAnt = ant as AntExlporer;
            data.Add(new AntSaveData
            {
                type = ant.antType,
                position = ant.transform.position,
                hp = ant.GetCurrentHP(),
                owner = ant.antOwner,
                armor = ant.GetArmor(),
                speed = ant.GetSpeed(),
                strength = ant.GetStrength(),
                reach = ant.GetReach(),
                vision = ant.GetVision(),
                linePriority = ant.GetLinePriority(),
                breedingCost = ant.GetBreedingCost(),
                acidBased = ant.GetAcidBased(),
                food = explorerAnt != null ? explorerAnt.GetFood() : 0,
                MC = explorerAnt != null ? explorerAnt.GetMC() : 0
            });
        }

        return data;
    }

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
                data.unlockedSkills.Add(skill.SkillName.GetLocalizedString());
            }
        }

        return data;
    }
}