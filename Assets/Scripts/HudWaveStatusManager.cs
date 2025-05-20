using UnityEngine;
using TMPro;

public class HudWaveStatusManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveTimer;
    [SerializeField] private TextMeshProUGUI readyInfoText;

    private void OnEnable()
    {
        EventManager.StageStateStartedEvent += UpdateVisibility;

        UpdateVisibility(StageManagerBase.GetCurrentStateType());
    }
    private void OnDisable()
    {
        EventManager.StageStateStartedEvent -= UpdateVisibility;
    }
    private void UpdateVisibility(StageStateBase.GameState s) 
    {
        readyInfoText.gameObject.SetActive(s == StageStateBase.GameState.Rest);
        waveTimer.gameObject.SetActive(s == StageStateBase.GameState.EnemyWave);
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
