using System.Collections.Generic;
using UnityEngine;

public class BuildingPreview : MonoBehaviour
{
    public enum BuildingPreviewState
    {
        POSITIVE,
        NEGATIVE
    }

    [SerializeField] private Material positiveMaterial;

    [SerializeField] private Material negativeMaterial;

    public BuildingPreviewState state { get; private set; } = BuildingPreviewState.NEGATIVE;

    public BuildingData data { get; private set; }

    public BuildingModel model { get; private set; }

    private List<Renderer> renderers = new();

    private List<Collider> colliders = new();

    public void Setup(BuildingData data)
    {
        this.data = data;
        model = Instantiate(data.model, transform.position, Quaternion.identity, transform);
        renderers.AddRange(model.GetComponentsInChildren<Renderer>());
        colliders.AddRange(model.GetComponentsInChildren<Collider>());
        foreach (var col in colliders)
        {
            col.enabled = false;
        }
        SetPreviewMaterial(state);
    }

    public void ChangeState(BuildingPreviewState newState)
    {
        if(newState == state) return;
        state = newState;
        SetPreviewMaterial(state);
    }

    public void Rotate(int rotationStep)
    {
        model.Rotate(rotationStep);
    }

    private void SetPreviewMaterial(BuildingPreviewState newState)
    {
        Material previewMat = newState == BuildingPreviewState.POSITIVE ? positiveMaterial : negativeMaterial;

        foreach (var rend in renderers)
        {
            Material[] mats = new Material[rend.sharedMaterials.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = previewMat;
            }
            rend.materials = mats;
        }
    }
}
