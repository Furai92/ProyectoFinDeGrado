using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private GameDatabase _gameDatabase;
    public event Action<bool> OnUIMenuChanged;
    public VisualElement RootVisualElement => _uiDocument.rootVisualElement;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        RegisterCallbacks();
    }

    void OnDisable()
    {
        UnregisterCallbacks();
    }

    private void RegisterCallbacks()
    {
        _uiDocument.rootVisualElement.RegisterCallback<FocusEvent>(OnElementFocus, TrickleDown.TrickleDown);
        _uiDocument.rootVisualElement.RegisterCallback<BlurEvent>(OnElementUnfocus, TrickleDown.TrickleDown);
    }

    private void UnregisterCallbacks()
    {
        _uiDocument.rootVisualElement.UnregisterCallback<FocusEvent>(OnElementFocus, TrickleDown.TrickleDown);
        _uiDocument.rootVisualElement.UnregisterCallback<BlurEvent>(OnElementUnfocus, TrickleDown.TrickleDown);
    }

    private void OnElementFocus(FocusEvent evt)
    {
        if(evt.target is TextInputBaseField<string>) 
        {
            OnUIMenuChanged?.Invoke(true);
        }
    }

    private void OnElementUnfocus(BlurEvent evt)
    {
        if(evt.target is TextInputBaseField<string>) 
        {
            OnUIMenuChanged?.Invoke(false);
        }
    }

    public void OpenMenu(MenuSelection menu)
    {
        RootVisualElement.Clear();
        _uiDocument.visualTreeAsset = _gameDatabase.Menus[menu].Item1;
        AddMenu(menu);
    }
    public void AddMenu(MenuSelection menu)
    {
        UIMenu uIMenu = _gameDatabase.Menus[menu].Item2.GetComponent<UIMenu>();
        if(uIMenu != null) gameObject.AddComponent(uIMenu.GetType());
        OnUIMenuChanged?.Invoke(true);
    }    
    public void RemoveMenu(MenuSelection menu)
    {
        RootVisualElement.Remove(_gameDatabase.Menus[menu].Item1.CloneTree());
        GameObject newMenu = Instantiate(_gameDatabase.Menus[menu].Item2);
        newMenu.transform.SetParent(transform);
        if (RootVisualElement.childCount == 0)
        {
            CloseMenu();
        }
    }
    public void ShowMenu()
    {
        RootVisualElement.style.display = DisplayStyle.Flex;
        OnUIMenuChanged?.Invoke(true);
    }
    public void CloseMenu()
    {
        RootVisualElement.style.display = DisplayStyle.None;
        OnUIMenuChanged?.Invoke(false);
    }

    public void Reset()
    {
        _uiDocument.visualTreeAsset = null;
        foreach (UIMenu child in _uiDocument.gameObject.GetComponentsInChildren<UIMenu>())
        {
            Destroy(child);
        }
    }
}

public enum MenuSelection
{
    Main,
    DebugConsole
}