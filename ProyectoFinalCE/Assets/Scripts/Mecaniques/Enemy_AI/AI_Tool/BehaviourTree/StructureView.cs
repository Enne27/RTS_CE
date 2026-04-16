using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class StructureView : UnityEditor.Experimental.GraphView.Node
{
    public Structure structure;

    public Action<StructureView> OnStructureSelected;
    public StructureView(Structure structure) : base("Assets/Scripts/Mecaniques/Enemy_AI/AI_Tool/NodeView.uxml")
    {
        this.structure = structure;
        this.title = structure.name;
        this.viewDataKey = structure.guid;
        style.left = structure.position.x;
        style.top = structure.position.y;
        SetUpClasses();

        ProgressBar progressBar = this.Q<ProgressBar>("ProgressBar");
        progressBar.title = "UpgradeTime";
        progressBar.bindingPath = "remainingUpgradeTime_100";
        progressBar.Bind(new SerializedObject(structure));
    }

    private void SetUpClasses()
    {
        if(structure is Camara_de_Almacenamiento)
        {
            AddToClassList("Camara_de_Almacenamiento");
        }
        else if (structure is Camara_de_Cria)
        {
            AddToClassList("Camara_de_Cria");
        }
        else if (structure is Cámara_de_la_Reina)
        {
            AddToClassList("Camara_de_la_Reina");
        }
        else if (structure is Monticulo)
        {
            AddToClassList("Monticulo");
        }
        else if (structure is Zona_de_Forrajeo)
        {
            AddToClassList("Zona_de_Forrajeo");
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        structure.position.x = newPos.xMin;
        structure.position.y = newPos.yMin;
    }

    public override void OnSelected()
    {
        base.OnSelected();
        if(OnStructureSelected != null)
        {
            OnStructureSelected.Invoke(this);
        }
    }
}
