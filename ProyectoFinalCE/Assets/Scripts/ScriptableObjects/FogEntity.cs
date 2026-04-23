using System.Collections;
using UnityEngine;
using static PlayerConstants;

public class FogEntity : MonoBehaviour
{

    [Header("Configuración de Entidad")]
    public ENTITY_TYPE type = ENTITY_TYPE.ENEMY;

    [Tooltip("Frecuencia con la que comprueba si debe ser visible (segundos)")]
    public float checkInterval = 0.2f;

    [Header("Elementos Visuales")]
    [Tooltip("El objeto hijo que contiene las mallas (MeshRenderers) y la UI de la unidad. " +
             "Activaremos y desactivaremos este objeto.")]
    public GameObject visualRoot;

    private bool isCurrentlyVisible = true;

    private void Start()
    {
        // Validación de seguridad
        if (visualRoot == null)
        {
            Debug.LogError($"FogEntity en {gameObject.name} no tiene asignado un visualRoot.", this);
            return;
        }

        // Iniciar el bucle de comprobación
        StartCoroutine(VisibilityCheckRoutine());
    }

    private IEnumerator VisibilityCheckRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(checkInterval);

        while (true)
        {
            UpdateVisibility();
            yield return wait;
        }
    }

    private void UpdateVisibility()
    {
        // 1. Preguntamos al Manager por el color del píxel en nuestra posición actual
        Color32 visibilityState = FogOfWarManager.Instance.GetVisibilityAtPosition(transform.position);
        Debug.Log(visibilityState);


        bool shouldBeVisible = false;

        // 2. Lógica de los tres estados
        // visibilityState.r -> Visión en tiempo real (128 o más = Visible)
        // visibilityState.g -> Historial de exploración (128 o más = Explorado)
        if (visibilityState.r > 128)
        {
            // ESTADO: VISIBLE (Dentro del rango de una unidad aliada)
            // Todos los objetos son visibles
            shouldBeVisible = true;
        }
        else if (visibilityState.g > 128)
        {
            // ESTADO: EXPLORADO (Zona gris, ya pasamos por aquí pero ahora no hay aliados)
            // Solo los recursos o edificios estáticos se quedan visibles. 
            // Los enemigos se ocultan.
            if (type == ENTITY_TYPE.RESOURCE)
            {
                shouldBeVisible = true;
            }
            else
            {
                shouldBeVisible = false;
            }
        }
        else
        {
            // ESTADO: NO EXPLORADO (Negro total)
            // Nada es visible
            shouldBeVisible = false;
        }

        // 3. Aplicar el cambio solo si el estado ha cambiado (para ahorrar rendimiento)
        if (shouldBeVisible != isCurrentlyVisible)
        {
            isCurrentlyVisible = shouldBeVisible;
            visualRoot.SetActive(isCurrentlyVisible);

            // Aquí también podrías añadir lógica extra, como pausar las animaciones 
            // del enemigo si no es visible, o desactivar su icono en el minimapa.
        }
    }
}