using UnityEngine;

public class IngameHudManager : MonoBehaviour
{
    [SerializeField] private HudShop shopManager;
    [SerializeField] private HudStatsMenu playerStatsMenuManager;
    [SerializeField] private HudStageStatusMenu stageStatsMenuManager;
    [SerializeField] private HudSettingsMenu settingsMenuManager;

    private IGameMenu currentMenu;

    private void OnEnable()
    {
        currentMenu = null;
        Cursor.lockState = CursorLockMode.Locked;
        EventManager.UiMenuClosedEvent += OnMenuClosed;
    }
    private void OnDisable()
    {
        EventManager.UiMenuClosedEvent -= OnMenuClosed;
    }
    private void OnMenuClosed(IGameMenu m) 
    {
        if (currentMenu == m) 
        {
            currentMenu = null;
            UpdateFocus();
        }
    }
    private void Update()
    {
        if (InputManager.Instance.GetShopInput()) { ToggleMenu(shopManager); }
        if (InputManager.Instance.GetStageStatsInput()) { ToggleMenu(stageStatsMenuManager); }
        if (InputManager.Instance.GetPlayerStatsInput()) { ToggleMenu(playerStatsMenuManager); }
        if (InputManager.Instance.GetSettingsInput()) { ToggleMenu(settingsMenuManager); }
    }
    private void UpdateFocus() 
    {
        Cursor.lockState = currentMenu == null ? CursorLockMode.Locked : CursorLockMode.None;
        Time.timeScale = currentMenu == null ? 1 : 0;
        EventManager.OnUiMenuFocusChanged(currentMenu);
    }
    private void ToggleMenu(IGameMenu newmenu) 
    {
        if (currentMenu == newmenu)
        {
            // If the current menu is the same than the one we're trying to open, close it
            currentMenu.CloseMenu();
            currentMenu = null;
        }
        else if (newmenu.CanBeOpened())
        {
            if (currentMenu != null) { currentMenu.CloseMenu(); }
            currentMenu = newmenu;
            newmenu.OpenMenu();
        }
        UpdateFocus();
    }
}
