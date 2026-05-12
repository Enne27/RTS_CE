using UnityEngine;

public class FogRevealer : MonoBehaviour
{
    [Header("Configuración de Visión")]
    [Tooltip("El radio en unidades del mundo que esta entidad puede revelar.")]
    public float visionRadius = 10f;

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
        // Al activarse el objeto, se registra en el Manager
        if (FogOfWarManager.Instance != null)
        {
            FogOfWarManager.Instance.RegisterRevealer(this);
        }
    }

    private void OnDisable()
    {
        // Al desactivarse (o morir), se borra de la lista del Manager
        if (FogOfWarManager.Instance != null)
        {
            FogOfWarManager.Instance.UnregisterRevealer(this);
        }
    }

    // Opcional: Dibujar el radio en el editor para facilitar el diseño de niveles
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, visionRadius);
    }
}