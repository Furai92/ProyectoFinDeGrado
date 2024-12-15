using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    void Start()
    {
        UIManager.Instance.OpenMenu(MenuSelection.Main);
    }
}
