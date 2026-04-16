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
    [Header("Starting Values")]
    [Tooltip("Cantidad inicial de comida que posee el jugador.")]
    public int startingFood = 96;

    [Tooltip("Cantidad inicial de huevas que posee el jugador al inicio.")]
    public int startingRoe = 0;

    [Tooltip("Cantidad inicial de materiales de construcción.")]
    public int startingMC = 0;


    [Header("Player resources")]
    Player player; // jugador
    Player playerIA; // IA


    #endregion

    /// <summary>
    /// Reiniciar los valores por los iniciales.
    /// </summary>
    public void ResetValues()
    {
        player.inventory.ResetAllVariables();
        playerIA.inventory.ResetAllVariables();
    }
}
