using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIMenu : MonoBehaviour
{
    protected VisualElement RootVisualElement => UIManager.Instance.RootVisualElement;
    protected abstract string MainParentName { get; }
    public VisualElement MainParent { get; private set; }
    private Stack<VisualElement> _subMenuStack = new Stack<VisualElement>();
    public abstract void InitializeUI();

    void Awake()
    {
        SetMainParent();
        InitializeUI();
    }

    public void SetMainParent()
    {
        MainParent = RootVisualElement.Q<VisualElement>(MainParentName);
    }

    protected void AddSubMenu(VisualElement root, Action initializeAction, bool hideChildren = false)
    {
        if (hideChildren)
        {
            foreach (VisualElement child in RootVisualElement.Children())
            {
                child.style.display = DisplayStyle.None;
            }
        }
        root.style.display = DisplayStyle.Flex;
        _subMenuStack.Push(root);
        initializeAction?.Invoke();
    }

    protected void RemoveSubMenu(Action closeAction)
    {
        closeAction?.Invoke();
        MainParent.style.display = DisplayStyle.Flex;
        VisualElement subMenu = _subMenuStack.Pop();
        subMenu.style.display = DisplayStyle.None;
    }

    protected void RemoveAllSubMenus()
    {
        foreach (VisualElement child in RootVisualElement.Children())
        {
            child.style.display = DisplayStyle.None;
        }
        _subMenuStack.Clear();
        MainParent.style.display = DisplayStyle.Flex;
    }
}
