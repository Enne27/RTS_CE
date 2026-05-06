using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class DialogueManager : MonoBehaviour
{
    #region VARIABLES

    [SerializeField, Tooltip("Tiempo de espera para que simule una máquina de escribir.")] private float tipingTime;

    [Header("Input Action")]
    [SerializeField] InputActionAsset inputActions;
    private InputAction nextLineInput;

    // Eventos para quien los necesite.
    public UnityEvent onStartDialogue;
    public UnityEvent endDialogue;
    public UnityEvent startLine;
    public UnityEvent endLine;

    //Asegura que unicamente haya una Corutina de muestra de dialogos ejecutandose a la vez 
    private Coroutine showingLine = null;

    private bool skipLine = false;
    bool showingLineBool = false;
    private bool inputUsedForSkip = false;
    private bool inputPressed = false;
    private bool waitingForInput = false;


    #endregion

    #region Singleton
    [Header("Singleton")]
    static DialogueManager dialogueManager;

    /// <summary>
    /// Le damos la propiedad del get a la variable estática instance y llamamos a nuestra 
    /// función estática que hará el singleton.
    /// </summary>
    public static DialogueManager instance
    {
        get
        {
            return RequestDialogueManager();
        }
    }

    /// <summary>
    /// Realiza el singleton (devuelve el objeto sinleton).
    /// </summary>
    /// <returns></returns>
    static DialogueManager RequestDialogueManager()
    {
        if (dialogueManager == null)
        {
            dialogueManager = FindFirstObjectByType<DialogueManager>();
        }
        return dialogueManager;
    }
    #endregion

    private void Awake()
    {
        if (tipingTime < 0)
        {
            tipingTime = 0;
        }

        nextLineInput = inputActions.FindActionMap("General").FindAction("NextDialogueLine");
        nextLineInput.performed += ctx => OnInputPressed();
        nextLineInput.Enable();
    }

    private void OnInputPressed()
    {
        if (waitingForInput)
        {
            inputPressed = true;
        }
        else if (showingLineBool)
        {
            skipLine = true;
        }
    }

    /// <summary>
    /// Método a llamar cuando empiece un "diálogo". 
    /// <para>Si queremos activar un panel, lo hacemos aparte.</para>
    /// </summary>
    public void StartDialogue(TextMeshProUGUI tmProDialogue, string tableName, string dialogueKey)
    {
        List<string> dialogueLines = GetLinesWithKey(tableName, dialogueKey);

        //cada vez que se llame una, se comprueba si hay otra Corutina ejecutandose y la finaliza 
        if (showingLine != null)
        {
            StopCoroutine(showingLine);
        }
        showingLine = StartCoroutine(ShowLine(tmProDialogue, dialogueLines));
        onStartDialogue.Invoke();
    }

    /// <summary>
    /// Corrutina para mostrar la línea letra a letra.
    /// Se espera para pasar a la siguiente.
    /// </summary>
    /// <returns></returns>
    IEnumerator ShowLine(TextMeshProUGUI tmProDialogue, List<string> dialogueLines)
    {
        int index = 0;
        tmProDialogue.text = "";

        while (index < dialogueLines.Count)
        {
            startLine.Invoke();
            tmProDialogue.text = string.Empty;

            foreach (char ch in dialogueLines[index])
            {
                if (skipLine)
                {
                    showingLineBool = false;
                    tmProDialogue.text = dialogueLines[index];

                    skipLine = false;
                    inputUsedForSkip = true; 
                    yield return StartCoroutine(WaitForNewLine());
                    break;
                }
                else
                {
                    showingLineBool = true;
                    yield return new WaitForSecondsRealtime(tipingTime);
                    tmProDialogue.text += ch;
                }
            }

            endLine.Invoke(); // En donde sea necesario, se pone el listener a este evento (endLine.AddListener(metodoNecesario)) y el código para poder terminar cuando queramos, por ejemplo.
            yield return StartCoroutine(WaitForNewLine()); 
            index++;
        }

        endDialogue.Invoke();
        showingLine = null;
    }

    //private void Update()
    //{
    //    if (nextLineInput.triggered)
    //    {
    //        if (showingLineBool && !inputBuffer)
    //        {
    //            skipLine = true;
    //        }

    //        inputBuffer = false; // Consume el input
    //    }
    //}


    /// <summary>
    /// Corrutina para pasar a la siguiente línea.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForNewLine()
    {
        if (inputUsedForSkip)
        {
            // Hemos usado ya el input, así que no esperamos otro
            inputUsedForSkip = false;

            // También esperamos a que el jugador suelte el botón
            while (nextLineInput.ReadValue<float>() > 0f)
            {
                yield return null;
            }

            yield break;
        }

        waitingForInput = true;
        inputPressed = false;

        while (!inputPressed)
        {
            yield return null;
        }

        // Esperamos a que suelte el botón para evitar doble click
        while (nextLineInput.ReadValue<float>() > 0f)
        {
            yield return null;
        }

        waitingForInput = false;
    }



    /// <summary>
    /// Método para obtener las líneas de la tabla correspondiente con la key adecuada.
    /// Para facilitar el uso, en las tablas de localización juntar todas las línias de un mismo diálogo con la misma clave 
    /// y separar mediante .1, .2, etc.
    /// </summary>
    /// <param name="tableName">Nombre de la tabla.</param>
    /// <param name="dialogueKey">Key de la tabla que se quiere mostrar. (Antes del .num)</param>
    /// <returns></returns>
    public List<string> GetLinesWithKey(string tableName, string dialogueKey)
    {
        List<string> lines = new List<string>();

        StringTable table = LocalizationSettings.StringDatabase.GetTable(tableName);

        if (table != null)
        {
            int numLines = 0;
            foreach (var entry in table)
            {
                if (entry.Value.Key.ToString().Contains(dialogueKey))
                {
                    numLines++;
                }
            }

            for (int i = 0; i <= numLines; i++)
            {
                foreach (var entry in table)
                {
                    if (entry.Value.Key.ToString().Equals(dialogueKey + "." + i))
                    {
                        //Debug.Log(entry.Value.LocalizedValue);
                        lines.Add(entry.Value.LocalizedValue);
                    }
                }
            }
        }
        return lines;
    }
}