using UnityEngine;

public class IngameHudManager : MonoBehaviour
{

    [SerializeField] private HudPlayerStatusManager playerStatusManager;
    [SerializeField] private HudGameStatusManager gameStatusManager;
    [SerializeField] private HudInteractionManager interactionManager;
    [SerializeField] private HudEnemyHealthBarManager enemyHealthBarManager;
    [SerializeField] private HudCombatNotifications combatNotificationsManager;
    [SerializeField] private HudDashBar dashBarManager;
    [SerializeField] private HudShop shopManager;

    private IngameMenuBase currentMenu;

    public void SetUp(PlayerEntity p)
    {
        currentMenu = null;

        playerStatusManager.SetUp(p);
        gameStatusManager.SetUp();
        interactionManager.SetUp(p.GetCamera());
        enemyHealthBarManager.SetUp(p.GetCamera());
        combatNotificationsManager.SetUp(p.GetCamera());
        dashBarManager.SetUp(p);
        shopManager.SetUp(p);

        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        if (InputManager.Instance.GetShopInput()) { ToggleMenu(shopManager); }
    }
    private void ToggleMenu(IngameMenuBase newmenu) 
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
        Cursor.lockState = currentMenu == null ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
