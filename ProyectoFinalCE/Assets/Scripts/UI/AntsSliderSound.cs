using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

/// <summary>
/// Componente para detectar el valor del Slider de audio y mostrarlo en el slider de hormigas
/// </summary>
public class AntsSliderSound : MonoBehaviour
{
    public Slider slider;
    public Image fillImage;

    void OnEnable()
    {
        slider.onValueChanged.AddListener(UpdateFillValue);
        UpdateFillValue(slider.value); 
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(UpdateFillValue);
    }

    void UpdateFillValue(float value)
    {
        fillImage.fillAmount = slider.normalizedValue;
    }
}
