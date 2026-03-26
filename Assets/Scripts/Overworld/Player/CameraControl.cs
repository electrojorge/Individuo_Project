using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    private CinemachineInputAxisController axisController;

    void Awake()
    {
        axisController = GetComponent<CinemachineInputAxisController>();
    }

    void Update()
    {
        // Comprobamos si el ratón existe y si el botón derecho (Right Button) NO está presionado
        if (Mouse.current != null && !Mouse.current.rightButton.isPressed)
        {
            // Desactivamos el controlador de ejes para que no procese el movimiento
            axisController.enabled = false;
        }
        else
        {
            // Reactivamos el controlador cuando se pulsa el botón
            axisController.enabled = true;
        }
    }
}
