using UnityEngine;

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

    [Tooltip("Cantidad inicial de materiales de construcción.")]
    public int startingMC = 0;

    [Tooltip("Cantidad inicial de hormigas obreras.")]
    public int startingWorkerAnts = 4;

    [Tooltip("Cantidad inicial proporcionada de hormigas exploradoras.")]
    public int startingExplorerAnts = 4;


    [Header("Starting structures (Map constructions)")]
    public int antHillsQuantity = 2;
    public int resourcesZoneQuantity = 2;


    [Header("Player resources")]
    public Player player; // Jugador
    public Player playerIA; // IA


    #endregion

    private void OnEnable()
    {
        player = new Player();
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
}
