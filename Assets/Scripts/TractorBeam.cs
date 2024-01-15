using UnityEngine;
using UnityEngine.UI;

public class TractorBeam : MonoBehaviour
{
    public Slider slider;
    public Image sliderFill;

    public void SetMaxTractorBeam(int energy)
    {
        slider.maxValue = energy;
    }

    public void SetTractorBeam(int energy)
    {
        slider.value = energy;
    }
}
