using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections;

/// <summary>
/// Script para traducir las opciones del dropdown quality, ubicado en settings
/// </summary>
public class QualityDropdownLocalize : MonoBehaviour
{
    #region VARIABLES
    public TMP_Dropdown dropdown;

    [Header("Keys en la tabla DropdownQuality")]
    public string[] keys =
    {
        "DropdownQuality.VeryHigh",
        "DropdownQuality.High",
        "DropdownQuality.Medium",
        "DropdownQuality.Low",
        "DropdownQuality.Custom"
    };
    #endregion

    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        StartCoroutine(UpdateDropdown());
    }

    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        StartCoroutine(UpdateDropdown());
    }

    IEnumerator UpdateDropdown()
    {
        dropdown.options.Clear();

        foreach (var key in keys)
        {
            var localizedString = new LocalizedString
            {
                TableReference = "DropdownQuality",
                TableEntryReference = key
            };

            var handle = localizedString.GetLocalizedStringAsync();
            yield return handle;

            dropdown.options.Add(new TMP_Dropdown.OptionData(handle.Result));
        }

        dropdown.RefreshShownValue();
    }
}