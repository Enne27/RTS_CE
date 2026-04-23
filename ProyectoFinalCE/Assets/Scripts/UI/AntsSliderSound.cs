using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Componente para detectar el valor del Slider de audio y mostrarlo en el slider de hormigas
/// </summary>
public class AntsSliderSound : MonoBehaviour
{
    public Slider audioSlider;
    public Image fillImage;

    // Update is called once per frame
    void Update()
    {
        fillImage.fillAmount = audioSlider.value;
    }
}
