using System.Collections.Generic;
using UnityEngine;

public static class SaveApplier
{
    public static void ApplyPlayer(PlayerSaveData data)
    {
        Debug.Log($"SaveApplier.ApplyPlayer() start: id={data.id}, playerName={data.playerName}, era={data.currentEra}");

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

        ApplyStructures(player, data.structures);

        ApplyAnts(player, data.ants);

        Debug.Log("SaveApplier.ApplyPlayer() finished");
    }

    public static void ApplyStructures(Player player, List<StructureSaveData> structuresData)
    {
        Debug.Log($"SaveApplier.ApplyStructures() start: savedStructureCount={structuresData?.Count ?? 0}");

        foreach (GameObject obj in player.structures)
        {
            if (obj != null)
                GameObject.Destroy(obj);
        }

        player.structures.Clear();

        int createdCount = 0;

        foreach (StructureSaveData data in structuresData)
        {
            Debug.Log($"Creating structure from save: type={data.type}, position={data.position}, rotation={data.rotation}, level={data.level}, state={data.state}");

            Building building = GameFactory.Instance.CreateBuilding(data.type, data.position, data.rotation);

            if (building == null)
            {
                Debug.LogError($"CreateBuilding returned null for type={data.type}");
                continue;
            }

            // *** AÑADE ESTAS LÍNEAS ***
            // Escala especial para Mound al cargar desde save
            if (data.type == "Mound" || data.type == "MoundData")
            {
                building.transform.localScale = Vector3.one * 15f;
                Debug.Log($"SaveApplier.ApplyStructures: Applied scale 15 to Mound at position {data.position}");
            }

            StructuresPlayer structuresPlayer = building.GetComponent<StructuresPlayer>();
            if (structuresPlayer != null)
            {
                structuresPlayer.currentLevel = data.level;
                if (System.Enum.TryParse(data.state, out StructureState state))
                {
                    structuresPlayer.currentStructureState = state;
                }
                else
                {
                    structuresPlayer.currentStructureState = StructureState.Idle;
                }
            }

            player.structures.Add(building.gameObject);
            createdCount++;
        }

        Debug.Log($"SaveApplier.ApplyStructures() created structures: {createdCount}");
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
        Debug.Log($"SaveApplier.ApplyAnts() start: savedAntCount={antsData?.Count ?? 0}");

        foreach (var ant in player.ants)
        {
            if (ant != null)
                GameObject.Destroy(ant.gameObject);
        }

        player.ants.Clear();

        int createdCount = 0;

        foreach (var antData in antsData)
        {
            Debug.Log($"Creating ant from save: type={antData.type}, position={antData.position}, hp={antData.hp}, owner={antData.owner}");

            Ant ant = GameFactory.Instance.CreateAnt(antData.type, antData.position);

            if (ant == null)
            {
                Debug.LogError($"CreateAnt returned null for type={antData.type}");
                continue;
            }

            ant.SetHP(antData.hp);
            ant.antOwner = antData.owner;
            ant.SetArmor(antData.armor);
            ant.SetSpeed(antData.speed);
            ant.SetStrength(antData.strength);
            ant.SetReach(antData.reach);
            ant.SetVision(antData.vision);
            ant.SetLinePriority(antData.linePriority);
            ant.SetBreedingCost(antData.breedingCost);
            ant.SetAcidBased(antData.acidBased);

            AntExlporer explorerAnt = ant as AntExlporer;
            if (explorerAnt != null)
            {
                explorerAnt.SetFood(antData.food);
                explorerAnt.SetMC(antData.MC);
            }

            player.ants.Add(ant);
            createdCount++;
        }

        Debug.Log($"SaveApplier.ApplyAnts() created ants: {createdCount}");
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