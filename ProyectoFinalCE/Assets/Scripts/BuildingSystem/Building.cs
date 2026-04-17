using UnityEngine;

public class Building : MonoBehaviour
{
    private BuildingModel model;

    private BuildingData data;

    public void Setup(BuildingData data, float rotation)
    {
        this.data = data;
        model = Instantiate(data.model, transform.position, Quaternion.identity, transform);
        model.Rotate(rotation);
    }
}
