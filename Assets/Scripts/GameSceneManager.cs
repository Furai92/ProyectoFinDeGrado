using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIManager.Instance.OpenMenu(MenuSelection.Main);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
