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
        public GameObject previewModel;
        public string antName;
    }

    #region VARIABLES
    [Header("Info view")]
    [SerializeField] private List<AntButton> antsButton;
    [SerializeField] GameObject antInfo;
    [SerializeField] TextMeshProUGUI antNameText;
    [SerializeField] TextMeshProUGUI statsValuesText;
    [SerializeField] TextMeshProUGUI statsText;

    [Header("Preview System")]
    [SerializeField] RawImage antImage;
    [SerializeField] private Transform previewSpawnPoint;
    [SerializeField] private RenderTexture previewTexture;

    private GameObject currentPreview;


    private LocalizedString antNameLocalized;

    private Vector2 statsValuesTextOriginalPos; // anchored position (x, y)
    private Vector2 statsValuesTextOriginalScale; // sizeDelta (width, height)

    [Header("RectTransform infoView")]
    [SerializeField] private Vector2 statsValuesTextWorkersPos = new Vector2(207.263f, 7.609f);
    [SerializeField] private Vector2 statsValuesTextWorkersScale = new Vector2(466.809f, 223.087f);


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

    [Header("Transforms spawn")]
    Transform antsSpawnPoint;
    Transform workersSpawnPoint;

    #endregion

    private void OnEnable()
    {
        //Initialize();
        if (antsSpawnPoint == null || workersSpawnPoint == null)
        {
            if (AntCreation.Instance != null)
            {
                antsSpawnPoint = AntCreation.Instance.antsSpawnPoint;
                workersSpawnPoint = AntCreation.Instance.workersSpawnPoint;
            }
        }
    }
    private void Update()
    {
        if (currentPreview != null && currentPreview.activeSelf)
        {
            currentPreview.transform.Rotate(Vector3.up, 50f * Time.deltaTime, Space.World);
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

        if(statsValuesText != null)
        {
            statsValuesTextOriginalScale = statsValuesText.rectTransform.sizeDelta;
            statsValuesTextOriginalPos = statsValuesText.rectTransform.anchoredPosition;
        }

        InitializeButtons();

        if (antImage != null && previewTexture != null)
            antImage.texture = previewTexture;
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

    /// <summary>
    /// Prepara los listeners de los botones para los diferentes eventos.
    /// </summary>
    /// <param name="data">Clase serializable que contiene toda la informaci�n necesaria.</param>
    private void SetupButtonEvents(AntButton data)
    {
        EventTrigger trigger = data.buttonComponent.GetComponent<EventTrigger>() ??
                              data.buttonComponent.gameObject.AddComponent<EventTrigger>();

        trigger.triggers.Clear();

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

    /// <summary>
    /// Evento que sucede al hacer hover del bot�n.
    /// Actualiza la informaci�n a mostrar en la preview.
    /// </summary>
    /// <param name="data"></param>
    private void OnButtonHoverStart(AntButton data)
    {
        if (data.antName.Contains("worker"))
        {
            antNameLocalized = new LocalizedString { TableReference = TABLE_ANTS, TableEntryReference = data.antName };
            UpdateWorkerPanel(antNameLocalized.GetLocalizedString());
        }
        else
        {
            antNameLocalized = new LocalizedString { TableReference = TABLE_HUD, TableEntryReference = data.antName };
            UpdateInfoPanel(data.antScript, antNameLocalized.GetLocalizedString());
        }

        ShowPreview(data.previewModel);

        antInfo.SetActive(true);
    }

    private void OnButtonHoverEnd(Button button)
    {
        antInfo.SetActive(false);

        if (currentPreview != null)
            currentPreview.SetActive(false);

        currentPreview = null;
    }

    /// <summary>
    /// Actualizaci�n de la informaci�n para todas las hormigas excepto worker.
    /// </summary>
    /// <param name="data">Script de la hormiga.</param>
    /// <param name="antName">Key de la tabla correspondiente a la hormiga</param>
    private void UpdateInfoPanel(Ant data, string antName)
    {
        antNameText.text = antName;
        statsText.gameObject.SetActive(true);

        statsValuesText.rectTransform.sizeDelta = statsValuesTextOriginalScale;
        statsValuesText.rectTransform.anchoredPosition = statsValuesTextOriginalPos;

        statsValuesText.text = $"<voffset=10><sprite name=\"huevas\"></voffset> {data.breedingCost[0]}" 
            + $"<space=50><voffset=10><sprite name=\"materiales\"></voffset> {data.breedingCost[1]}"
            + "\n" + data.HP
            + "\n" + data.armor
            + "\n" + data.speed
            + "\n" + data.strength
            + "\n" + data.vision
            + "\n" + data.reach;
    }

    /// <summary>
    /// Actualizaci�n de informaci�n para la hormiga trabajadora.
    /// </summary>
    /// <param name="antName">Key de la tabla para el nombre de la hormiga.</param>
    private void UpdateWorkerPanel(string antName)
    {
        antNameText.text = antName;
        statsText.gameObject.SetActive(false);

        statsValuesText.rectTransform.sizeDelta = statsValuesTextWorkersScale;
        statsValuesText.rectTransform.anchoredPosition = statsValuesTextWorkersPos;

        statsValuesText.text = new LocalizedString { TableReference = TABLE_ANTS, TableEntryReference = KEY_WORKER_DESCRIPTION }
            .GetLocalizedString();
    }

    /// <summary>
    /// Muestra en la rawImage el modelo de hormiga correspondiente rotando.
    /// </summary>
    /// <param name="model"></param>
    private void ShowPreview(GameObject model)
    {
        if (model == null) return;

        if (currentPreview != null)
            currentPreview.SetActive(false);

        currentPreview = model;

        currentPreview.SetActive(true);

        // reset transform por seguridad
        currentPreview.transform.localPosition = Vector3.zero;
        currentPreview.transform.localRotation = Quaternion.identity;
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
