using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    #region VARIABLES
    static PauseController pauseController;

    [Tooltip("Esto son el conjunto de inputs del juego")]
    [SerializeField] InputActionAsset inputActions;

    private InputAction escapeToPause;
    public bool pausableMoment;
    private bool isPaused;

    [HideInInspector] public UnityEvent onPause;
    [HideInInspector] public UnityEvent onUnPause;
    #endregion

    #region SINGLETON

    public static PauseController instance
    {
        get
        {
            return FindOrGetPauseController();
        }
    }

    static PauseController FindOrGetPauseController()
    {
        if (pauseController == null) 
            pauseController = FindFirstObjectByType<PauseController>();

        return pauseController;
    }
    #endregion

    private void Awake()
    {
        escapeToPause = inputActions.FindActionMap("General").FindAction("Pause");
        escapeToPause.Enable();
    }


    void Update()
    {
        if (escapeToPause.triggered)
        {
            if (pausableMoment)
                TogglePause();
        }

    }

    /// <summary>
    /// Verifica el estado de pausa dependiendo de si se muestra la vista del menú de pausa.
    /// </summary>
    public void TogglePause()
    {
        // Verifica si la vista de pausa está activa
        isPaused = ViewManager.IsViewActive<PauseMenuView>();
        //Debug.Log(isPaused);

        if (isPaused)
        {
            ViewManager.Hide<PauseMenuView>();
            ViewManager.ShowLastView();
            onUnPause?.Invoke();
        }
        else
        {
            //ViewManager.Hide<GameHUDView>(); // AQUÍ DEBERÍAMOS REVISAR LA VIEW, NO SIEMPRE SERÁ LA DE GAME, PUEDE SER INFO GENERAL, ETC.
            ViewManager.Show<PauseMenuView>();
            onPause?.Invoke();
        }

    }
}
