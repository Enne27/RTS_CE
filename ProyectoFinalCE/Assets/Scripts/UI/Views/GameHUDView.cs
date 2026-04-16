using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;

public class GameHUDView : View
{
    #region VARIABLES
    [Header("Texts Resources")]
    [SerializeField] TextMeshProUGUI foodText;
    [SerializeField] TextMeshProUGUI roeText;
    [SerializeField] TextMeshProUGUI mcText; // Materiales de construcción
    [SerializeField] TextMeshProUGUI antWorkersText;
    #endregion

    public override void Initialize()
    {
        /*if(foodText == null) 
            foodText.text = GameManager.instance.startingFood.toString();
        if (roeText == null) 
            // roeText.text = GameManager.instance.startingRoe.toString();
        if(mcText == null)
            mcText.text = GameManager.instance.startingMC.toString();
        if (antWorkersText == null) 
            antWorkersText.text = GameManager.instance.startingAntWorkers.toString();*/
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="num">Número tanto positivo como negativo.</param>
    public void UpdateFoodText(int num)
    {
        //int newValue = GameManager.instance.startingFood += num;
        //foodText.text = newValue.toString();
    }
}
