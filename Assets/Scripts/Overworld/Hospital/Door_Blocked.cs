using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door_Blocked : MonoBehaviour
{
    [Header("Door settings")]
    public int doorNumber = 0;
    public Animator doorAnimator;
    public Animator playerAnimator;
    public string openTriggerName = "Open";
    private bool isOpen = false;
    private GameObject doorObject;
    private BoxCollider doorCollider;

    void Awake()
    {
        doorObject = this.gameObject;
        doorCollider = GetComponent<BoxCollider>();
        doorAnimator = GetComponent<Animator>();
        PlayerController PC = Object.FindFirstObjectByType<PlayerController>();
        PC.GetComponent<Animator>();
        playerAnimator = PC.animator;
    }

    // Intenta abrir la puerta, verificando si ya está abierta o si el jugador tiene la llave necesaria
    public void OpenDoor()
    {
        playerAnimator.SetTrigger("TryOpen");
        // Verifica si la puerta ya está abierta
        if (isOpen)
        {
            Debug.Log("La puerta ya está abierta.");
            return;
        }

        if (PlayerHasKey(4))
        {
            SceneManager.LoadScene(5);
        }

        // Verifica si el jugador tiene la llave necesaria para abrir la puerta
        if (PlayerHasKey(doorNumber))
        {
            Debug.Log($"Se tiene la llave {doorNumber}, abriendo puerta.");
            DoOpen();
            doorAnimator.SetTrigger(openTriggerName);
        }
        // Si el jugador no tiene la llave, mostrar un mensaje indicando que la puerta está cerrada
        else
        {
            Debug.Log($"Puerta cerrada, necesitas la llave {doorNumber} para abrirla.");
        }
    }

    // Abre la puerta, activando la animación o desactivando el objeto de la puerta, y deshabilitando el collider para permitir el paso
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

    // Verifica si la puerta está abierta
    public bool IsOpen()
    {
        return isOpen;
    }

    private static KeyInventory cachedInventory;

    //Obtiene el inventario de llaves del jugador
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

    //Interactua con el inventario de llaves del jugador para dar llaves al jugador desde otros scripts
    public static void GivePlayerKey(int id)
    {
        var inv = GetInventory();
        if (inv != null)
        {
            inv.AddKey(id);
            Debug.Log($"Llave añadida: {id}");
        }
    }

    // Verifica si el jugador tiene la llave con el ID especificado
    public static bool PlayerHasKey(int id)
    {
        var inv = GetInventory();
        return inv != null && inv.HasKey(id);
    }

    // Devuelve un array con los IDs de las llaves que el jugador tiene actualmente (no se usa pero )
    public static int[] GetPlayerKeys()
    {
        var inv = GetInventory();
        return inv != null ? inv.GetKeys() : new int[0];
    }
}
