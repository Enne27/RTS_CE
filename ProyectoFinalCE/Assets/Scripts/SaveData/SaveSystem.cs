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
        if (!File.Exists(Path))
        {
            Debug.LogWarning("No save file found");
            return;
        }

        // Marcar antes de cargar para evitar creación inicial de hormigas
        AntCreation.MarkLoaded();

        string json = File.ReadAllText(Path);

        SaveGameData data = JsonUtility.FromJson<SaveGameData>(json);

        if (data == null)
        {
            Debug.LogError("Failed to parse save file");
            return;
        }

        SaveApplier.ApplyPlayer(data.player);
        SaveApplier.ApplyStats(data.stats);
        SaveApplier.ApplySkills(data.skills);

        Debug.Log("Game Loaded");
    }
    #endregion
}