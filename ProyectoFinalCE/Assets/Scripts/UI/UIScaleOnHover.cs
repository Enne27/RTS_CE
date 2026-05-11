using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// Componente para manejar efectos de escala UI en eventos hover
/// </summary>
public class UIScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Factor de escala aplicado")]
    [SerializeField] private float scaleFactor = 1.1f;

    [Tooltip("Duración de la animación en segundos")]
    [SerializeField] private float duration = 0.2f;

    private Vector3 originalScale;


    [Header("Sound")]
    [Tooltip("Event Emitter del sonido para onHover un botón.")] 
    [SerializeField] StudioEventEmitter onHoverEmitter;

    private void Awake()
    {
        InitializeValues(scaleFactor, duration);
    }

    private void OnEnable()
    {
        gameObject.transform.localScale = originalScale;
    }


    /// <summary>
    /// Inicializa el componente con parámetros personalizados
    /// </summary>
    public void InitializeValues(float scale, float animDuration)
    {
        scaleFactor = scale;
        duration = animDuration;
        originalScale = transform.localScale;
    }

    /// <summary>
    /// Maneja el evento de entrada del puntero.
    /// Emite un sonido SFX y escala el objeto de interfaz.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onHoverEmitter != null) SFXManager.PlaySFX(onHoverEmitter);

        UIEffects.instance.IncreaseScale(gameObject, originalScale, scaleFactor, duration);
    }

    /// <summary>
    /// Maneja el evento de salida del puntero.
    /// Detiene el sonido SFX y devuelve el tamaño a la normalidad.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (onHoverEmitter != null) SFXManager.StopSFX(onHoverEmitter);

        UIEffects.instance.RestartScale(gameObject, originalScale, duration);
    }
}
