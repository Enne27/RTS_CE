#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeEditor : EditorWindow
{
    AntHillView antHillView;
    InspectorView inspectorView;

    Anthill ia1_Anthill;

    #region Default
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("AiTools/BehaviourTree")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
    } 
    #endregion

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        m_VisualTreeAsset.CloneTree(root);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Mecaniques/Enemy_AI/AI_Tool/BehaviourTree/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        antHillView = root.Q<AntHillView>();
        inspectorView = root.Q<InspectorView>();
        antHillView.OnStructureSelected = OnStructureSelectionChanged;

        ia1_Anthill = ScriptableObject.CreateInstance<Anthill>();
        antHillView.PopulateView(ia1_Anthill);

    }

    void OnStructureSelectionChanged(StructureView structureView)
    {
        inspectorView.UpdateSelection(structureView);
    }
}
#endif