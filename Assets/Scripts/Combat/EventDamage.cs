using TMPro;
using UnityEngine;

public class EventDamage : MonoBehaviour
{
    public TextMeshProUGUI text;
    float life = 1f;

    void Update()
    {
        transform.position += Vector3.up * 20f * Time.deltaTime;

        life -= Time.deltaTime;
        if (life <= 0)
            Destroy(gameObject);
    }

    public void Set(int amount, bool heal)
    {
        text.text = heal ? "+" + amount : "-" + amount;
        text.color = heal ? Color.green : Color.red;
    }
}
