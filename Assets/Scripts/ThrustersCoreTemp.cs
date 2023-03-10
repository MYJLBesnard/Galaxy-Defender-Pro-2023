using UnityEngine;
using UnityEngine.UI;

public class ThrustersCoreTemp : MonoBehaviour
{
    public Slider Slider;
    public Image SliderFill;


    public void Update()
    {
        SliderFill.color = Color.Lerp(Color.green, Color.red, Slider.value / Slider.maxValue);
    }

    public void SetMaxCoreTemp(int temp)
    {
        Slider.maxValue = temp;
    }

    public void SetCoreTemp(int temp)
    {
        Slider.value = temp;
    }
}
