using UnityEngine;

public abstract class IngameMenuBase : MonoBehaviour
{
    public abstract void OpenMenu();
    public abstract void CloseMenu();
    public abstract bool IsOpen();
    public abstract bool CanBeOpened();
}
