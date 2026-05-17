using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string Path => Application.persistentDataPath + "/save.json";

    #region SAVE
    public static void SaveGame()
    {
        SaveGameData data = new SaveGameData
        {
            player = SaveConverter.ToSaveData(GameManager.instance.player),
            playerIA = SaveConverter.ToSaveData(GameManager.instance.playerIA),
            stats = SaveConverter.GetStats(),
            skills = SaveConverter.GetSkills()
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Path, json);
        Debug.Log("Game Saved -> " + Path);
    }
    #endregion

    #region LOAD

    public static bool CanLoadGame()
    {
        if (!File.Exists(Path))
        {
            Debug.LogWarning("No save file found");
            return false;
        }
        else
        {
            return true;
        }
    }
    public static void LoadGame()
    {
        Debug.Log("SaveSystem.LoadGame() start. Path: " + Path);

        if (!File.Exists(Path))
        {
            Debug.LogWarning("No save file found");
            return;
        }

        // Marcar antes de cargar para evitar creación inicial de hormigas
        AntCreation.MarkLoaded();

        string json = File.ReadAllText(Path);
        Debug.Log("Save file length: " + json.Length);

        SaveGameData data = JsonUtility.FromJson<SaveGameData>(json);

        if (data == null)
        {
            Debug.LogError("Failed to parse save file");
            return;
        }

        if (data.player == null)
        {
            Debug.LogError("Saved player data is null");
            return;
        }

        Debug.Log($"SaveSystem.LoadGame() data loaded: playerName={data.player.playerName}, era={data.player.currentEra}, ants={data.player.ants?.Count ?? 0}, structures={data.player.structures?.Count ?? 0}");

        SaveApplier.ApplyPlayer(data.player);
        // Forzar que EraManager refresque la UI y los requisitos de nivel
        if (EraManager.instance != null)
        {
            EraManager.instance.ForceRecalculateLevels();
            EraManager.instance.RefreshUI();
        }
        // Cargar IA solo si existe en el fichero (compatibilidad con saves antiguos)
        if (data.playerIA != null)
        {
            SaveApplier.ApplyPlayerIA(data.playerIA);
        }
        else
        {
            Debug.LogWarning("Save file does not contain playerIA data. Initializing default IA.");
            // Opcional: inicializar IA por defecto
            GameManager.instance.playerIA = new Player();
        }

        SaveApplier.ApplyStats(data.stats);
        SaveApplier.ApplySkills(data.skills);

        Debug.Log("Game Loaded");
    }
    #endregion
}