using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HudVictoryPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private CanvasGroup victoryPanel;

    private bool crStarted = false;

    private const float VICTORY_PANEL_FADE_IN_DURATION = 0.5f;
    private const float VICTORY_PANEL_LINGER_DURATION = 3f;
    private const float FADE_TO_BLACK_DURATION = 2f;
    private const float BLACK_SCREEN_LINGER = 1f;

    private void UpdateVisibility(StageStateBase.GameState s) 
    {
        if (s == StageStateBase.GameState.Victory && !crStarted) 
        {
            crStarted = true;
            StartCoroutine(VictoryCR());
        }
    }
    private void OnEnable()
    {
        EventManager.StageStateStartedEvent += UpdateVisibility;
        UpdateVisibility(StageManagerBase.GetCurrentStateType());
    }
    private void OnDisable()
    {
        EventManager.StageStateStartedEvent -= UpdateVisibility;
    }

    IEnumerator VictoryCR() 
    {
        victoryPanel.gameObject.SetActive(true);


        float t = 0;
        while (t < 1) 
        {
            t = Mathf.MoveTowards(t, 1, Time.deltaTime / VICTORY_PANEL_FADE_IN_DURATION);
            victoryPanel.alpha = t;
            yield return null;
        }
        yield return new WaitForSeconds(VICTORY_PANEL_LINGER_DURATION);
        t = 0;
        fadePanel.alpha = 0;
        fadePanel.gameObject.SetActive(true);
        while (t < 1) 
        {
            t = Mathf.MoveTowards(t, 1, Time.deltaTime / FADE_TO_BLACK_DURATION);
            fadePanel.alpha = t;
            yield return null;
        }
        yield return new WaitForSeconds(BLACK_SCREEN_LINGER);
        SceneManager.LoadScene("MainMenu");
    }
}
