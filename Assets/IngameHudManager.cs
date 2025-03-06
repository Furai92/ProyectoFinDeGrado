using UnityEngine;

public class IngameHudManager : MonoBehaviour
{
    [SerializeField] private HudPlayerStatusManager playerStatusManager;
    [SerializeField] private HudGameStatusManager gameStatusManager;

    public void SetUp(PlayerController p)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.Confined;

        playerStatusManager.SetUp(p);
        gameStatusManager.SetUp();
    }
}
