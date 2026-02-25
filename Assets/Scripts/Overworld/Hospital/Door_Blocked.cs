using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door_Blocked : MonoBehaviour
{
    [Header("Door settings")]
    public int doorNumber = 0;
    public Animator doorAnimator;
    public string openTriggerName = "Open";
    private bool isOpen = false;
    private GameObject doorObject;
    private BoxCollider doorCollider;

    void Awake()
    {
        doorObject = this.gameObject;
        doorCollider = GetComponent<BoxCollider>();
        
        if (doorCollider == null)
        {
            Debug.LogWarning($"No hay BoxCollider en'{gameObject.name}'");
        }
    }

    public void OpenDoor()
    {
        if (isOpen)
        {
            Debug.Log("La puerta ya está abierta.");
            return;
        }

        if (doorNumber <= 0)
        {
            Debug.LogWarning("DoorNumber no está configurado o es 0. La puerta se abrirá de todos modos.");
            DoOpen();
            return;
        }

        if (PlayerHasKey(doorNumber))
        {
            Debug.Log($"Se tiene la llave {doorNumber}, abriendo puerta.");
            DoOpen();
        }
        else
        {
            Debug.Log($"Puerta cerrada, necesitas la llave {doorNumber} para abrirla.");
        }
    }

    private void DoOpen()
    {
        isOpen = true;

        if (doorAnimator != null && !string.IsNullOrEmpty(openTriggerName))
        {
            doorAnimator.SetTrigger(openTriggerName);
        }
        else
        {
            if (doorObject != null)
                doorObject.SetActive(false);
        }

        if (doorCollider != null)
            doorCollider.enabled = false;
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    private static KeyInventory cachedInventory;

    private static KeyInventory GetInventory()
    {
        if (cachedInventory == null)
        {
            cachedInventory = Object.FindFirstObjectByType<KeyInventory>();
            if (cachedInventory == null)
                Debug.LogWarning("Door_Blocked: no se encontró KeyInventory en la escena.");
        }
        return cachedInventory;
    }

    public static void GivePlayerKey(int id)
    {
        var inv = GetInventory();
        if (inv != null)
        {
            inv.AddKey(id);
            Debug.Log($"Llave añadida: {id}");
        }
    }

    public static bool PlayerHasKey(int id)
    {
        var inv = GetInventory();
        return inv != null && inv.HasKey(id);
    }

    public static int[] GetPlayerKeys()
    {
        var inv = GetInventory();
        return inv != null ? inv.GetKeys() : new int[0];
    }
}
