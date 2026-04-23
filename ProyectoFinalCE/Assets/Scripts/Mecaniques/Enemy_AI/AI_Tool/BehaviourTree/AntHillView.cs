#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;

[UxmlElement]
public partial class AntHillView : GraphView
{
    Anthill anthill;
    public Action<StructureView> OnStructureSelected;

    public AntHillView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
            "Assets/Scripts/Mecaniques/Enemy_AI/AI_Tool/BehaviourTree/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);
    }

    internal void PopulateView(Anthill anthill)
    {
        this.anthill = anthill;
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        anthill.structures.ForEach(s => CreateStructureView(s)); 
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                StructureView structureView = elem as StructureView;
                if (structureView != null)
                {
                    anthill.DeleteStrucutre(structureView.structure);
                }
            });
        }
        return graphViewChange;
    }


    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);
        var types = TypeCache.GetTypesDerivedFrom<Structure>();
        foreach (var type in types)
        {
            evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateStructure(type));
        }
    }

    public void CreateStructure(System.Type type)
    {
        Structure structure = anthill.CreateStructure(type);
        CreateStructureView(structure);
    }
    public void CreateStructure(System.Type type, Rect pos)
    {
        Structure structure = anthill.CreateStructure(type);
        CreateStructureView(structure, pos);
    }

    void CreateStructureView(Structure structure)
    {
        StructureView structureView = new StructureView(structure);
        structureView.OnStructureSelected = OnStructureSelected;
        AddElement(structureView);
    }
    void CreateStructureView(Structure structure, Rect pos)
    {
        StructureView structureView = new StructureView(structure);
        structureView.SetPosition(pos);
        structureView.OnStructureSelected = OnStructureSelected;
        AddElement(structureView);
    }
}
#endif