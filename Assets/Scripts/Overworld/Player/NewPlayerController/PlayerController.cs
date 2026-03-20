using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour

{
    [SerializeField] private float AnimBlendSpeed = 8.9f;
    [SerializeField] private Transform CameraRoot;
    [SerializeField] private Transform Camera;
    [SerializeField] private float MouseSensitivity = 21.9f;
    public Rigidbody rb;
    private InputManager IM;
    private Animator _animator;
    private bool _hasAnimator;
    private int _xVelHash;
    private int _yVelHash;
    private float _xRotation;
    public float _walkSpeed = 2f;
    private Vector2 _currentVelocity;
    public Animator animator;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI enemyUIText; // Texto del HUD
    [SerializeField] private GameObject enemyUIPanel;     // Panel del HUD (opcional)
    [SerializeField] private string message = "Atacar";

    public bool enemyInRange;
    private string enemyTag = "Enemy";
    public int enemyID;

    private void Start()

    {
        _hasAnimator = TryGetComponent<Animator>(out _animator);
        rb = GetComponent<Rigidbody>();
        IM = GetComponent<InputManager>();
        _xVelHash = Animator.StringToHash("X_Velocity");
        _yVelHash = Animator.StringToHash("Y_Velocity");
    }

    public void FixedUpdate()

    {
        Move();
    }

    public void LateUpdate()
    {
        CamMovements();
    }
    public Rigidbody GetRigidbody() { return rb; }
    private void Move()

    {
        if (!_hasAnimator) return;
        float targetSpeed = IM.Move.magnitude * _walkSpeed;
        if (IM.Move == Vector2.zero) targetSpeed = 0.1f;

        _currentVelocity.x = Mathf.Lerp(_currentVelocity.x, IM.Move.x * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);
        _currentVelocity.y = Mathf.Lerp(_currentVelocity.y, IM.Move.y * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);
        var xVelDifference = _currentVelocity.x - rb.linearVelocity.x;
        var zVelDifference = _currentVelocity.y - rb.linearVelocity.z;

        rb.AddForce(transform.TransformVector(new Vector3(xVelDifference, 0, zVelDifference)), ForceMode.VelocityChange);
        _animator.SetFloat(_xVelHash, _currentVelocity.x);
        _animator.SetFloat(_yVelHash, _currentVelocity.y);
    }

    private void CamMovements()
    {
        if (!_hasAnimator) return;

        var Mouse_X = IM.Look.x;
        var Mouse_Y = IM.Look.y;
        Camera.position = CameraRoot.position;

        _xRotation -= Mouse_Y * MouseSensitivity * Time.deltaTime;
        _xRotation = Mathf.Clamp(_xRotation, 0, 0);

        Camera.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        transform.Rotate(Vector3.up, Mouse_X * MouseSensitivity * Time.deltaTime);
    }
    public void AttackEnemy()
    {
        Game_Manager.instance.savedIDs.Add(enemyID);
        Game_Manager.instance.PlayerPos = transform.position;
        ShowCursor();
        SceneManager.LoadScene(2);
    }

    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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