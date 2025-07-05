using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class HudWaveStatusManager : MonoBehaviour
{

    private enum NotificationType { WaveStarted, WaveEnded, ChestSpawned, GetReady }

    [SerializeField] private TextMeshProUGUI waveTimer;
    [SerializeField] private TextMeshProUGUI waveName;
    [SerializeField] private TextMeshProUGUI readyInfoText;
    [SerializeField] private Transform visibilityParent;
    [SerializeField] private TextMeshProUGUI waveNotificationText;
    [SerializeField] private CanvasGroup waveNotificationCG;
    [SerializeField] private StringDatabaseSO sdb;

    private bool crRunning;

    private const float NOTIFICATION_FADE_DURATION = 0.2f;
    private const float NOTIFICATION_LINGER = 2f;
    private const float NOTIFICATION_MAX_SCALE = 5f;

    private Queue<NotificationType> pendingNotifications;

    private void OnEnable()
    {
        crRunning = false;
        pendingNotifications = new Queue<NotificationType>();
        waveNotificationCG.gameObject.SetActive(false);
        EventManager.StageStateStartedEvent += OnStageStateStarted;
        EventManager.UiMenuFocusChangedEvent += OnUiFocusChanged;
        EventManager.StageStateEndedEvent += OnStageStateEnded;

        UpdateVisibility();
    }
    private void OnDisable()
    {
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
        EventManager.UiMenuFocusChangedEvent -= OnUiFocusChanged;
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
        StopAllCoroutines();
    }
    private void OnUiFocusChanged(IGameMenu m)
    {
        UpdateVisibility();
    }
    private void OnStageStateStarted(StageStateBase.GameState s) 
    {
        UpdateVisibility();

        if (s == StageStateBase.GameState.EnemyWave) { AddNotification(NotificationType.WaveStarted); }
        if (s == StageStateBase.GameState.Delay) { AddNotification(NotificationType.GetReady); }
    } 
    private void OnStageStateEnded(StageStateBase.GameState s) 
    {
        if (s == StageStateBase.GameState.EnemyWave) { AddNotification(NotificationType.WaveEnded); }
    }
    private void UpdateVisibility()
    {
        readyInfoText.gameObject.SetActive(StageManagerBase.GetCurrentStateType() == StageStateBase.GameState.Rest && IngameMenuManager.GetActiveMenu() == null);
        waveTimer.gameObject.SetActive(StageManagerBase.GetCurrentStateType() == StageStateBase.GameState.EnemyWave && IngameMenuManager.GetActiveMenu() == null);
        waveName.text = StageManagerBase.GetCurrentStateType() == StageStateBase.GameState.EnemyWave ? string.Format("Wave {0}", StageManagerBase.GetWaveCount()+1) : "";
    }


    private void Update()
    {
        if (waveTimer.gameObject.activeInHierarchy) 
        {
            float t = StageManagerBase.GetTimerDisplay();
            waveTimer.text = t < 0 ? "" : t.ToString("F0");
        }
    }
    private void AddNotification(NotificationType n)
    {
        pendingNotifications.Enqueue(n);
        DisplayNextNotification();
    }
    private void DisplayNextNotification() 
    {
        if (crRunning) { return; }
        if (pendingNotifications.Count == 0) { return; }

        NotificationType nextNotification = pendingNotifications.Dequeue();
        switch (nextNotification) 
        {
            case NotificationType.ChestSpawned: { waveNotificationText.text = sdb.GetString("NOTIF_CHEST"); break; }
            case NotificationType.WaveStarted: { waveNotificationText.text = sdb.GetString("NOTIF_WAVE_START"); break; }
            case NotificationType.WaveEnded: { waveNotificationText.text = sdb.GetString("NOTIF_WAVE_END"); break; }
            case NotificationType.GetReady: { waveNotificationText.text = sdb.GetString("NOTIF_WAVE_GET_READY"); break; }
        }
        StartCoroutine(NotificationAnimation());
    }
    IEnumerator NotificationAnimation() 
    {
        crRunning = true;
        waveNotificationText.gameObject.SetActive(true);
        waveNotificationCG.alpha = 0;
        float t = 0;
        while (t < 1) 
        {
            t += Time.deltaTime / NOTIFICATION_FADE_DURATION;
            waveNotificationCG.alpha = t;
            waveNotificationCG.transform.localScale = Vector3.one * Mathf.Lerp(NOTIFICATION_MAX_SCALE, 1, t);
            yield return null;
        }
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / NOTIFICATION_LINGER;
            yield return null;
        }
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / NOTIFICATION_FADE_DURATION;
            waveNotificationCG.alpha = 1-t;
            waveNotificationCG.transform.localScale = Vector3.one * Mathf.Lerp(1, NOTIFICATION_MAX_SCALE, t);
            yield return null;
        }
        waveNotificationText.gameObject.SetActive(false);
        crRunning = false;
        DisplayNextNotification();
    }
}
