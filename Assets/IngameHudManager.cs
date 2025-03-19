using UnityEngine;

public class IngameHudManager : MonoBehaviour
{
    [SerializeField] private HudPlayerStatusManager playerStatusManager;
    [SerializeField] private HudGameStatusManager gameStatusManager;
    [SerializeField] private HudInteractionManager interactionManager;

    public void SetUp(PlayerEntity p)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.Confined;

        playerStatusManager.SetUp(p);
        gameStatusManager.SetUp();
        interactionManager.SetUp();
    }
}
