using System.Collections;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Floating_HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    [SerializeField] float changeSpeed = 2f;
    float tV;
    float cV;

    [SerializeField] private TextMeshProUGUI eventDamage;
    [SerializeField] private GameObject selector;

    private void Awake()
    {
        if (eventDamage != null) eventDamage.gameObject.SetActive(false);
        if (selector != null) selector.SetActive(false);
    }

    private void Update()
    {
        cV = Mathf.MoveTowards(cV, tV, changeSpeed * Time.deltaTime);
        slider.value = cV;
        UpdateColor();
    }
    public void Init(float currentValue, float maxValue)
    {
        slider.maxValue = 1f;
        float normalized = currentValue / maxValue;
        this.cV = normalized;
        tV = normalized;
        slider.value = normalized;
        UpdateColor();
    }
    public void ShowSelector()
    {
        if (selector != null) selector.SetActive(true);
    }

    public void Deselect()
    {
        if (selector != null) selector.SetActive(false);
    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        tV = currentValue / maxValue;
    }

    void UpdateColor()
    {
        float t = slider.value;
        Color color;
        if (t > 0.5f)
        {
            color = Color.Lerp(Color.yellow, Color.green, (t - 0.5f) * 2);
        }
        else
        {
            color = Color.Lerp(Color.red, Color.yellow, t * 2);
        }
        fillImage.color = color;
    }

    public void ShowNumberEvent(int amount, bool isHeal)
    {
        if (eventDamage == null) return;

        StopAllCoroutines();

        eventDamage.gameObject.SetActive(true);

        if (isHeal)
        {
            eventDamage.color = Color.green;
            eventDamage.text = "+" + amount.ToString();
        }
        else
        {
            eventDamage.color = Color.red;
            eventDamage.text = "-" + amount.ToString();
        }

        StartCoroutine(HideEvent());
    }

    private IEnumerator HideEvent()
    {
        RectTransform rect = eventDamage.rectTransform;

        Vector3 startPos = rect.localPosition;
        Vector3 endPos = startPos + new Vector3(0, 40f, 0);

        float duration = 0.9f;
        float time = 0;

        Color startColor = eventDamage.color;
        Color fadeColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        // POP inicial
        rect.localScale = Vector3.one * 1.6f;

        while (time < duration)
        {
            float t = time / duration;

            // subir
            rect.localPosition = Vector3.Lerp(startPos, endPos, t);

            // fade
            eventDamage.color = Color.Lerp(startColor, fadeColor, t);

            // volver a tamaño normal
            rect.localScale = Vector3.Lerp(Vector3.one * 1.6f, Vector3.one, t);

            time += Time.deltaTime;
            yield return null;
        }

        rect.localPosition = startPos;
        rect.localScale = Vector3.one;
        eventDamage.gameObject.SetActive(false);
    }

}
