using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class IngameMenuManager : MonoBehaviour
{
    [SerializeField] private HudSettingsMenu settingsMenu;
    [SerializeField] private HudStatsMenu playerStatsMenu;
    [SerializeField] private HudStageStatusMenu stageStatusMenu;
    [SerializeField] private HudActiveTechMenu activeTechMenu;
    [SerializeField] private HudShop shopMenu;

    [SerializeField] private Transform menuBarParent;
    [SerializeField] private List<CanvasGroup> menuNamePanels;
    [SerializeField] private StringDatabaseSO sdb;

    private IGameMenu activeMenu;
    private int currentMenuIndex;
    private List<IGameMenu> nonShopMenus;
    private static IngameMenuManager instance;
    private bool playerDefeated;

    private const float MENU_TAB_ALPHA_SELECTED = 1f;
    private const float MENU_TAB_ALPHA_UNSELECTED = 0.5f;

    private void OnEnable()
    {
        currentMenuIndex = 0;
        nonShopMenus = new List<IGameMenu>() { playerStatsMenu, stageStatusMenu, activeTechMenu, settingsMenu };
        menuBarParent.gameObject.SetActive(false);
        UpdateFocus();
        instance = this;

        EventManager.PlayerDefeatedEvent += OnPlayerDefeated;
    }
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        EventManager.PlayerDefeatedEvent -= OnPlayerDefeated;
    }
    private void Update()
    {
        if (playerDefeated) { return; } // Ignore inputs if the player is defeated

        if (InputManager.Instance.GetIngameMenuInput()) 
        {
            ToggleMenu(nonShopMenus[currentMenuIndex]);
            UpdateMenuBar();
            UpdateFocus();
        }
        if (InputManager.Instance.GetShopInput()) 
        {
            ToggleMenu(shopMenu);
            UpdateFocus();
        }
        if (InputManager.Instance.GetReadyUpInput() && activeMenu == null)
        {
            EventManager.OnAllPlayersReady();
        }

    }
    private void OnPlayerDefeated() 
    {
        playerDefeated = true;
    }
    private void ToggleMenu(IGameMenu m) 
    {
        if (!m.CanBeOpened()) { return; }
        if (activeMenu == m) 
        {
            activeMenu.CloseMenu();
            menuBarParent.gameObject.SetActive(false);
            activeMenu = null;
            EventManager.OnUiMenuFocusChanged(null);
            return; 
        }

        if (activeMenu != null) { activeMenu.CloseMenu(); }
        m.OpenMenu();
        activeMenu = m;
        menuBarParent.gameObject.SetActive(m != (IGameMenu)shopMenu);
        EventManager.OnUiMenuFocusChanged(m);
    }
    private void UpdateMenuBar() 
    {
        for (int i = 0; i < menuNamePanels.Count; i++) 
        {
            menuNamePanels[i].alpha = currentMenuIndex == i ? MENU_TAB_ALPHA_SELECTED : MENU_TAB_ALPHA_UNSELECTED;
        }
    }
    private void UpdateFocus() 
    {
        Cursor.lockState = activeMenu != null ? CursorLockMode.None : CursorLockMode.Locked;
        TimescaleManager.SetPauseState(activeMenu != null);
    }
    public void OnCloseShopClicked() 
    {
        if (activeMenu != (IGameMenu)shopMenu) { return; }

        ToggleMenu(shopMenu);
        UpdateFocus();
    }
    public void OnQuitClicked()
    {
        EventManager.OnStageAbandoned();
    }
    public void OnNextMenuButtonClicked() 
    {
        currentMenuIndex++;
        if (currentMenuIndex >= menuNamePanels.Count) { currentMenuIndex = 0; }
        ToggleMenu(nonShopMenus[currentMenuIndex]);
        UpdateMenuBar();

    }
    public void OnPreviousMenuButtonClicked() 
    {
        currentMenuIndex--;
        if (currentMenuIndex < 0) { currentMenuIndex = menuNamePanels.Count-1; }
        ToggleMenu(nonShopMenus[currentMenuIndex]);
        UpdateMenuBar();
    }

    public static IGameMenu GetActiveMenu() 
    {
        if (instance == null) { return null; }

        return instance.activeMenu;
    }

}
