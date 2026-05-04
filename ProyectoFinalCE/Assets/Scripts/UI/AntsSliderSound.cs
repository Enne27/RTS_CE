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

    void Start()
    {
        slider.onValueChanged.AddListener(UpdateFillValue);
        UpdateFillValue(slider.value); 
    }

    void UpdateFillValue(float value)
    {
        fillImage.fillAmount = slider.normalizedValue;
    }
}
