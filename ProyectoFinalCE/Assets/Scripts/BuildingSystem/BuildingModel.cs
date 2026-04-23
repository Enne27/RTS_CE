using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingModel : MonoBehaviour
{
    [SerializeField] private Transform wrapper;
    public float Rotation => wrapper.transform.eulerAngles.y;

    private BuildingShapeUnit[] shapeUnits;
    private Material buildingMaterial;
    
    private Renderer model;

    private void Awake()
    {
        shapeUnits = GetComponentsInChildren<BuildingShapeUnit>();
        buildingMaterial = GetComponentInChildren<Renderer>().material;
        model = GetComponentInChildren<Renderer>();
    }

    public void Rotate(float rotationStep)
    {
        wrapper.Rotate(new(0, rotationStep, 0));
    }

    public List<Vector3> GetAllBuilddingPositions()
    {
        return shapeUnits.Select(unit => unit.transform.position).ToList();
    }

    public void ChangeModelOutlineColor(Color color)
    {
        buildingMaterial.SetColor("_Outline_Color", color);
    }


}
