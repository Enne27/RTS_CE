#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Localization.Components;

/// <summary>
/// Tool que nos añade directamente el componente localizeStringEvent a los TMP de una escena, tanto activos como inactivos.
/// </summary>
public class AddLocalizeStringEvent : MonoBehaviour
{
    [MenuItem("Tools/Localization/Add localizeString")]
    /// <summary>
    /// Busca los TMP, inactivos y activos en escena. 
    /// Añade el componente localizedStringEvent si no lo tiene. 
    /// Limpia los listeners que tuvieran y añade al onUpdateString el texto para que se actualize.
    /// </summary>
    public static void AddLocalizeStringToTMP()
    {
        TMP_Text[] texts = FindObjectsByType<TMP_Text>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (TMP_Text tmp in texts)
        {
            LocalizeStringEvent stringEvent = tmp.GetComponent<LocalizeStringEvent>();

            // Añadimos un localizeStringEvent si no lo tenía.
            if (stringEvent == null)
               stringEvent = Undo.AddComponent<LocalizeStringEvent>(tmp.gameObject);


            // Limpiar listeners, tanto persistentes como no, no queremos duplicados.
            stringEvent.OnUpdateString.RemoveAllListeners();
            int count = stringEvent.OnUpdateString.GetPersistentEventCount();
            for (int i = count - 1; i >= 0; i--)
            {
                UnityEventTools.RemovePersistentListener(stringEvent.OnUpdateString, i);
            }

            // Procedemos a introducir su propio TMP para actualizar el valor.
            UnityEventTools.AddPersistentListener(
                stringEvent.OnUpdateString,
                tmp.SetText // en principio pone el text
            );

            EditorUtility.SetDirty(stringEvent);
        }
    }
}
#endif