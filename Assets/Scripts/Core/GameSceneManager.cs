using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    void Start()
    {
        //UIManager.Instance.OpenMenu(MenuSelection.Main);
        UIManager.Instance.OpenMenu(MenuSelection.DebugConsole);
    }
}
