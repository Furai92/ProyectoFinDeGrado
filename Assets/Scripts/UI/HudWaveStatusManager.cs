using UnityEngine;
using TMPro;

public class HudWaveStatusManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveTimer;
    [SerializeField] private TextMeshProUGUI readyInfoText;
    [SerializeField] private Transform visibilityParent;

    private void OnEnable()
    {
        EventManager.StageStateStartedEvent += OnStageStateStarted;
        EventManager.UiMenuFocusChangedEvent += OnUiFocusChanged;

        UpdateVisibility();
    }
    private void OnDisable()
    {
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
        EventManager.UiMenuFocusChangedEvent -= OnUiFocusChanged;
    }
    private void OnUiFocusChanged(IGameMenu m)
    {
        UpdateVisibility();
    }
    private void OnStageStateStarted(StageStateBase.GameState s) 
    {
        UpdateVisibility();
    }
    private void UpdateVisibility()
    {
        readyInfoText.gameObject.SetActive(StageManagerBase.GetCurrentStateType() == StageStateBase.GameState.Rest && IngameMenuManager.GetActiveMenu() == null);
        waveTimer.gameObject.SetActive(StageManagerBase.GetCurrentStateType() == StageStateBase.GameState.EnemyWave && IngameMenuManager.GetActiveMenu() == null);
    }

    private void Update()
    {
        if (waveTimer.gameObject.activeInHierarchy) 
        {
            float t = StageManagerBase.GetTimerDisplay();
            waveTimer.text = t < 0 ? "" : t.ToString("F0");
        }
    }
}
