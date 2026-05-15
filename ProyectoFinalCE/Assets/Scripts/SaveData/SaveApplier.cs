using System;
using System.Collections.Generic;
using UnityEngine;
using static PlayerConstants;
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

    // Aplica los datos a la IA
    public static void ApplyPlayerIA(PlayerSaveData data)
    {
        Debug.Log($"SaveApplier.ApplyPlayerIA() start: id={data.id}, playerName={data.playerName}");
        var playerIA = GameManager.instance.playerIA;

        playerIA.id = data.id;
        playerIA.playerName = data.playerName;
        playerIA.currentEra = data.currentEra;
        playerIA.playerColor = new Color(data.color[0], data.color[1], data.color[2], data.color[3]);

        ApplyInventory(playerIA.inventory, data.inventory);
        ApplyStructures(playerIA, data.structures);
        ApplyAnts(playerIA, data.ants);
    }

    public static void ApplyStructures(Player player, List<StructureSaveData> structuresData)
    {
        Debug.Log($"ApplyStructures for player {player.playerName}, count={structuresData?.Count ?? 0}");

        // 1. Identificar y preservar los mounds existentes (no se guardan ni destruyen)
        List<GameObject> preservedMounds = new List<GameObject>();
        foreach (GameObject obj in player.structures)
        {
            if (obj == null) continue;
            Building building = obj.GetComponent<Building>();
            if (building != null && building.data.buildingType == BuildingType.Mound)
            {
                preservedMounds.Add(obj);
                Debug.Log($"Preserving mound for player {player.playerName} at {obj.transform.position}");
            }
            else
            {
                // Destruir estructuras que no son mounds
                GameObject.Destroy(obj);
            }
        }

        // Limpiar la lista y volver a añadir solo los mounds conservados
        player.structures.Clear();
        foreach (GameObject mound in preservedMounds)
        {
            player.structures.Add(mound);
        }

        // 2. Reconstruir el resto de estructuras desde el save (excluyendo posibles mounds que pudieran venir en el save)
        foreach (StructureSaveData data in structuresData)
        {
            // Saltar por si acaso el save contiene algún mound (por compatibilidad)
            if (data.type == "Mound" || data.type == "MoundData")
                continue;

            Building building = GameFactory.Instance.CreateBuilding(data.type, data.position, data.rotation);
            if (building == null) continue;

            // Escala especial para Mound (aunque ya no debería llegar aquí)
            if (data.type == "Mound" || data.type == "MoundData")
                building.transform.localScale = Vector3.one * 15f;

            StructuresPlayer sp = building.GetComponent<StructuresPlayer>();
            if (sp != null)
            {
                sp.currentLevel = data.level;
                if (Enum.TryParse(data.state, out StructureState state))
                    sp.currentStructureState = state;
            }

            player.structures.Add(building.gameObject);
        }
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
        Debug.Log($"ApplyAnts for player {player.playerName}, count={antsData?.Count ?? 0}");

        foreach (Ant ant in player.ants)
            if (ant != null) UnityEngine.Object.Destroy(ant.gameObject);
        player.ants.Clear();

        foreach (AntSaveData antData in antsData)
        {
            Ant ant = GameFactory.Instance.CreateAnt(antData.type, antData.position);
            if (ant == null) continue;

            // Añadir FogRevealer si no es obrera
            if (antData.type != ANT_TYPES.WORKER)
            {
                FogRevealer fr = ant.gameObject.AddComponent<FogRevealer>();
                fr.visionRadius = antData.vision;
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

            if (ant is AntExlporer explorer)
            {
                explorer.SetFood(antData.food);
                explorer.SetMC(antData.MC);
            }

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