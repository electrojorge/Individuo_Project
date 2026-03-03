using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class Floating_HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void Init(float currentValue, float maxValue)
    {
        slider.maxValue = 1f;
        UpdateHealthBar(currentValue, maxValue);
    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        if (maxValue <= 0f)
        {
            slider.value = 0f;
            return;
        }

        slider.value = currentValue / maxValue;
    }
}
