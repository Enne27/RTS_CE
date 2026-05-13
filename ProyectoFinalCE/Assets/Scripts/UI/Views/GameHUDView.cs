using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static PlayerConstants;

public class GameHUDView : View
{
    #region VARIABLES
    [Header("Texts Resources")]
    [SerializeField] TextMeshProUGUI foodText;
    [SerializeField] TextMeshProUGUI eggsText;
    [SerializeField] TextMeshProUGUI mcText; // Materiales de construcción
    [SerializeField] TextMeshProUGUI antWorkersText;


    [Header("Buttons")]
    [SerializeField] public Button constructionButton;
    private bool constructionMenuActived = false;
    [SerializeField] Button generalInfoButton;

    [Header("ANTS")]
    [SerializeField] Button fakeAntsDropwdownButton;
    [SerializeField] GameObject fakeAntsDropdown;
    bool isActive = false;
    
    [SerializeField] TextMeshProUGUI totalAntsText;
    [SerializeField] TextMeshProUGUI explorerAntsText;
    [SerializeField] TextMeshProUGUI soldierAntsText;
    [SerializeField] TextMeshProUGUI berserkerAntsText;
    [SerializeField] TextMeshProUGUI acidAntsText;
    [SerializeField] TextMeshProUGUI crazyAntsText;
    [SerializeField] TextMeshProUGUI kamikazeAntsText;

    [Header("General Info Texts")]
    [SerializeField] TextMeshProUGUI currentEraText;
    #endregion

    public override void Initialize()
    {
        if (GameManager.instance.tutorialShown == false)
        {
            foodText.text = GameManager.instance.startingFood.ToString();
            eggsText.text = GameManager.instance.startingEggs.ToString() + "/" + GameManager.instance.player.inventory.eggCapacity;
            mcText.text = GameManager.instance.startingMC.ToString();
            antWorkersText.text = GameManager.instance.startingWorkerAnts.ToString();

            totalAntsText.text = GameManager.instance.startingExplorerAnts.ToString();
            explorerAntsText.text = GameManager.instance.startingExplorerAnts.ToString();
            soldierAntsText.text = "0";
            berserkerAntsText.text = "0";
            acidAntsText.text = "0";
            crazyAntsText.text = "0";
            kamikazeAntsText.text = "0";

        }
        else
        {
            foodText.text = GameManager.instance.player.inventory.food.ToString();
            eggsText.text = GameManager.instance.player.inventory.eggs.ToString() + "/" + GameManager.instance.player.inventory.eggCapacity;
            mcText.text = GameManager.instance.player.inventory.materials.ToString();
            antWorkersText.text = GameManager.instance.player.inventory.workerAnts.ToString();

            totalAntsText.text = GameManager.instance.player.ants.Count.ToString();

            /*explorerAntsText.text = GameManager.instance.startingExplorerAnts.ToString();
            soldierAntsText.text = "0";
            berserkerAntsText.text = "0";
            acidAntsText.text = "0";
            crazyAntsText.text = "0";
            kamikazeAntsText.text = "0";*/
        }

        currentEraText.text = GameManager.instance.player.currentEra.ToString();

        if(constructionButton != null)
        {
          constructionButton.onClick.AddListener(()=> {
              if(constructionMenuActived == false)
              {
                  //ViewManager.Show<ConstructionMenuView>();
                  ViewManager.GetView<ConstructionMenuView>().gameObject.SetActive(true);
                  constructionMenuActived = true;
              }
              else 
              { 
                  constructionMenuActived = false;
                  //ViewManager.ShowLastView(); 
                  ViewManager.GetView<ConstructionMenuView>().gameObject.SetActive(false);
              }
          });
        }

        if (fakeAntsDropwdownButton != null)
            fakeAntsDropwdownButton.onClick.AddListener(ShowAntsUI);
        
        if (generalInfoButton != null)
            generalInfoButton.onClick.AddListener(()=>ViewManager.Show<GeneralInfoView>(true));
    }

    private void ShowAntsUI()
    {
        if (isActive)
        {
            fakeAntsDropdown.gameObject.SetActive(false);
            isActive = false;
        } else
        {
            fakeAntsDropdown.gameObject.SetActive(true);
            isActive = true;
        }
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
        antWorkersText.text = newValue.ToString();
    }

    /// <summary>
    /// Actualiza las cantidades de hormigas del jugador.
    /// </summary>
    /// <param name="antType">Tipo de hormiga para actualizar le texto.</param>
    /// <param name="num">Si es positivo añade una hormiga, si es negativo, la resta.</param>
    public void UpdateAntText(ANT_TYPES antType, int num)
    {
        switch (antType)
        {
            case ANT_TYPES.ACID:
                acidAntsText.text = AddAntToCurrentTextValue(acidAntsText.text, num);
                break;

            case ANT_TYPES.BERSERKER:
                berserkerAntsText.text = AddAntToCurrentTextValue(berserkerAntsText.text, num);
                break;

            case ANT_TYPES.EXPLORER:
                explorerAntsText.text = AddAntToCurrentTextValue(explorerAntsText.text, num);
                break;

            case ANT_TYPES.SOLDIER:
                soldierAntsText.text = AddAntToCurrentTextValue(soldierAntsText.text, num);
                break;

            case ANT_TYPES.CRAZY:
                crazyAntsText.text = AddAntToCurrentTextValue(crazyAntsText.text, num);
                break;

            case ANT_TYPES.KAMIKAZE:
                kamikazeAntsText.text = AddAntToCurrentTextValue(kamikazeAntsText.text, num);
                break;

            case ANT_TYPES.WORKER:
                UpdateWorkerAntsText();
                break;
        }

        if(antType != ANT_TYPES.WORKER)
            totalAntsText.text = AddAntToCurrentTextValue(totalAntsText.text, num);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="antText"></param>
    /// <param name="num">Positivo suma, negativo resta.</param>
    /// <returns></returns>
    private string AddAntToCurrentTextValue(string antText, int num)
    {
        int intValue = Int32.Parse(antText);
        if (intValue + num < 0)
            return "0";
        else { return antText = (intValue + num).ToString(); }     
    }
}
