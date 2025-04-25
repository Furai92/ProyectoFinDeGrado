using UnityEngine;
using System.Collections.Generic;

public class HudNewRunMenu : MonoBehaviour, IGameMenu
{
    [SerializeField] private Transform menuParent;
    [SerializeField] private List<Transform> pages;

    private int currentPage;

    private void UpdatePage() 
    {

    }

    public bool CanBeOpened()
    {
        return true;
    }

    public void CloseMenu()
    {
        menuParent.gameObject.SetActive(false);
    }

    public bool IsOpen()
    {
        return menuParent.gameObject.activeInHierarchy;
    }

    public void OpenMenu()
    {
        menuParent.gameObject.SetActive(true);
        currentPage = 0;
    }
}
