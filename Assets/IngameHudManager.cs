using UnityEngine;

public class IngameHudManager : MonoBehaviour
{
    [SerializeField] private HudPlayerStatusManager playerStatusManager;

    public void SetUp(PlayerController p)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.Confined;

        playerStatusManager.SetUp(p);
    }
}
