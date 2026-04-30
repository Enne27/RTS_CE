using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections.Generic;

public class DropdownLocalized : MonoBehaviour
{
    #region VARIABLES
    public TMP_Dropdown dropdown;
    public string tableReference;

    private List<LocalizedString> localizedStrings = new List<LocalizedString>();
    #endregion

    void Awake()
    {
        localizedStrings.Clear();
        InitializeLocalization();
    }

    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        OnLocaleChanged(LocalizationSettings.SelectedLocale);
    }

    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    /// <summary>
    /// Crea los localized strings en la lista por cada valor del dropdown y se subscribe a los cambios de 
    /// esos strings para actualizarlos.
    /// </summary>
    private void InitializeLocalization()
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            string key = dropdown.options[i].text;

            var localizedString = new LocalizedString
            {
                TableReference = tableReference,
                TableEntryReference = key
            };

            int index = i;

            localizedString.StringChanged += (value) =>
            {
                if (index < dropdown.options.Count)
                {
                    dropdown.options[index].text = value;

                    if (dropdown.value == index)
                        dropdown.captionText.text = value;
                }
            };

            localizedStrings.Add(localizedString);
        }
    }

    /// <summary>
    /// Cambiar el texto del dropdown al cambiar el idioma.
    /// </summary>
    /// <param name="locale">idioma selecionado</param>
    void OnLocaleChanged(Locale locale)
    {
        foreach (var localized in localizedStrings)
        {
            localized.RefreshString();
        }

        dropdown.RefreshShownValue();
    }
}