using UnityEngine;

// Todas las vistas necesitan un CanvasGroup y un script de efectos.
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(UIEffects))]
[RequireComponent(typeof(GuidComponent))]
/// <summary>
/// Clase abstracta que representa los diferentes Canvas o pantallas del juego.
/// Cada uno de ellos tiene una vista personalizada que hereda de esta clase.
/// Contiene los métodos base que todos los Canvas o pantallas han de tener.
/// </summary>
public abstract class View : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] protected GuidComponent guidComponent;
    public string GetGuid()
    {
        if (!guidComponent)
            guidComponent = GetComponent<GuidComponent>();
        return guidComponent.GetGuid().ToString();
    }

    [Header("Componentes vista")]

    [Tooltip("CanvasGroup para ocultar y mostrar la vista.")]
    [SerializeField] private CanvasGroup cg;
    [SerializeField] private UIEffects effects;
    #endregion

    /// <summary>
    /// Inicializa todos los elementos necesarios del Canvas: botones, scroll views...
    /// Es el primer método que toda pantalla debe ejecutar.
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// Mostrar la vista en escena simplemente activando el objecto.
    /// </summary>
    public virtual void Show() => gameObject.SetActive(true);

    /// <summary>
    /// Ocultar la vista en escena simplemente desactivando el objeto.
    /// </summary>
    public virtual void Hide() => gameObject.SetActive(false);


    /* public virtual void ShowWithFade() => effects.;
     * public virtual void HideWithFade() => effects.;
     * 
     * */
}
