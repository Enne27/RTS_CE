using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;
using static PlayerConstants;
using static ConstantsAndKeys;
using Unity.VisualScripting;

public class BroodChamberView : View
{
    [System.Serializable]
    public class AntButton
    {
        public Button buttonComponent;
        public Ant antScript;
        public string antName;
    }

    #region VARIABLES
    [Header("Info view")]
    [SerializeField] private List<AntButton> antsButton;
    [SerializeField] GameObject antInfo;
    [SerializeField] TextMeshProUGUI antNameText;
    [SerializeField] TextMeshProUGUI statsValuesText;
    [SerializeField] TextMeshProUGUI statsText;

    private LocalizedString antNameLocalized;

    [Header("Buttons")]
    [SerializeField] Button soldierButton;
    [SerializeField] Button berserkerButton;
    [SerializeField] Button workerButton;
    [SerializeField] Button explorerButton;
    [SerializeField] Button acidButton;
    [SerializeField] Button crazyutton;
    [SerializeField] Button kamikazeButton;

    [Header("Functionality")]
    [SerializeField] BroodChamberFunction broodChamberFunction;

    [Header("Transforms")]
    Transform antsSpawnPoint;
    Transform workersSpawnPoint;

    #endregion

    private void OnEnable()
    {
        //Initialize();
        if (antsSpawnPoint == null || workersSpawnPoint == null)
        {
            antsSpawnPoint = AntCreation.Instance.antsSpawnPoint;
            workersSpawnPoint = AntCreation.Instance.workersSpawnPoint;
        }
    }

    public override void Initialize()
    {
        if (AntCreation.Instance != null)
        {
            antsSpawnPoint = AntCreation.Instance.antsSpawnPoint;
            workersSpawnPoint = AntCreation.Instance.workersSpawnPoint;
        }

        if (soldierButton != null)
            soldierButton.onClick.AddListener(()=>broodChamberFunction.CreateAnt(ANT_TYPES.SOLDIER, antsSpawnPoint));

        if (berserkerButton != null)
            berserkerButton.onClick.AddListener(()=>broodChamberFunction.CreateAnt(ANT_TYPES.BERSERKER, antsSpawnPoint));

        if (workerButton != null)
            workerButton.onClick.AddListener(()=>broodChamberFunction.CreateAnt(ANT_TYPES.WORKER, workersSpawnPoint));

        if (explorerButton != null)
            explorerButton.onClick.AddListener(()=>broodChamberFunction.CreateAnt(ANT_TYPES.EXPLORER, antsSpawnPoint));

        if (acidButton != null)
            acidButton.onClick.AddListener(()=>broodChamberFunction.CreateAnt(ANT_TYPES.ACID, antsSpawnPoint));

        if (crazyutton != null)
            crazyutton.onClick.AddListener(()=>broodChamberFunction.CreateAnt(ANT_TYPES.CRAZY, antsSpawnPoint));

        if (kamikazeButton != null)
            kamikazeButton.onClick.AddListener(()=>broodChamberFunction.CreateAnt(ANT_TYPES.KAMIKAZE, antsSpawnPoint));

        InitializeButtons();
    }

    private void InitializeButtons()
    {
        if (antsButton.Count > 0)
        {
            foreach (var mb in antsButton)
            {
                if (mb.buttonComponent != null)
                {
                    SetupButtonEvents(mb);
                }
            }
        }
    }

    private void SetupButtonEvents(AntButton data)
    {
        EventTrigger trigger = data.buttonComponent.GetComponent<EventTrigger>() ??
                              data.buttonComponent.gameObject.AddComponent<EventTrigger>();

        // Evento Hover Enter
        EventTrigger.Entry enterEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        enterEntry.callback.AddListener((e) => OnButtonHoverStart(data));
        trigger.triggers.Add(enterEntry);


        // Evento Hover Exit
        EventTrigger.Entry exitEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        exitEntry.callback.AddListener((e) => OnButtonHoverEnd(data.buttonComponent));
        trigger.triggers.Add(exitEntry);
    }

    private void OnButtonHoverStart(AntButton data)
    {

        if (data.antName.Contains("worker"))
        {
            antNameLocalized = new LocalizedString { TableReference = TABLE_ANTS, TableEntryReference = data.antName };
            UpdateWorkerPanel(data.antName);
        }
        else
        {
            antNameLocalized = new LocalizedString { TableReference = TABLE_HUD, TableEntryReference = data.antName };
            UpdateInfoPanel(data.antScript, antNameLocalized.GetLocalizedString());
        }

        antInfo.SetActive(true);
    }

    private void OnButtonHoverEnd(Button button)
    {
        antInfo.SetActive(false);
    }

    private void UpdateInfoPanel(Ant data, string antName)
    {
        antNameText.text = antName;
        statsText.gameObject.SetActive(true);
        statsValuesText.text = data.breedingCost[0].ToString() + " " + data.breedingCost[1].ToString() 
            + "\n" 
            + "\n" 
            + "\n" 
            + "\n" 
            + "\n" 
            + "\n";
    }

    private void UpdateWorkerPanel(string antName)
    {
        antNameText.text = antName;
        statsText.gameObject.SetActive(false);
        statsValuesText.text = "";
    }

    /*private void OnDisable()
    {
        soldierButton.onClick.RemoveAllListeners();
        berserkerButton.onClick.RemoveAllListeners();
        workerButton.onClick.RemoveAllListeners();
        explorerButton.onClick.RemoveAllListeners();
        acidButton.onClick.RemoveAllListeners();
        crazyutton.onClick.RemoveAllListeners();
        kamikazeButton.onClick.RemoveAllListeners();
    }*/


}
