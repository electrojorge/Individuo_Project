using UnityEngine;
using UnityEngine.UI;

public class Floating_HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void UpadeteHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
    }
}
