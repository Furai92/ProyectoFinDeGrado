using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class HudMilestoneMenu : MonoBehaviour, IGameMenu
{
    [SerializeField] private GameDatabaseSO db;
    [SerializeField] private Transform menuParent;
    [SerializeField] private HudPageDisplay pageDisplay;
    [SerializeField] private List<HudMilestonePanel> milestoneCards;

    private List<MilestoneSO> milestonesDisplayed;

    private int currentPage;

    private void OnEnable()
    {
        currentPage = 0;
        milestonesDisplayed = db.Milestones;
    }
    public bool CanBeOpened()
    {
        return true;
    }

    public void OnNextPageClicked()
    {
        if (!IsOpen()) { return; }
        if (milestonesDisplayed.Count <= ((currentPage + 1) * milestoneCards.Count)) { return; }

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
        for (int i = 0; i < milestoneCards.Count; i++)
        {
            int indexReaded = i + (currentPage * milestoneCards.Count);
            if (indexReaded < milestonesDisplayed.Count)
            {
                milestoneCards[i].SetUp(milestonesDisplayed[indexReaded]);
                milestoneCards[i].gameObject.SetActive(true);
            }
            else
            {
                milestoneCards[i].gameObject.SetActive(false);
            }
        }
        int pages = Mathf.CeilToInt((float)milestonesDisplayed.Count / milestoneCards.Count);
        pageDisplay.UpdateDisplay(pages, currentPage);
    }
    public string GetMenuNameID()
    {
        return "MENU_NAME_MILESTONE";
    }
}
