using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUDView : View
{
    #region VARIABLES
    [Header("Texts Resources")]
    [SerializeField] TextMeshProUGUI foodText;
    [SerializeField] TextMeshProUGUI eggsText;
    [SerializeField] TextMeshProUGUI mcText; // Materiales de construcción
    [SerializeField] TextMeshProUGUI antWorkersText;

    [Header("Buttons")]
    [SerializeField] Button constructionButton;
    #endregion

    public override void Initialize()
    {
        foodText.text = GameManager.instance.startingFood.ToString();
        eggsText.text = GameManager.instance.startingEggs.ToString() + "/" + GameManager.instance.player.inventory.eggCapacity;
        mcText.text = GameManager.instance.startingMC.ToString();
        antWorkersText.text = GameManager.instance.startingWorkerAnts.ToString();

        //if(constructionButton != null)
        //  constructionButton.onClick.AddListener();
    }

    #region UPDATE_TEXTS
    /// <summary>
    /// Cambiamos el texto de la cantidad de comida a la que tenga el player en su inventario.
    /// </summary>
    public void UpdateFoodText()
    {
        int newValue = GameManager.instance.player.inventory.food;
        int maxCapacity = GameManager.instance.player.inventory.foodCapacity;
        foodText.text = newValue.ToString() + "/" + maxCapacity.ToString();
    }

    /// <summary>
    /// Cambiamos el texto de la cantidad de huevas a la que tenga el player en su inventario.
    /// </summary>
    public void UpdateEggsText()
    {
        int newValue = GameManager.instance.player.inventory.eggs;
        int maxCapacity = GameManager.instance.player.inventory.eggCapacity;
        eggsText.text = newValue.ToString() + "/" + maxCapacity.ToString();
    }

    /// <summary>
    /// Cambiamos el texto de la cantidad de materiales de construcción a la que tenga el player en su inventario.
    /// </summary>
    public void UpdateMCText()
    {
        int newValue = GameManager.instance.player.inventory.materials;
        int maxCapacity = GameManager.instance.player.inventory.materialsCapacity;
        mcText.text = newValue.ToString() + "/" + maxCapacity.ToString();
    }

    /// <summary>
    /// Cambiamos el texto de la cantidad de obreras a la que tenga el player en su inventario.
    /// </summary>
    public void UpdateWorkerAntsText()
    {
        int newValue = GameManager.instance.player.inventory.workerAnts;
        eggsText.text = newValue.ToString();
    }
    #endregion
}
