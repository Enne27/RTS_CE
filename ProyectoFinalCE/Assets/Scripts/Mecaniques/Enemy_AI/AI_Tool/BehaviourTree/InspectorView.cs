using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }
    public InspectorView()
    {
    }

    Editor editor;
    internal void UpdateSelection(StructureView structureView)
    {
        Clear();
        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(structureView.structure);
        IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        Add(container);
    }
}
