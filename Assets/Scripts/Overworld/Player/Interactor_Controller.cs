using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

public class Interactor_Controller : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI playerUIText; // Texto del HUD
    [SerializeField] private GameObject playerUIPanel;     // Panel del HUD (opcional)
    [SerializeField] private string message = "Pulsa E";

    [Header("World Indicator")]
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private Vector3 indicatorLocalOffset = new Vector3(0f, 0.5f, 1f);
    [SerializeField] private bool indicatorFaceCamera = true;

    [Header("Input System")]
    [SerializeField] private InputActionReference interactActionReference;

    private string playerTag = "Player";
    public UnityEvent onInteract;

    private bool playerInRange;
    private Camera mainCamera;

    // Acción en tiempo de ejecución (puede venir de la referencia o crearse)
    private InputAction runtimeInteractAction;
    private bool usingReference => interactActionReference != null && interactActionReference.action != null;

    // Instancia del indicador en mundo
    private GameObject indicatorInstance;
    private Vector3 indicatorPrefabLocalScale = Vector3.one;

    void Start()
    {
        mainCamera = Camera.main;

        // Preparar HUD
        if (playerUIText != null)
        {
            playerUIText.text = message;
            if (playerUIPanel != null)
                playerUIPanel.SetActive(false);
            else
                playerUIText.gameObject.SetActive(false);
        }

        // Guardar escala original del prefab
        if (indicatorPrefab != null)
            indicatorPrefabLocalScale = indicatorPrefab.transform.localScale;

        // Instanciar indicador en el mundo (desactivado)
        if (indicatorPrefab != null)
        {
            indicatorInstance = Instantiate(indicatorPrefab);
            indicatorInstance.SetActive(false);
            indicatorInstance.transform.localScale = indicatorPrefabLocalScale;
            UpdateIndicatorTransform();
        }
    }

    void OnEnable()
    {
        // Preparar acción de interacción
        if (usingReference)
        {
            // Usar la referencia proporcionada en el inspector (si existe)
            runtimeInteractAction = interactActionReference.action;
        }
        else
        {
            // Si no hay referencia, crear una acción por defecto.
            // Si este GameObject tiene un EnemyPatrol_Controller, usamos click izquierdo como binding.
            if (runtimeInteractAction == null)
            {
                var enemy = GetComponent<EnemyPatrol_Controller>();
                if (enemy != null)
                {
                    // Enemigo: interacción = atacar -> click izquierdo del ratón (y gamepad botón Sur)
                    runtimeInteractAction = new InputAction("Interact", InputActionType.Button, "<Mouse>/leftButton");
                    runtimeInteractAction.AddBinding("<Gamepad>/buttonSouth");
                }
                else
                {
                    // No-enemigo: uso por defecto (teclado E + gamepad)
                    runtimeInteractAction = new InputAction("Interact", InputActionType.Button, "<Keyboard>/e");
                    runtimeInteractAction.AddBinding("<Gamepad>/buttonSouth");
                }
            }
        }

        if (runtimeInteractAction != null)
        {
            runtimeInteractAction.performed += OnInteractPerformed;
            runtimeInteractAction.Enable();
        }
    }

    void OnDisable()
    {
        if (runtimeInteractAction != null)
        {
            runtimeInteractAction.performed -= OnInteractPerformed;
            if (!usingReference)
                runtimeInteractAction.Disable();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInRange = true;
        ShowUI(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInRange = false;
        ShowUI(false);
    }

    void Update()
    {
        // Actualiza indicador si está activo
        if (indicatorInstance != null && indicatorInstance.activeSelf)
        {
            UpdateIndicatorTransform();
            if (indicatorFaceCamera && mainCamera != null)
            {
                // Hacer que mire a la cámara
                Vector3 dir = mainCamera.transform.position - indicatorInstance.transform.position;
                if (dir.sqrMagnitude > 0.0001f)
                    indicatorInstance.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            }
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        // Ejecutar evento si el jugador está en rango
        if (playerInRange)
        {
            onInteract?.Invoke();
        }
    }

    private void ShowUI(bool show)
    {
        // Mostrar/ocultar HUD
        if (playerUIText != null)
        {
            playerUIText.text = message;
            if (playerUIPanel != null)
                playerUIPanel.SetActive(show);
            else
                playerUIText.gameObject.SetActive(show);
        }

        // Mostrar/ocultar indicador en mundo
        if (indicatorInstance != null)
        {
            indicatorInstance.SetActive(show);
            if (show) UpdateIndicatorTransform();
        }
    }

    private void UpdateIndicatorTransform()
    {
        if (indicatorInstance == null) return;
        // Posicionar frente al objeto y mantener escala original
        indicatorInstance.transform.position = transform.TransformPoint(indicatorLocalOffset);
        indicatorInstance.transform.localScale = indicatorPrefabLocalScale;
    }
}
