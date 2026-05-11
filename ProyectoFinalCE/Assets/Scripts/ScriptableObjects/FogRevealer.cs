using UnityEngine;

public class FogRevealer : MonoBehaviour
{
    public int visionRadius;

    // Usamos OnEnable y OnDisable en lugar de Start y OnDestroy.
    // Esto es vital si usas "Object Pooling" o si las unidades entran/salen de transportes.
    private void Start()
    {
        if (FogOfWarManager.Instance != null)
        {
            FogOfWarManager.Instance.RegisterRevealer(this);
        }
    }


    private void OnEnable()
    {
        if (gameObject.GetType() == typeof(Ant))
        
        FogOfWarManager.instance.RegisterRevealer(this);

        
    }

    void Disable()
    {
        
    }
}
