using System.Collections.Generic;
using UnityEngine;

public static class SaveApplier
{
    public static void ApplyPlayer(PlayerSaveData data)
    {
        var player = GameManager.instance.player;

        player.id = data.id;
        player.playerName = data.playerName;
        player.currentEra = data.currentEra;

        player.playerColor = new Color(
            data.color[0],
            data.color[1],
            data.color[2],
            data.color[3]
        );

        ApplyInventory(player.inventory, data.inventory);
        ApplyAnts(player, data.ants);
    }

    public static void ApplyInventory(Inventory inv, InventorySaveData data)
    {
        inv.eggs = data.eggs;
        inv.food = data.food;
        inv.materials = data.materials;
        inv.upgradePoints = data.upgradePoints;
        inv.workerAnts = data.workerAnts;
        inv.eggCapacity = data.eggCapacity;
        inv.foodCapacity = data.foodCapacity;
        inv.materialsCapacity = data.materialsCapacity;
    }

    public static void ApplyAnts(Player player, List<AntSaveData> antsData)
    {
        if (player == null)
        {
            Debug.LogError("Player not ready when loading ants");
            return;
        }

        // Destruir hormigas actuales
        foreach (var ant in player.ants)
        {
            if (ant != null)
                GameObject.Destroy(ant.gameObject);
        }

        player.ants.Clear();

        foreach (var antData in antsData)
        {
            Ant ant = GameFactory.CreateAnt(antData.type, antData.position);

            if (ant == null)
            {
                Debug.LogWarning($"Failed to create ant: {antData.type}");
                continue;
            }

            ant.SetHP(antData.hp);

            player.ants.Add(ant);
        }
    }

    public static void ApplyStats(StatsSaveData data)
    {
        if (StatManager.Instance == null)
        {
            Debug.LogError("StatManager not found");
            return;
        }

        foreach (var entry in data.stats)
        {
            StatManager.Instance.SetStat(entry.type, entry.value);
        }
    }

    public static void ApplySkills(SkillsSaveData data)
    {
        if (SkillManager.Instance == null)
        {
            Debug.LogError("SkillManager not found");
            return;
        }

        foreach (string skillName in data.unlockedSkills)
        {
            SkillData skill = FindSkillByName(skillName);

            if (skill != null)
            {
                if (!SkillManager.Instance.IsSkillUnlocked(skill))
                {
                    SkillManager.Instance.ForceUnlockSkill(skill);
                }
            }
            else
            {
                Debug.LogWarning("Skill not found: " + skillName);
            }
        }
    }

    private static SkillData FindSkillByName(string name)
    {
        foreach (var skill in SkillManager.Instance.GetAllSkills())
        {
            if (skill.SkillName == name)
                return skill;
        }

        return null;
    }
}