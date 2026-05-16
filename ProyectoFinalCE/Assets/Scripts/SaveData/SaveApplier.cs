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

        // Asegurar que el número de obreras coincida con el inventario guardado
        EnsureWorkerCount(player);

        foreach (var worker in player.workers)
            worker.RefreshReferences();

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

        // 1. Recorrer las estructuras actuales del jugador (las que hay en la escena)
        //    y eliminar de la lista SOLO las que no son mounds (sin destruirlas todavía)
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (GameObject obj in player.structures)
        {
            if (obj == null) continue;
            Building building = obj.GetComponent<Building>();
            if (building != null && building.data.buildingType == BuildingType.Mound)
            {
                Debug.Log($"Preserving mound for {player.playerName} at {obj.transform.position}");
                // No hacemos nada, lo conservamos
            }
            else
            {
                toDestroy.Add(obj);
            }
        }

        // Eliminar de la lista y destruir los que no son mounds
        foreach (GameObject obj in toDestroy)
        {
            player.structures.Remove(obj);
            UnityEngine.Object.Destroy(obj);
        }

        // 2. Reconstruir las estructuras guardadas (excluyendo mounds)
        foreach (StructureSaveData data in structuresData)
        {
            if (data.type == "Mound" || data.type == "MoundData")
                continue;

            Building building = GameFactory.Instance.CreateBuilding(data.type, data.position, data.rotation);
            if (building == null) continue;

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

        //Reconstruir la lista de workers a partir de los ants cargados
        player.workers.Clear();
        foreach (Ant ant in player.ants)
        {
            if (ant is AntWorker worker)
            {
                AntWorkerBehaviour behaviour = worker.GetComponent<AntWorkerBehaviour>();
                if (behaviour != null)
                    player.workers.Add(behaviour);
                else
                    Debug.LogWarning($"Worker ant at {worker.transform.position} has no AntWorkerBehaviour component.");
            }
        }
        foreach (var workerBehaviour in player.workers)
        {
            if (workerBehaviour != null)
                workerBehaviour.RefreshReferences();
        }
        foreach (var workerBehaviour in player.workers)
        {
            workerBehaviour?.RefreshReferences();
        }
    }

    public static void EnsureWorkerCount(Player player)
    {
        int expected = player.inventory.workerAnts;
        int current = player.workers.Count;

        if (current >= expected) return;

        int missing = expected - current;
        Debug.Log($"Creating {missing} missing worker ants for player {player.playerName}");

        // Obtener puntos de spawn desde AntCreation (se definen en el inspector)
        AntCreation antCreation = AntCreation.Instance;
        if (antCreation == null)
        {
            Debug.LogError("AntCreation.Instance not found. Cannot spawn workers.");
            return;
        }

        // Usar cualquier punto de spawn disponible (el primero de la lista o un fallback)
        Transform spawnPoint = null;
        if (antCreation.workersSpawnPoint != null && antCreation.workersSpawnPoint.Count > 0)
            spawnPoint = antCreation.workersSpawnPoint[0];
        else if (antCreation.antsSpawnPoint != null)
            spawnPoint = antCreation.antsSpawnPoint;

        if (spawnPoint == null)
        {
            Debug.LogError("No spawn point available for workers.");
            return;
        }

        // Crear obreras sin incrementar de nuevo el contador (addsQuantity = false)
        antCreation.SystemAntCreation(missing, ANT_TYPES.WORKER, spawnPoint, isPlayer: true, addsQuantity: false);
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