using UnityEngine;

public class QueenChamberFunction : MonoBehaviour
{
    #region VARIABLES
    [Header("Building parameters")]
    [Tooltip("Tiempo que tarda en producir huevas.")]
    [SerializeField] float timeToProduceEggs = 60f;

    [Tooltip("Nivel del edificio.")]
    private int lvl = 1;

    [Tooltip("Cantidad de huevas que produce por minuto.")]
    [SerializeField] float quantityProduction;
    // DICCIONARIO O LO QUE SEA DE LOS VALORES DE CANTIDAD DE PRODUCCIÓN, COSTE DE HUEVAS, COSTE DE MC POR NIVEL.
    // EN IA HAY MÁS COSAS PARA ESTOS
    #endregion

    private void OnEnable()
    {
        TimeManager.Instance.Register(timeToProduceEggs, ProduceEggs);
    }

    private void OnDisable()
    {
        TimeManager.Instance.Unregister(timeToProduceEggs, ProduceEggs);
    }

    /// <summary>
    /// FALTARÍA FEEDBACK, PERO SE GENERAN CADA 60 SEGUNDOS
    /// </summary>
    public void ProduceEggs()
    {
       int currentEggs = GameManager.instance.player.inventory.eggs;
       int currentEggCapacity = GameManager.instance.player.inventory.eggCapacity;

       if (currentEggs < currentEggCapacity) 
       {
            GameManager.instance.player.inventory.eggs += currentEggCapacity - currentEggs;
       }
    }
}
