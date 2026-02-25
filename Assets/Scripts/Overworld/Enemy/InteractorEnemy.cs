using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractorEnemy : MonoBehaviour
{
    [Header("World Indicator")]
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private Vector3 indicatorLocalOffset = new Vector3(0f, 0.5f, 1f);
    [SerializeField] private bool indicatorFaceCamera = true;

    [Header("Input System")]
    [SerializeField] private InputActionReference interactActionReference;

    public UnityEvent onInteract;

    private bool playerInRange;
    private Camera mainCamera;

    // Acción en tiempo de ejecución (puede venir de la referencia o crearse)
    private InputAction runtimeInteractAction;
    private bool usingReference => interactActionReference != null && interactActionReference.action != null;

    // Instancia del indicador en mundo
    private GameObject indicatorInstance;
    private Vector3 indicatorPrefabLocalScale = Vector3.one;

    Player_Controller PC;

    private void Awake()
    {
        PC = FindAnyObjectByType<Player_Controller>();
    }

    void Start()
    {
        mainCamera = Camera.main;

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
            // Usamos click izquierdo como binding.
            if (runtimeInteractAction == null)
            {
                runtimeInteractAction = new InputAction("Interact", InputActionType.Button, "<Mouse>/leftButton");
                runtimeInteractAction.AddBinding("<Gamepad>/buttonSouth");
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

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        // Ejecutar evento si el jugador está en rango
        if (playerInRange)
        {
            PC.AttackEnemy();
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
