using Unity.VectorGraphics;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
            Debug.Log("Refresh");
        }
    }
}
