using UnityEngine;

public class IngameHudManager : MonoBehaviour
{
    [SerializeField] private HudPlayerStatusManager playerStatusManager;
    [SerializeField] private HudGameStatusManager gameStatusManager;
    [SerializeField] private HudInteractionManager interactionManager;
    [SerializeField] private HudEnemyHealthBarManager enemyHealthBarManager;
    [SerializeField] private HudDamageNotificationsManager damageNotificationsManager;

    public void SetUp(PlayerEntity p)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.Confined;

        playerStatusManager.SetUp(p);
        gameStatusManager.SetUp();
        interactionManager.SetUp(p.GetCamera());
        enemyHealthBarManager.SetUp(p.GetCamera());
        damageNotificationsManager.SetUp(p.GetCamera());
    }
}
