using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class HudCollectionMenu : MonoBehaviour, IGameMenu
{
    [SerializeField] private GameDatabaseSO db;
    [SerializeField] private Transform menuParent;
    [SerializeField] private List<HudTechCard> techCards;

    private List<TechSO> techDisplayed;
    private TechSOComparer comparer;

    private int page;

    private void OnEnable()
    {
        page = 0;
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
        if (techDisplayed.Count <= ((page+1) * techCards.Count)) { return; }

        page++;
        UpdateDisplayedInfo();
    }
    public void OnPreviousPageClicked() 
    {
        if (!IsOpen()) { return; }
        if (page <= 0) { return; }

        page--;
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
            int indexReaded = i +(page * techCards.Count);
            if (indexReaded < techDisplayed.Count)
            {
                techCards[i].SetUp(techDisplayed[indexReaded], HudTechCard.TechDisplayMode.Collection);
                techCards[i].gameObject.SetActive(true);
            }
            else 
            {
                techCards[i].gameObject.SetActive(false);
            }
        }
    }
}
