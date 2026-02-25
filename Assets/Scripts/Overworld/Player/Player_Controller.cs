using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player_Controller : MonoBehaviour
{
    public static Player_Controller instance;

    [Header("Input System")]
    [SerializeField] private InputActionReference moveActionReference;

    [Header("Cámara")]
    [SerializeField] private Transform cameraTransform;

    [Header("Movimiento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI enemyUIText; // Texto del HUD
    [SerializeField] private GameObject enemyUIPanel;     // Panel del HUD (opcional)
    [SerializeField] private string message = "Atacar";

    private Rigidbody rb;
    private Vector3 inputDirection = Vector3.zero;
    public bool enemyInRange;
    private string enemyTag = "Enemy";

    public int enemyID;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
    }
    void Start()
    {

        // Obtener o añadir Rigidbody: el controlador siempre usa Rigidbody.
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        // Evitar que la física rote el cuerpo en X/Z — la rotación Y la controla el script.
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    public Rigidbody GetRigidbody() { return rb; }

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
        // Leer input cada frame (se usa luego en FixedUpdate).
        inputDirection = ReadInputRelativeToCamera();
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Movimiento por Rigidbody
        if (inputDirection.sqrMagnitude > 0.0001f)
        {
            Vector3 move = inputDirection.normalized * speed * Time.fixedDeltaTime;
            Vector3 targetPos = rb.position + move;
            rb.MovePosition(targetPos);

            Quaternion targetRot = Quaternion.LookRotation(inputDirection.normalized, Vector3.up);
            Quaternion newRot = Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newRot);
        }
    }

    // Convierte el movimiento en direccion a la cámara.
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

        Vector3 inputLocal = new Vector3(raw.x, 0f, raw.y);
        if (inputLocal.sqrMagnitude < 0.0001f) return Vector3.zero;

        Transform cam = cameraTransform;
        if (cam == null && Camera.main != null) cam = Camera.main.transform;
        if (cam == null) return inputLocal.normalized;

        Vector3 camForward = cam.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cam.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 worldDir = camForward * inputLocal.z + camRight * inputLocal.x;
        if (worldDir.sqrMagnitude > 1f) worldDir.Normalize();
        return worldDir;
    }

    void OnValidate()
    {
        if (speed < 0f) speed = 0f;
        if (rotationSpeed < 0f) rotationSpeed = 0f;
    }

    public void AttackEnemy()
    {
        Game_Manager.instance.savedIDs.Add(enemyID);
        Game_Manager.instance.PlayerPos = transform.position;
        SceneManager.LoadScene(2);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(enemyTag)) return;
        enemyInRange = true;
        enemyID = other.GetComponent<EnemyPatrol_Controller>().enemyID;
        ShowUI(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(enemyTag)) return;
        enemyInRange = false;
        ShowUI(false);
    }

    private void ShowUI(bool show)
    {
        // Mostrar/ocultar HUD
        if (enemyUIText != null)
        {
            enemyUIText.text = message;
            if (enemyUIPanel != null)
                enemyUIPanel.SetActive(show);
            else
                enemyUIText.gameObject.SetActive(show);
        }
    }
}