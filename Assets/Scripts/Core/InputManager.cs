using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    private InputSystem_Actions _inputActions;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        _inputActions.Enable();
        UIManager.Instance.OnUIMenuChanged += OnUIMenuChanged;
        
    }

    void OnDisable()
    {
        _inputActions.Disable();
        UIManager.Instance.OnUIMenuChanged -= OnUIMenuChanged;
    }

    public Vector2 GetMovementInput()
    {
        return _inputActions.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetLookInput()
    {
        return _inputActions.Player.Look.ReadValue<Vector2>();
    }

    private void OnUIMenuChanged(bool status)
    {
        if (status)
        {
            _inputActions.Player.Disable();
        }
        else
        {
            _inputActions.Player.Enable();
        }
    }
}
