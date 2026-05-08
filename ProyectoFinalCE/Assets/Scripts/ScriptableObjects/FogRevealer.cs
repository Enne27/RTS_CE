using UnityEngine;

public class FogRevealer : MonoBehaviour
{
    public int visionRadius;

    

    void Enable()
    {
        if (gameObject.GetType() == typeof(Ant))
        
        FogOfWarManager.instance.RegisterRevealer(this);

        
    }

    void Disable()
    {
        
    }
}
