using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HudDefeatPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private CanvasGroup defeatPanel;

    private bool crStarted = false;

    private const float DEFEAT_PANEL_FADE_IN_DURATION = 0.5f;
    private const float DEFEAT_PANEL_LINGER_DURATION = 4f;
    private const float FADE_TO_BLACK_DURATION = 2f;
    private const float BLACK_SCREEN_LINGER = 1f;

    private void OnPlayerDefeated()
    {
        if (crStarted) { return; }

        crStarted = true;
        StartCoroutine(DefeatCR());
    }
    private void OnEnable()
    {
        EventManager.PlayerDefeatedEvent += OnPlayerDefeated;
    }
    private void OnDisable()
    {
        EventManager.PlayerDefeatedEvent -= OnPlayerDefeated;
    }

    IEnumerator DefeatCR()
    {
        defeatPanel.gameObject.SetActive(true);
        float t = 0;
        while (t < 1)
        {
            t = Mathf.MoveTowards(t, 1, Time.unscaledDeltaTime / DEFEAT_PANEL_FADE_IN_DURATION);
            defeatPanel.alpha = t;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(DEFEAT_PANEL_LINGER_DURATION);
        t = 0;
        fadePanel.alpha = 0;
        fadePanel.gameObject.SetActive(true);
        while (t < 1)
        {
            t = Mathf.MoveTowards(t, 1, Time.unscaledDeltaTime / FADE_TO_BLACK_DURATION);
            fadePanel.alpha = t;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(BLACK_SCREEN_LINGER);
        SceneManager.LoadScene("MainMenu");
    }
}
