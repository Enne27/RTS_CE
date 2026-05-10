using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.UI;

public class RebindUIView : View
{
    #region VARIABLES

    [Header("UI")]
    [SerializeField] Button backButton;

    [Tooltip("Dropdown de settings que maneja qué inputs tenemos actualmente. (selector)")]
    [SerializeField] TMP_Dropdown controlSchemeDropdown;


    [Tooltip("Proceso activo de rebind o remap. (La escucha de teclas)")]
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    [Tooltip("Action a la que estamos haciendo rebind actualmente.")]
    private RebindConfig currentRebind;


    [Tooltip("Estado actual del dropdown.")]
    private ControlScheme currentScheme = ControlScheme.KeyboardMouse;


    [Header("Localization")]
    [Tooltip("Texto que aparece mientras se ejecuta la escucha del rebind.")]
    [SerializeField] LocalizedString rebindText_ls;

    #endregion


    #region INITIALIZE

    public override void Initialize()
    {
        RebindManager.Instance.LoadBindings();
        SetupDropdown();

        foreach (var config in RebindManager.Instance.rebinds)
            SetupItem(config);

        RefreshAllUI();

        backButton.onClick.AddListener(() => ViewManager.ShowLastView(1, false));
    }

    /// <summary>
    /// Prepara los valores del dropdown de dispositivo de controles y rebinds.
    /// </summary>
    void SetupDropdown()
    {
        controlSchemeDropdown.ClearOptions();

        controlSchemeDropdown.AddOptions(new List<string>
        {
            "Mouse + Keyboard",
            "Keyboard",
            "Gamepad"
        });

        controlSchemeDropdown.onValueChanged.AddListener(OnSchemeChanged);
    }

    /// <summary>
    /// Actualiza el esquema de controles actual y refresca la UI.
    /// </summary>
    /// <param name="index"></param>
    void OnSchemeChanged(int index)
    {
        currentScheme = (ControlScheme)index;
        RefreshAllUI();
    }

    #endregion


    #region UI

    /// <summary>
    /// Conecta los botones con su lógica.
    /// Si se pulsa el botón y estaba a la escucha, cancela el rebind. 
    /// Si no, al contario, lo empieza.
    /// </summary>
    /// <param name="config"></param>
    void SetupItem(RebindConfig config)
    {
        config.rebindButton.onClick.AddListener(() =>
        {
            if (currentRebind == config)
                CancelRebind();
            else
                StartRebind(config);
        });
    }

    /// <summary>
    /// Refresca TODA la interfaz.
    /// </summary>
    void RefreshAllUI()
    {
        foreach (var config in RebindManager.Instance.rebinds)
            UpdateUI(config);
    }

    /// <summary>
    /// Actualiza el texto del rebind. Si no hay ningún binding disponible para ese control, muestra: -.
    /// Cuando se realiza el rebind, pone el nombre de la tecla en English.
    /// </summary>
    /// <param name="config"></param>
    void UpdateUI(RebindConfig config)
    {
        var action = config.action.action;

        int index = RebindManager.Instance.GetRebindableBindingIndex(
            action,
            currentScheme,
            config
        );

        if (index == -1)
        {
            config.bindingText.text = "-";
            return;
        }

        var path = action.bindings[index].effectivePath;

        config.bindingText.text = InputControlPath.ToHumanReadableString
        (
            path,
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );
    }

    #endregion


    #region REBIND

    /// <summary>
    /// Inicia el proceso de remapeo uno a uno.
    /// Obtiene el binding correcto teniendo en cuenta el dispositivo.
    /// Guarda el estado actual, desactiva la action mientras se hace el rebind para evitar dispararla por accidente e
    /// inicia el estado de rebind para escuchar, aplicando las exclusiones que le hemos indicado.
    /// </summary>
    /// <param name="config"></param>
    void StartRebind(RebindConfig config)
    {
        if (currentRebind != null)
            return;

        var action = config.action.action;

        int index = RebindManager.Instance.GetRebindableBindingIndex(
            action,
            currentScheme,
            config
        );

        if (index == -1)
            return;

        currentRebind = config;

        config.bindingText.text = rebindText_ls.GetLocalizedString();

        action.Disable();

        var exclusions = GetExclusions(currentScheme);

        rebindingOperation = action.PerformInteractiveRebinding(index);

        if (!string.IsNullOrEmpty(exclusions))
        {
            rebindingOperation = rebindingOperation.WithControlsExcluding(exclusions);
        }

        rebindingOperation.OnComplete(op => OnRebindComplete(index));

        rebindingOperation.Start();
    }

    /// <summary>
    /// Exclusiones de teclas dependiendo de qué control tiene actualmente el juego.
    /// </summary>
    /// <param name="scheme">Enum indicador del esquema de controles actual.</param>
    /// <returns>Exclusión de teclas indicada en cada esquema de control.</returns>
    string GetExclusions(ControlScheme scheme)
    {
        switch (scheme)
        {
            case ControlScheme.Gamepad:
                return "<Keyboard>,<Mouse>";

            case ControlScheme.KeyboardOnly:
                return "<Mouse>"; // Excluir el ratón si juega sin él.

            case ControlScheme.KeyboardMouse:
                return null; // No excluir el ratón.
        }

        return null;
    }

    /// <summary>
    /// Se ejecuta al pulsar algo.
    /// Obtiene el input real y evita duplicados, cancelando y volviendo al anterior.
    /// </summary>
    /// <param name="bindingIndex"></param>
    void OnRebindComplete(int bindingIndex)
    {
        var action = currentRebind.action.action;

        string newPath = action.bindings[bindingIndex].effectivePath;

        if (RebindManager.Instance.IsBindingUsed(
            newPath,
            action,
            bindingIndex))
        {
            action.RemoveBindingOverride(bindingIndex);
        }

        FinishRebind();
    }

    /// <summary>
    /// Limpia todo. Libera memoria, reactiva el input, guarda los cambios y actualiza la UI, además de liberar el estado de rebind.
    /// </summary>
    void FinishRebind()
    {
        rebindingOperation?.Dispose();

        currentRebind.action.action.Enable();

        RebindManager.Instance.SaveBindings();
        UpdateUI(currentRebind);

        currentRebind = null;
    }

    /// <summary>
    /// Abortar el proceso de rebind. Cancela la escucha, reactiva el input, restaura la UI y devuelve el estado rebind a null.
    /// </summary>
    void CancelRebind()
    {
        rebindingOperation?.Dispose();

        if (currentRebind != null)
        {
            currentRebind.action.action.Enable();
            UpdateUI(currentRebind);
        }

        currentRebind = null;
    }

    #endregion
}