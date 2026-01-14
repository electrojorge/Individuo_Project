using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    [Header("Input System")]
    [Tooltip("Asigna aquí la Input Action de tipo Vector2 (ej: 'Move') desde tu asset .inputactions")]
    [SerializeField] private InputActionReference moveActionReference;

    [Header("Cámara")]
    [Tooltip("Transform de la cámara. Si está vacío se usa Camera.main")]
    [SerializeField] private Transform cameraTransform;

    [Header("Movimiento")]
    [Tooltip("Velocidad en unidades por segundo")]
    [SerializeField] private float speed = 5f;

    [Tooltip("Velocidad de rotación al mirar la dirección de movimiento")]
    [SerializeField] private float rotationSpeed = 10f;

    [Tooltip("Usar Rigidbody si está presente (mejor para física). Si no, se usa Transform.Translate.")]
    [SerializeField] private bool preferRigidbody = true;

    private Rigidbody rb;
    private Vector3 inputDirection = Vector3.zero; // dirección EN MUNDO

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) preferRigidbody = false;
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void OnEnable()
    {
        if (moveActionReference != null && moveActionReference.action != null)
            moveActionReference.action.Enable();
    }

    void OnDisable()
    {
        if (moveActionReference != null && moveActionReference.action != null)
            moveActionReference.action.Disable();
    }

    void Update()
    {
        inputDirection = ReadInputRelativeToCamera();

        if (!preferRigidbody)
        {
            Vector3 move = inputDirection.normalized * speed * Time.deltaTime;
            transform.Translate(move, Space.World);

            if (inputDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(inputDirection.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
    }

    void FixedUpdate()
    {
        if (preferRigidbody && rb != null)
        {
            Vector3 move = inputDirection.normalized * speed * Time.fixedDeltaTime;
            Vector3 targetPos = rb.position + move;
            rb.MovePosition(targetPos);

            if (inputDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(inputDirection.normalized, Vector3.up);
                Quaternion newRot = Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
                rb.MoveRotation(newRot);
            }
        }
    }

    // Lee la input action (Vector2) y la convierte en una dirección en mundo relativa a la cámara
    private Vector3 ReadInputRelativeToCamera()
    {
        Vector2 raw = Vector2.zero;

        if (moveActionReference != null && moveActionReference.action != null)
        {
            raw = moveActionReference.action.ReadValue<Vector2>();
        }
        else if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) raw.x = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) raw.x = 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) raw.y = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) raw.y = -1f;
        }

        // Construimos la dirección local (x = izquierda/derecha, z = adelante/atrás)
        Vector3 inputLocal = new Vector3(raw.x, 0f, raw.y);

        // Si no hay entrada, devolvemos cero
        if (inputLocal.sqrMagnitude < 0.0001f) return Vector3.zero;

        // Obtener referencia de cámara
        Transform cam = cameraTransform;
        if (cam == null && Camera.main != null) cam = Camera.main.transform;

        if (cam == null)
        {
            // Sin cámara, movimiento en ejes globales
            return inputLocal.normalized;
        }

        // Proyectar forward y right de la cámara en el plano XZ
        Vector3 camForward = cam.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cam.right;
        camRight.y = 0f;
        camRight.Normalize();

        // worldDir = forward * z + right * x
        Vector3 worldDir = camForward * inputLocal.z + camRight * inputLocal.x;
        if (worldDir.sqrMagnitude > 1f) worldDir.Normalize();
        return worldDir;
    }

    void OnValidate()
    {
        if (speed < 0f) speed = 0f;
        if (rotationSpeed < 0f) rotationSpeed = 0f;
    }
}