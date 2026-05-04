#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public static class ScriptableActionCreator
{
    private const string TEMPLATE =
@"using UnityEngine;
using StateMachine.Runtime;

[CreateAssetMenu(fileName = ""#SCRIPTNAME#"", menuName = ""Tools/IA/Actions/#SCRIPTNAME#"")]
public class #SCRIPTNAME# : ScriptableAction
{
    public override void Execute(StateMachineManager manager)
    {
        // TODO: Implement action logic
    }
}";
    [MenuItem("Assets/Create/Tools/IA/New Scriptable Action Script", false, 80)]
    public static void CreateScript()
    {
        string folderPath = GetSelectedPathOrFallback();
        string filePath = EditorUtility.SaveFilePanelInProject(
            "Create Scriptable Action",
            "NewScriptableAction",
            "cs",
            "Enter a name for the script",
            folderPath
        );

        if (string.IsNullOrEmpty(filePath))
            return;

        string fileName = Path.GetFileNameWithoutExtension(filePath);
        string scriptContent = TEMPLATE.Replace("#SCRIPTNAME#", fileName);

        File.WriteAllText(filePath, scriptContent);
        AssetDatabase.Refresh();
    }

    private static string GetSelectedPathOrFallback()
    {
        string path = "Assets";

        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }
            break;
        }

        return path;
    }
}
#endif