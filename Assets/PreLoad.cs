using UnityEngine;
using UnityEngine.SceneManagement;

public class PreLoad : MonoBehaviour
{
    private void OnEnable()
    {
        SaveDataManager.LoadFromFile();
        SceneManager.LoadScene("MainMenu");

    }
}
