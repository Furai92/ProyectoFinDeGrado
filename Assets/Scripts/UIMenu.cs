using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIMenu : MonoBehaviour
{
    protected VisualElement RootVisualElement => UIManager.Instance.RootVisualElement;
    protected abstract string MainParentName { get; }
    protected VisualElement MainParent { get => GetMainParent(); }
    private Stack<VisualElement> _subMenuStack = new Stack<VisualElement>();
    public abstract void InitializeUI();

    protected VisualElement GetMainParent()
    {
        return RootVisualElement.Q<VisualElement>(MainParentName);
    }

    protected void AddSubMenu(VisualElement root, bool hideChildren = false)
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
    }

    protected void RemoveSubMenu()
    {
        Debug.Log(MainParent);
        MainParent.style.display = DisplayStyle.Flex;
        VisualElement subMenu = _subMenuStack.Pop();
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
