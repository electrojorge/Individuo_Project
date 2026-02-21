using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door_Blocked : MonoBehaviour
{
    [Header("Door settings")]
    // Número de la puerta, para verificar si el jugador tiene la llave
    public int doorNumber = 0;
    // Animator para reproducir la animación
    public Animator doorAnimator;
    // Nombre del trigger en el Animator para abrir la puerta
    public string openTriggerName = "Open";
    // Estado local
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

    //Abre la puerta si el jugador tiene la llave correcta.
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

        // Comprueba el inventario del jugador
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

    // Ejecuta la animación y desactiva el collider para abrir (provisional)
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

    // Comprueba el estado de la puerta (abierta o cerrada)
    public bool IsOpen()
    {
        return isOpen;
    }

    private static KeyInventory cachedInventory;

    // Accede al KeyInventory.
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

    // Método público para dar una llave al jugador (se usará proximamente desde KeyDrop)
    public static void GivePlayerKey(Key_SO key)
    {
        if (key == null)
        {
            Debug.LogWarning("No hay llave");
            return;
        }
        var inv = GetInventory();
        if (inv != null)
        {
            inv.AddKey(key.keyID);
            Debug.Log($"Llave añadida: {key.keyID}");
        }
    }

    //Devuelve que el bool es true si el jugador tiene la llave con el mismo ID.
    public static bool PlayerHasKey(int id)
    {
        var inv = GetInventory();
        return inv != null && inv.HasKey(id);
    }

    // Devuelve un array con los IDs de las llaves que el jugador tiene actualmente.
    public static int[] GetPlayerKeys()
    {
        var inv = GetInventory();
        return inv != null ? inv.GetKeys() : new int[0];
    }
}
