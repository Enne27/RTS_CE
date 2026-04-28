using TMPro;
using UnityEngine;

public class DialogueView : View
{
    #region VARIABLES
    [Tooltip("Panel para el diálogo.")]
    [SerializeField] private GameObject dialoguePanel;
    [Tooltip("Componente TextMeshProUGUI para mostrar el diálogo.e")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    #endregion

    public override void Initialize()
    {
    }

    public void ShowDialogue(string tableName, string dialogueKey)
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        DialogueManager.instance.StartDialogue(dialogueText, tableName, dialogueKey);
    }
}
