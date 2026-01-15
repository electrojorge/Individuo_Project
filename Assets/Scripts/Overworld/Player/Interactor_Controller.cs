using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

public class Interactor_Controller : MonoBehaviour
{
    [Header("UI - Player HUD")]
    [Tooltip("Texto en la interfaz del jugador (Canvas Screen Space - TextMeshProUGUI). Se habilita/deshabilita y muestra `message`.")]
    [SerializeField] private TextMeshProUGUI playerUIText;
    [Tooltip("Si asignas el GameObject padre del texto, se activará/desactivará en bloque. Si está vacío se alternará solo el Text.")]
    [SerializeField] private GameObject playerUIPanel;
    [SerializeField] private string message = "Pulsa E";

    [Header("World Indicator (sprite animado)")]
    [Tooltip("Prefab del indicador (debe contener SpriteRenderer y Animator/Animation). Se instanciará delante del objeto.")]
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private Vector3 indicatorLocalOffset = new Vector3(0f, 0.5f, 1f);
    [SerializeField] private bool indicatorFaceCamera = true;

    [Header("Input (Nuevo Input System)")]
    [Tooltip("InputActionReference opcional para la interacción. Si está vacío se usa <Keyboard>/e y <Gamepad>/buttonSouth.")]
    [SerializeField] private InputActionReference interactActionReference;

    [Header("Interaction")]
    [SerializeField] private string playerTag = "Player";
    public UnityEvent onInteract;

    private bool playerInRange;
    private Camera mainCamera;

    // Acción en tiempo de ejecución (puede venir de la referencia o crearse)
    private InputAction runtimeInteractAction;
    private bool usingReference => interactActionReference != null && interactActionReference.action != null;

    // Instancia del indicador en mundo (no parentada para evitar deformaciones por escala del padre)
    private GameObject indicatorInstance;
    private Vector3 indicatorPrefabLocalScale = Vector3.one;

    void Start()
    {
        mainCamera = Camera.main;

        // Preparar player UI (HUD)
        if (playerUIText != null)
        {
            playerUIText.text = message;
            if (playerUIPanel != null)
                playerUIPanel.SetActive(false);
            else
                playerUIText.gameObject.SetActive(false);
        }

        // Guardar escala original del prefab (si existe)
        if (indicatorPrefab != null)
            indicatorPrefabLocalScale = indicatorPrefab.transform.localScale;

        // Instanciar indicador en mundo (desactivado inicialmente)
        if (indicatorPrefab != null)
        {
            // Instanciar sin padre para evitar heredar escala no uniforme del objeto padre
            indicatorInstance = Instantiate(indicatorPrefab);
            indicatorInstance.SetActive(false);
            // Asegurar la escala original del prefab (instanciado sin padre mantiene localScale, pero lo forzamos)
            indicatorInstance.transform.localScale = indicatorPrefabLocalScale;
            UpdateIndicatorTransform();
        }
    }

    void OnEnable()
    {
        if (usingReference)
        {
            runtimeInteractAction = interactActionReference.action;
        }
        else
        {
            if (runtimeInteractAction == null)
            {
                runtimeInteractAction = new InputAction("Interact", InputActionType.Button, "<Keyboard>/e");
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
        // Actualizar indicador en mundo
        if (indicatorInstance != null && indicatorInstance.activeSelf)
        {
            UpdateIndicatorTransform();
            if (indicatorFaceCamera && mainCamera != null)
            {
                // Hacer que el sprite mire a la cámara (billboard)
                Vector3 dir = indicatorInstance.transform.position - mainCamera.transform.position;
                if (dir.sqrMagnitude > 0.0001f)
                    indicatorInstance.transform.rotation = Quaternion.LookRotation(dir);
            }
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        if (playerInRange)
        {
            onInteract?.Invoke();
        }
    }

    private void ShowUI(bool show)
    {
        // UI del jugador (HUD)
        if (playerUIText != null)
        {
            playerUIText.text = message;
            if (playerUIPanel != null)
                playerUIPanel.SetActive(show);
            else
                playerUIText.gameObject.SetActive(show);
        }

        // Indicador en mundo delante del objeto
        if (indicatorInstance != null)
        {
            indicatorInstance.SetActive(show);
            if (show) UpdateIndicatorTransform();
        }
    }

    private void UpdateIndicatorTransform()
    {
        if (indicatorInstance == null) return;
        // Posicionar delante del objeto usando TransformPoint para calcular la posición en world space
        indicatorInstance.transform.position = transform.TransformPoint(indicatorLocalOffset);
        // Mantener la escala original del prefab (evita deformaciones por la escala del objeto padre)
        indicatorInstance.transform.localScale = indicatorPrefabLocalScale;
    }
}
