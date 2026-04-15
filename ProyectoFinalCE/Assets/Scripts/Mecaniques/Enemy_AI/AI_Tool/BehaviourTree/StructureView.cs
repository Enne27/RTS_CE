using System;
using UnityEngine;

public class StructureView : UnityEditor.Experimental.GraphView.Node
{
    public Structure structure;

    public Action<StructureView> OnStructureSelected;
    public StructureView(Structure structure)
    {
        this.structure = structure;
        this.title = structure.name;
        this.viewDataKey = structure.guid;
        style.left = structure.position.x;
        style.top = structure.position.y;
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
