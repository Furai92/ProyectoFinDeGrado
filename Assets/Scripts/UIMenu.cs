using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIMenu : MonoBehaviour
{
    protected VisualElement RootVisualElement => UIManager.Instance.RootVisualElement;
    protected abstract string MainParentName { get; }
    protected VisualElement MainParent { get; }
    public abstract void InitializeUI();

    protected VisualElement GetMainParent()
    {
        return RootVisualElement.Q<VisualElement>(MainParentName);
    }

    protected void AddSubMenu(VisualElement root, bool hideChildren = false)
    {
        if (hideChildren)
        {
            MainParent.style.display = DisplayStyle.None;
        }
        root.style.display = DisplayStyle.Flex;
    }

    protected void RemoveSubMenu()
    {
        MainParent.style.display = DisplayStyle.Flex;
    }

    protected void RemoveAllSubMenus()
    {
        foreach (VisualElement child in MainParent.Children())
        {
            child.style.display = DisplayStyle.None;
        }
    }
}
