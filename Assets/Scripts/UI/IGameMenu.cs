using UnityEngine;

public interface IGameMenu
{
    public abstract void OpenMenu();
    public abstract void CloseMenu();
    public abstract bool IsOpen();
    public abstract bool CanBeOpened();
}
