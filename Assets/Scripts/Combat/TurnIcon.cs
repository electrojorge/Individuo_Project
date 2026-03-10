using UnityEngine;
using UnityEngine.UI;

public class TurnIcon : MonoBehaviour
{
    public Image potrait;
    public Image frame;

    public void SetUnit(Unit unit)
    {
        potrait.sprite = unit.unitPotrait;
    }
    public void SetActiveTurn(bool active)
    {
        frame.enabled = active;
        transform.localScale = active ? Vector3.one * 1.15f : Vector3.one;
    }
}
