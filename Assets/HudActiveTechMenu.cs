using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class HudActiveTechMenu : MonoBehaviour, IGameMenu
{
    [SerializeField] private List<HudTechCard> cardsDisplayed;
    [SerializeField] private Transform menuParent;

    private int index;

    private void UpdateDisplayedInfo()
    {
        if (!IsOpen()) { return; }

        List<PlayerEntity.TechGroup> activeTech = PlayerEntity.ActiveInstance.ActiveTechList;

        for (int i = 0; i < cardsDisplayed.Count; i++) 
        {
            if (i + index - 2 < 0 || i + index - 2 >= activeTech.Count) { cardsDisplayed[i].gameObject.SetActive(false); }
            else { cardsDisplayed[i].gameObject.SetActive(true); cardsDisplayed[i].SetUp(activeTech[i + index - 2].SO, activeTech[i + index - 2].Level, false, true); }
        }

    }

    public void OnNextButtonClicked() 
    {
        if (!IsOpen()) { return; }

        index++;
        if (index >= PlayerEntity.ActiveInstance.ActiveTechList.Count) { index = PlayerEntity.ActiveInstance.ActiveTechList.Count - 1; }
        UpdateDisplayedInfo();
    }
    public void OnPreviousButtonClicked() 
    {
        if (!IsOpen()) { return; }

        index--;
        if (index < 0) { index = 0; }
        UpdateDisplayedInfo();
    }

    public bool CanBeOpened()
    {
        return PlayerEntity.ActiveInstance != null;
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
        menuParent.gameObject.SetActive(true);
        index = 0;
        UpdateDisplayedInfo();
    }
}
