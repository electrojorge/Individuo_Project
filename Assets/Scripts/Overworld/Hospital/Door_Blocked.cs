using UnityEngine;

public class Door_Blocked : MonoBehaviour
{
    [Header("Door settings")]
    // Número de la puerta, para verificar si el jugador tiene la llave
    public int doorNumber = 0;
    //Animator para reproducir la animación
    public Animator doorAnimator;
    // Nombre del trigger en el Animator para abrir la puerta
    public string openTriggerName = "Open";
    // Estado local
    private bool isOpen = false;
    private GameObject doorObject;
    private BoxCollider doorCollider;

    void Awake()
    {
        // El objeto de la puerta será el propio GameObject que contiene el script
        doorObject = this.gameObject;

        // Intentar obtener el BoxCollider en el mismo GameObject
        doorCollider = GetComponent<BoxCollider>();
        if (doorCollider == null)
        {
            Debug.LogWarning($"Door_Blocked on '{gameObject.name}': no se encontró un BoxCollider en el mismo GameObject.");
        }
    }

    // Llamar desde el interactable (o desde un script de input) para intentar abrir la puerta
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

        //if (HospitalDoor_Controller.HasKey(doorNumber))
        //{
        //    Debug.Log($"Se tiene la llave {doorNumber} -> abriendo puerta.");
        //    DoOpen();
        //}
        else
        {
            Debug.Log($"Puerta cerrada: necesitas la llave {doorNumber} para abrirla.");
        }
    }

    // Ejecuta la animación o desactiva el objeto/collider para abrir
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

    // Método público para comprobar estado
    public bool IsOpen()
    {
        return isOpen;
    }
}
