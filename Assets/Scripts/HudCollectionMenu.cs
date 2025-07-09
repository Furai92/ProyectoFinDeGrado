using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class HudCollectionMenu : MonoBehaviour, IGameMenu
{
    [SerializeField] private GameDatabaseSO db;
    [SerializeField] private Transform menuParent;
    [SerializeField] private HudPageDisplay pageDisplay;
    [SerializeField] private List<HudTechCard> techCards;

    private List<TechSO> techDisplayed;
    private TechSOComparer comparer;

    private int currentPage;

    private void OnEnable()
    {
        currentPage = 0;
        techDisplayed = db.Techs;
        comparer = new TechSOComparer();
        techDisplayed.Sort(comparer);
    }
    public bool CanBeOpened()
    {
        return true;
    }

    public void OnNextPageClicked() 
    {
        if (!IsOpen()) { return; }
        if (techDisplayed.Count <= ((currentPage+1) * techCards.Count)) { return; }

        currentPage++;
        UpdateDisplayedInfo();
    }
    public void OnPreviousPageClicked() 
    {
        if (!IsOpen()) { return; }
        if (currentPage <= 0) { return; }

        currentPage--;
        UpdateDisplayedInfo();
    }

    public void CloseMenu()
    {
        EventManager.OnUiMenuClosed(this);
        menuParent.gameObject.SetActive(false);
    }

    public bool IsOpen()
    {
        return menuParent.gameObject.activeInHierarchy;
    }

    public void OpenMenu()
    {
        UpdateDisplayedInfo();
        menuParent.gameObject.SetActive(true);
    }
    private void UpdateDisplayedInfo() 
    {
        for (int i = 0; i < techCards.Count; i++) 
        {
            int indexReaded = i +(currentPage * techCards.Count);
            if (indexReaded < techDisplayed.Count)
            {
                techCards[i].SetUp(techDisplayed[indexReaded], 1, false, false, true);
                techCards[i].gameObject.SetActive(true);
            }
            else 
            {
                techCards[i].gameObject.SetActive(false);
            }
        }
        int pages = Mathf.CeilToInt((float)techDisplayed.Count / techCards.Count);
        pageDisplay.UpdateDisplay(pages, currentPage);
    }
    public string GetMenuNameID()
    {
        return "MENU_NAME_COLLECTION";
    }
}
