using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ConstantsAndKeys;

public class TutorialControl : MonoBehaviour
{
    #region VARIABLES
    [Header("Base")]
    public GameObject invertedMask;

    [Header("View & cameras")]
    [SerializeField] public DialogueView dialogueView;
    [SerializeField] CameraMovement cameraMoveScript;

    [Header("Items")]
    public GameObject resources;
    public GameObject AntsType;
    public GameObject generalInfo;
    public GameObject antHill;

    [Header("Transforms")]
    [SerializeField] List<Transform> positions;

    [Header("Control Tutorial")]
    public int lineNum = 0;
    public bool tutorialShowed = false;

    private BuildingType? requiredBuildingType = null;
    private bool antCreated = false;
    #endregion

    void Awake()
    {
        tutorialShowed = GameManager.instance.tutorialShown;
        if (tutorialShowed == false)
        {
            DialogueManager.instance.startLine.AddListener(TutorialController);
            dialogueView.ShowDialogue(TABLE_DIALOGUES, KEY_DIALOGUES_TUTORIAL);
            DialogueManager.instance.endDialogue.AddListener(EndTutorial);

            //Avisamos al tutorial
            BuildingManager.Instance.OnBuildingPlaced += CheckBuildingTask;
            
        }
    }

    private void Start()
    {
        if (tutorialShowed == false)
        {
            cameraMoveScript?.DisableCameraInput();

            //ViewManager.Show<DialogueView>();
            ViewManager.GetView<DialogueView>().gameObject.SetActive(true);
            PauseController.instance.pausableMoment = false;
        }
        else ViewManager.Show<GameHUDView>();
    }

    void TutorialController()
    {
        switch (lineNum)
        {
            case 3: // "Construye una cámara real"
                requiredBuildingType = BuildingType.QueenChamber;
                DialogueManager.instance.taskPending = true;
                break;

            case 6: // "Ahora necesitamos una cámara de cría"
                requiredBuildingType = BuildingType.BroodChamber;
                DialogueManager.instance.taskPending = true;
                break;

            case 7: // "Ahora crea una hormiga"
                while (GameManager.instance.player.ants.Count <= 4)
                {
                    DialogueManager.instance.taskPending = false;
                }
                DialogueManager.instance.taskPending = true;
                break;
        }
        
        /* switch (lineNum)
         {
             case 0:
                 invertedMask.SetActive(true);
                 StartCoroutine(MoverSuavemente(invertedMask.transform, positions[0].position, 0.4f));
                 break;
             case 1:
                 StartCoroutine(MoverSuavemente(invertedMask.transform, positions[1].position, 0.4f));
                 AntsType.SetActive(true);
                 resources.SetActive(true);
                 break;
             case 2:

                 StartCoroutine(MoverYEscalar(
                     invertedMask.transform,
                     positions[5].position,
                     new Vector3(6f, 1f, 0f),
                     0.5f));
                 break;
             case 4:

                 StartCoroutine(MoverYEscalar(
                    invertedMask.transform,
                    positions[1].position,
                    new Vector3(1f, 1f, 0f),
                    0.5f));
                 break;
             case 5:
                 AntsType.SetActive(false);
                 resources.SetActive(false);
                 StartCoroutine(MoverSuavemente(invertedMask.transform, positions[0].position, 0.4f));
                 break;
         }*/
        Debug.Log(lineNum.ToString());
        lineNum++;
    }

    public void EndTutorial()
    {
        dialogueView.Hide();
        invertedMask.SetActive(false);
        ViewManager.Show<GameHUDView>(false);
        PauseController.instance.pausableMoment = true;
        tutorialShowed = true;
        GameManager.instance.tutorialShown = tutorialShowed;
        cameraMoveScript.EnableCameraInput();
    }

    void CheckBuildingTask(BuildingType builtType)
    {
        if (requiredBuildingType != null && builtType == requiredBuildingType)
        {
            // Tarea completada: Desbloqueamos el diálogo
            DialogueManager.instance.taskPending = false;
            requiredBuildingType = null;
            Debug.Log("Tarea completada: " + builtType);
        }
    }

    IEnumerator MoverSuavemente(Transform objeto, Vector3 destino, float duracion)
    {
        Vector3 inicio = objeto.position;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            objeto.position = Vector3.Lerp(inicio, destino, tiempo / duracion);
            tiempo += Time.deltaTime;
            yield return null;
        }

        objeto.position = destino;
    }

    IEnumerator MoverYEscalar(Transform objeto, Vector3 posDestino, Vector3 escalaDestino, float duracion)
    {
        Vector3 inicioPos = objeto.position;
        Vector3 inicioEscala = objeto.localScale;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            float t = tiempo / duracion;
            objeto.position = Vector3.Lerp(inicioPos, posDestino, t);
            objeto.localScale = Vector3.Lerp(inicioEscala, escalaDestino, t);
            tiempo += Time.deltaTime;
            yield return null;
        }

        objeto.position = posDestino;
        objeto.localScale = escalaDestino;
    }

}
