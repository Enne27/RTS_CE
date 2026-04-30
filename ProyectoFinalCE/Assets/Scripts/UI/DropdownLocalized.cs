using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections;
using System.Collections.Generic;

public class DropdownLocalized : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public string tableReference;

    private List<string> keys = new List<string>();

    void Awake()
    {
        foreach (var option in dropdown.options)
        {
            keys.Add(option.text);
        }
    }

    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        StartCoroutine(UpdateDropdown());
    }

    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    void OnLocaleChanged(Locale locale)
    {
        StartCoroutine(UpdateDropdown());
    }

    IEnumerator UpdateDropdown()
    {
        int selectedIndex = dropdown.value;

        dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();

        foreach (var key in keys)
        {
            var localizedString = new LocalizedString
            {
                TableReference = tableReference,
                TableEntryReference = key
            };

            var handle = localizedString.GetLocalizedStringAsync();
            yield return handle;

            newOptions.Add(new TMP_Dropdown.OptionData(handle.Result));
        }

        dropdown.AddOptions(newOptions);

        dropdown.value = selectedIndex;
        dropdown.RefreshShownValue();
    }
}