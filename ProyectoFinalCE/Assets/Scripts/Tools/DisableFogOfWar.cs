using Unity.VectorGraphics;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class DisableFogOfWar
{
    static DisableFogOfWar()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
        }
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            int a;
            Debug.Log("Refresh");
        }
    }
}
