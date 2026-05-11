using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Clase serializable para remapear las acciones. Incluye: 
///     - Action del inputSystem.
///     - Array de índices de inputs específicos a ignorar si no queremos que se remapeen. 
///     - Botón que activará la escucha de remapeo.
///     - Texto que muestra la tecla actual mapeada.
/// </summary>
[System.Serializable]
public class RebindConfig
{
    [Tooltip("Action del inputSystem a remapear")]
    public InputActionReference action;

    [Tooltip("Bindings que nunca se pueden remapear")]
    public int[] fixedBindingIndexes;

    [Tooltip("Botón de remapeo de la action")]
    public Button rebindButton;
    
    [Tooltip("Texto del input actual de action")]
    public TextMeshProUGUI bindingText;
}

/// <summary>
/// Enum que indica qué dispositivo se está usando para hacer el mapeo.
/// En el input System se tiene que indicar el grupo al que pertenece para que funcione.
/// </summary>
public enum ControlScheme
{
    KeyboardMouse,
    KeyboardOnly,
    Gamepad
}

public class RebindManager : MonoBehaviour
{
    #region SINGLETON

    public static RebindManager Instance;
    private void Awake()
    {
        if (Instance == null)  Instance = this;
    }

    #endregion

    #region VARIABLES

    [SerializeField] InputActionAsset inputAsset;

    [Header("Rebinds config")]
    [Tooltip("Lista que contiene todas las clases rebindConfig de las acciones que vamos a mapear.")]
    public List<RebindConfig> rebinds;

    #endregion

    /// <summary>
    /// Comprueba si una tecla o botón ya está usada.
    /// Recorre todas las acciones configuradas en nuestra lista mapeable y todos los bindings de cada acción.
    /// Ignora la acción actual que está siendo mapeada.
    /// Ignora composites.
    /// </summary>
    /// <param name="newPath">Nuevo bind para hacer rebind.</param>
    /// <param name="ignoreAction">Action siendo mapeada.</param>
    /// <param name="ignoreIndex">índice de la acción siendo mapeada.</param>
    /// <returns>True si hay conflicto entre teclas y false si no lo hay.</returns>
    public bool IsBindingUsed(string newPath, InputAction ignoreAction, int ignoreIndex)
    {
        foreach (var config in rebinds)
        {
            var action = config.action.action;

            for (int i = 0; i < action.bindings.Count; i++)
            {
                var b = action.bindings[i];

                if (action == ignoreAction && i == ignoreIndex)
                    continue;

                if (b.isComposite || b.isPartOfComposite)
                    continue;

                if (b.effectivePath == newPath)
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Encuentra el binding que se puede modificar siguiendo nuestras reglas:
    ///     - Dispositivo actual.
    ///     - Bindings fijos que no pueden modificarse.
    /// Recorre todas las acciones y sus bindings, buscando uno disponible para hacer el mapeo.
    /// Si no encuentra ninguno, devuelve negativo.
    /// </summary>
    /// <param name="action">Action a remapear.</param>
    /// <param name="scheme">Esquema de control actual.</param>
    /// <param name="config">Clase serializable con los datos de esa action y sus binds.</param>
    /// <returns>-1 si no hay binding disponible a mapear o el índice del bind disponible si existe.</returns>
    public int GetRebindableBindingIndex(InputAction action, ControlScheme scheme, RebindConfig config)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            var b = action.bindings[i];

            if (b.isComposite || b.isPartOfComposite)
                continue;

            if (IsFixed(config, i))
                continue;

            if (!MatchesScheme(b, scheme))
                continue;

            return i;
        }

        return -1;
    }

    /// <summary>
    /// Recorre los índices bloqueados de esa action y los compara con el indicado desde parámetro.
    /// </summary>
    /// <param name="config">Clase con los parámetros del rebind.</param>
    /// <param name="index">índice que comprueba.</param>
    /// <returns>True si ese índice está bloqueado y false si no lo está.</returns>
    bool IsFixed(RebindConfig config, int index)
    {
        if (config.fixedBindingIndexes == null)
            return false;

        foreach (var f in config.fixedBindingIndexes)
            if (f == index)
                return true;

        return false;
    }

    /// <summary>
    /// Comprueba si un binding pertenece al dispositivo actual de controles.
    /// Debe estar bien configurado el grupo desde el input system.
    /// </summary>
    /// <param name="binding">Bind a revisar.</param>
    /// <param name="scheme">Esquema de control de dispositivo.</param>
    /// <returns>False si no pertenece, true si se corresponde con el dispositivo.</returns>
    bool MatchesScheme(InputBinding binding, ControlScheme scheme)
    {
        string bindingGroups = binding.groups;

        switch (scheme)
        {
            case ControlScheme.KeyboardMouse:
                return bindingGroups.Contains("Keyboard") || bindingGroups.Contains("Mouse");

            case ControlScheme.KeyboardOnly:
                return bindingGroups.Contains("Keyboard");

            case ControlScheme.Gamepad:
                return bindingGroups.Contains("Gamepad");
        }

        return false;
    }

    #region SAVE / LOAD

    /// <summary>
    /// Guardar el nuevo mapping si ha habido cambios.
    /// </summary>
    public void SaveBindings()
    {
        string json = inputAsset.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString("rebinds", json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Cargar el mapping anterior.
    /// </summary>
    public void LoadBindings()
    {
        if (!PlayerPrefs.HasKey("rebinds"))
            return;

        inputAsset.LoadBindingOverridesFromJson(PlayerPrefs.GetString("rebinds"));
    }

    #endregion
}