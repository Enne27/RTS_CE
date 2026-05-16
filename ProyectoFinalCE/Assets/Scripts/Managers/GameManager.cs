using UnityEngine;
using UnityEngine.SceneManagement;
using static ConstantsAndKeys;

public class GameManager : MonoBehaviour
{
    #region SINGLETON
    public static GameManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region VARIABLES
    [Header("Starting Values (Player Resources)")]
    [Tooltip("Cantidad inicial de comida que posee el jugador.")]
    public int startingFood = 96;

    [Tooltip("Cantidad inicial de huevas que posee el jugador al inicio.")]
    public int startingEggs = 0;

    [Tooltip("Cantidad inicial de materiales de construcci�n.")]
    public int startingMC = 0;

    [Tooltip("Cantidad inicial de hormigas obreras.")]
    public int startingWorkerAnts = 1;

    [Tooltip("Cantidad inicial proporcionada de hormigas exploradoras.")]
    public int startingExplorerAnts = 2;


    [Header("Starting structures (Map constructions)")]
    public int antHillsQuantity = 2;
    public int resourcesZoneQuantity = 2;

    // Mecánicas desbloqueables
    [Header("Unlockable mechanics")]
    public bool explorersInvisible;

    // Modificadores especiales
    [Header("Special modifiers")]
    public float workerBonusPer10;

    [Header("Player resources")]
    public Player player; // Jugador
    public Player playerIA; // IA

    [Header("Flux")]
    public bool tutorialShown = false;
    #endregion

    private void OnEnable()
    {
        // Asegurar que los objetos Player existen antes de cargar
        player = new Player();
        playerIA = new Player();

        if (SaveSystem.CanLoadGame())
        {
            if (SceneManager.GetActiveScene().name == SINGLE_PLAYER_GAME_SCENE_NAME)
            {
                Debug.Log("GameManager.OnEnable: save exists and current scene is game scene, loading save.");
                SaveSystem.LoadGame();
            }
            else
            {
                Debug.Log("GameManager.OnEnable: save exists, deferring load until game scene.");
            }
        }
        else
        {
            // Crear nuevos jugadores con valores iniciales
            player.inventory.AddEggs(startingEggs);
            player.inventory.AddFood(startingFood);
            player.inventory.AddMC(startingMC);
            // Nota: Las hormigas iniciales se crearán en AntCreation (si no viene de carga)
        }

        // Asegurar que el objeto persista (ya se hizo en Awake, pero por claridad)
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Reiniciar los valores por los iniciales.
    /// </summary>
    public void ResetValues()
    {
        player.inventory.ResetAllVariables();
        playerIA.inventory.ResetAllVariables();

        // FALTA AÑADIR LAS OTRAS HORMIGAS INICIALES
    }

    /// <summary>
    /// Desbloquea una mecánica global del juego a partir de su identificador.
    /// </summary>
    public void UnlockMechanic(string id)
    {
        Debug.Log("Unlocked mechanic: " + id);
    }
}
