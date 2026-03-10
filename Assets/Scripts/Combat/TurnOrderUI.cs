using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderUI : MonoBehaviour
{
    public static TurnOrderUI instance;

    public Transform[] slots = new Transform[4];
    public Image[] portraits = new Image[4];

    List<Unit> currentOrder = new List<Unit>();

    void Awake()
    {
        instance = this;
    }

    public void UpdateTurnOrder(List<Unit> players, List<Unit> enemies)
    {
        currentOrder.Clear();

        currentOrder.AddRange(players);
        currentOrder.AddRange(enemies);

        int max = Mathf.Min(currentOrder.Count, 6);

        for (int i = 0; i < portraits.Length; i++)
        {
            if (i < max)
            {
                portraits[i].gameObject.SetActive(true);

                Sprite portrait = currentOrder[i]
                    .unitPrefab
                    .GetComponentInChildren<SpriteRenderer>()
                    .sprite;

                portraits[i].sprite = portrait;
            }
            else
            {
                portraits[i].gameObject.SetActive(false);
            }
        }
    }

    public void RotateTurn()
    {
        if (currentOrder.Count == 0) return;

        Unit first = currentOrder[0];
        currentOrder.RemoveAt(0);
        currentOrder.Add(first);

        UpdateTurnOrderDisplay();
    }

    void UpdateTurnOrderDisplay()
    {
        int max = Mathf.Min(currentOrder.Count, 6);

        for (int i = 0; i < portraits.Length; i++)
        {
            if (i < max)
            {
                portraits[i].gameObject.SetActive(true);

                Sprite portrait = currentOrder[i]
                    .unitPrefab
                    .GetComponentInChildren<SpriteRenderer>()
                    .sprite;

                portraits[i].sprite = portrait;
            }
            else
            {
                portraits[i].gameObject.SetActive(false);
            }
        }
    }
}
