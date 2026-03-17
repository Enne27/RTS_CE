using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class ScriptableIconRule
{
    public MonoScript scriptableScript; // Reference to the ScriptableObject script
    public Texture2D icon;              // Assigned icon
}


public class ScriptableIconManager : EditorWindow
{
    private const string SETTINGS_PATH = "Assets/Editor/ScriptableIconSettings.asset";
    private ScriptableIconSettings settings;

    //Hardcoded icon path for the settings asset
    private const string SETTINGS_ICON_PATH = "Assets/Editor/Icons/settings_icon.png";

    [MenuItem("Tools/Scriptable Icon Manager")]
    public static void ShowWindow()
    {
        GetWindow<ScriptableIconManager>("Scriptable Icon Manager");
    }

    private void OnEnable()
    {
        settings = AssetDatabase.LoadAssetAtPath<ScriptableIconSettings>(SETTINGS_PATH);

        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<ScriptableIconSettings>();
            AssetDatabase.CreateAsset(settings, SETTINGS_PATH);
            AssetDatabase.SaveAssets();

#if UNITY_2022_2_OR_NEWER
            // 🔧 Apply hardcoded icon when the asset is first created
            Texture2D settingsIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(SETTINGS_ICON_PATH);
            if (settingsIcon != null)
            {
                EditorGUIUtility.SetIconForObject(settings, settingsIcon);
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                Debug.Log($"Assigned hardcoded icon '{settingsIcon.name}' to ScriptableIconSettings.asset");
            }
            else
            {
                Debug.LogWarning($"Could not find settings icon at path: {SETTINGS_ICON_PATH}");
            }
#else
            Debug.LogWarning("⚠️ Custom icons require Unity 2022.2 or newer to be displayed.");
#endif
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Scriptable Icon Manager", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Assign custom icons to your ScriptableObjects.\nThese icons will appear in the Project window.", MessageType.Info);

        EditorGUILayout.Space();

        SerializedObject so = new SerializedObject(settings);
        SerializedProperty rulesProp = so.FindProperty("rules");

        EditorGUILayout.PropertyField(rulesProp, new GUIContent("Icon Rules"), true);
        so.ApplyModifiedProperties();

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Apply icons to ScriptableObjects", GUILayout.Height(30)))
        {
            ApplyIcons();
        }
    }

    private void ApplyIcons()
    {
#if UNITY_2022_2_OR_NEWER
        int totalChanges = 0;
        System.Text.StringBuilder logBuilder = new System.Text.StringBuilder();

        foreach (var rule in settings.rules)
        {
            if (rule.scriptableScript == null || rule.icon == null)
                continue;

            Type type = rule.scriptableScript.GetClass();
            if (type == null || !type.IsSubclassOf(typeof(ScriptableObject)))
                continue;

            var guids = AssetDatabase.FindAssets($"t:{type.Name}");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var obj = AssetDatabase.LoadAssetAtPath(path, type) as ScriptableObject;
                if (obj != null)
                {
                    EditorGUIUtility.SetIconForObject(obj, rule.icon);
                    EditorUtility.SetDirty(obj);

                    totalChanges++;
                    logBuilder.AppendLine($"Applied icon '{rule.icon.name}' to '{obj.name}' ({type.Name})");
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (totalChanges > 0)
        {
            Debug.Log($"Scriptable Icon Manager - {totalChanges} icons applied:\n{logBuilder}");
        }
        else
        {
            Debug.Log("No icons were applied. Check if your rules or icons are correctly assigned.");
        }


#else
        EditorUtility.DisplayDialog(
            "Unity version not supported",
            "SetIconForObject is only officially available from Unity 2022.2 onwards.",
            "OK");
#endif
    }

    [InitializeOnLoadMethod]
    private static void AutoApplyIconsOnLoad()
    {
        const string SETTINGS_PATH = "Assets/Editor/ScriptableIconSettings.asset";

        // Cargar el asset de configuración
        var settings = AssetDatabase.LoadAssetAtPath<ScriptableIconSettings>(SETTINGS_PATH);
        if (settings == null) return; // No existe

        int totalChanges = 0;
        System.Text.StringBuilder logBuilder = new System.Text.StringBuilder();

        // Recorrer todas las reglas
        foreach (var rule in settings.rules)
        {
            if (rule.scriptableScript == null || rule.icon == null)
                continue;

            Type type = rule.scriptableScript.GetClass();
            if (type == null || !type.IsSubclassOf(typeof(ScriptableObject)))
                continue;

            var guids = AssetDatabase.FindAssets($"t:{type.Name}");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var obj = AssetDatabase.LoadAssetAtPath(path, type) as ScriptableObject;
                if (obj != null)
                {
                    EditorGUIUtility.SetIconForObject(obj, rule.icon);
                    EditorUtility.SetDirty(obj);

                    totalChanges++;
                    logBuilder.AppendLine($"Applied icon '{rule.icon.name}' to '{obj.name}' ({type.Name})");
                }
            }
        }

        if (totalChanges > 0)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            //UnityEngine.Debug.Log($"Auto Apply Icons on Load - {totalChanges} icons applied:\n{logBuilder}");
        }
    }
}
