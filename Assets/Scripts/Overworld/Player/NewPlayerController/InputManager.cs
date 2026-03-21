using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class InputManager : MonoBehaviour
{
    [SerializeField] private PlayerInput PlayerInput;
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool Attack { get; private set; }
    public bool Interact { get; private set; }

    private InputActionMap _currentmap;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _attackAction;
    private InputAction _interactAction;

    private void Awake()
    {
        HideCursor();
        _currentmap = PlayerInput.currentActionMap;
        _moveAction = _currentmap.FindAction("Move");
        _lookAction = _currentmap.FindAction("Look");
        _attackAction = _currentmap.FindAction("Attack");
        _interactAction = _currentmap.FindAction("Interact");

        _moveAction.performed += onMove;
        _lookAction.performed += onLook;
        _attackAction.performed += onAttack;
        _interactAction.performed += onInteract;

        _moveAction.canceled += onMove;
        _lookAction.canceled += onLook;
        _attackAction.canceled += onAttack;
        _interactAction.canceled += onInteract;
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void onMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }
    private void onLook(InputAction.CallbackContext context)
    {
        Look = context.ReadValue<Vector2>();
    }
    private void onAttack(InputAction.CallbackContext context)
    {
        Attack = context.ReadValueAsButton();
    }
    private void onInteract(InputAction.CallbackContext context)
    {
        Interact = context.ReadValueAsButton();
    }

    private void OnEnable()
    {
       _currentmap.Enable();
    }

    private void OnDisable()
    {
       _currentmap.Disable();
    }
}
