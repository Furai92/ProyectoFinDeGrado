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
        EventManager.UiFocusChangedEvent += OnUiFocusChanged;
        
    }

    void OnDisable()
    {
        _inputActions.Disable();
        EventManager.UiFocusChangedEvent -= OnUiFocusChanged;
    }

    public Vector2 GetMovementInput()
    {
        return _inputActions.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetLookInput()
    {
        return _inputActions.Player.Look.ReadValue<Vector2>();
    }
    public bool GetShopInput() 
    {
        return _inputActions.Player.ToggleShop.WasPressedThisFrame();
    }
    public bool GetRangedAttackInput() 
    {
        return _inputActions.Player.RangedAttack.IsPressed();
    }
    public bool GetMeleeAttackInput()
    {
        return _inputActions.Player.MeleeAttack.IsPressed();
    }
    public bool GetInteractInput() 
    {
        return _inputActions.Player.Interact.WasPressedThisFrame();
    }
    public bool GetReadyUpInput() 
    {
        return _inputActions.Player.ReadyUp.WasPerformedThisFrame();
    }
    public bool GetDashInput()
    {
        return _inputActions.Player.Dash.WasPerformedThisFrame();
    }

    private void OnUiFocusChanged(bool status)
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
