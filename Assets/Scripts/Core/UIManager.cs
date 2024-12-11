using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;

    void OnEnable()
    {
        _uiDocument.rootVisualElement.RegisterCallback<FocusEvent>(OnElementFocus);
        _uiDocument.rootVisualElement.RegisterCallback<BlurEvent>(OnElementUnfocus);
        //_uiDocument.rootVisualElement.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    void OnDisable()
    {
        _uiDocument.rootVisualElement.UnregisterCallback<FocusEvent>(OnElementFocus);
        _uiDocument.rootVisualElement.UnregisterCallback<BlurEvent>(OnElementUnfocus);
        //_uiDocument.rootVisualElement.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    private void OnElementFocus(FocusEvent evt)
    {
        Debug.Log("Element focused: " + evt.target);
    }

private void OnElementUnfocus(BlurEvent evt)
{
    Debug.Log("Element unfocused: " + evt.target);
}

/*
    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        var rootElement = evt.target as VisualElement;
        if (rootElement.resolvedStyle.display == DisplayStyle.Flex)
        {
            // Raise the OnRootElementDisplayed event
            OnRootElementDisplayed?.Invoke(rootElement);
            Debug.Log("The root element of the UIDocument was displayed.");
        }
        else if (rootElement.resolvedStyle.display == DisplayStyle.None)
        {
            // Raise the OnRootElementHidden event
            OnRootElementHidden?.Invoke(rootElement);
            Debug.Log("The root element of the UIDocument was hidden.");
        }
    }*/
}
