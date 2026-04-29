using TMPro;
using UnityEngine;

public class DialogueView : View
{
    #region VARIABLES
    [Header("UI visual dialogue")]
    [Tooltip("Panel para el diálogo.")]
    [SerializeField] private GameObject dialoguePanel;

    [Tooltip("Componente TextMeshProUGUI para mostrar el diálogo.e")]
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Tooltip("Indicador de que hay más diálogo.")]
    [SerializeField] GameObject indicatorNextLines;
    #endregion

    public override void Initialize()
    {
        
    }

    public void ShowDialogue(string tableName, string dialogueKey)
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if(indicatorNextLines != null)
        {
            indicatorNextLines.SetActive(true);
            DialogueManager.instance.endDialogue.AddListener(()=> indicatorNextLines.SetActive(false));
        }

        DialogueManager.instance.StartDialogue(dialogueText, tableName, dialogueKey);
    }

    public override void Hide()
    {
        base.Hide();

        DialogueManager.instance.endDialogue.RemoveListener(()=> indicatorNextLines.SetActive(false));
    }
}
