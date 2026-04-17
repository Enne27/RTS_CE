using UnityEngine;

public class QueenChamberFunction : MonoBehaviour
{
    #region VARIABLES
    [Header("Building parameters")]
    [Tooltip("Tiempo que tarda en producir huevas.")]
    [SerializeField] float timeToProduceEggs = 60f;

    [Tooltip("Cantidad de huevas que produce por minuto.")]
    [SerializeField] float quantityProduction;

    [Tooltip("Nivel del edificio.")]
    private int lvl = 1;
    #endregion

    private void OnEnable()
    {
        TimeManager.Instance.Register(timeToProduceEggs, ProduceEggs);
    }

    private void OnDisable()
    {
        TimeManager.Instance.Unregister(timeToProduceEggs, ProduceEggs);
    }

    public void ProduceEggs()
    {
       
       // if () { }
    }
}
