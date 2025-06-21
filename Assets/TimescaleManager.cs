using UnityEngine;

public class TimescaleManager : MonoBehaviour
{
    private static TimescaleManager instance;

    private bool paused;
    private bool playerDefeated;

    private const float DEFEATED_TIMESCALE = 0.15f;

    private void OnEnable()
    {
        instance = this;
        EventManager.PlayerDefeatedEvent += OnPlayerDefeated;
    }
    private void OnDisable()
    {
        EventManager.PlayerDefeatedEvent -= OnPlayerDefeated;
        Time.timeScale = 1;
    }
    private void OnPlayerDefeated() 
    {
        playerDefeated = true;
        UpdateTimescale();
    }
    private void UpdateTimescale() 
    {
        if (paused) { Time.timeScale = 0; }
        else 
        {
            Time.timeScale = playerDefeated ? DEFEATED_TIMESCALE : 1;
        }
    }

    public static void SetPauseState(bool paused) 
    {
        if (instance == null) { return; }

        instance.paused = paused;
        instance.UpdateTimescale();
    }
}
