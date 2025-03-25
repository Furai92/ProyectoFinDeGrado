using UnityEngine;
using TMPro;

public class HudGameStatusManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyCounter;
    [SerializeField] private TextMeshProUGUI waveTimer;
    [SerializeField] private TextMeshProUGUI readyInfoText;

    public void SetUp() 
    {
        gameObject.SetActive(true);
        EventManager.CurrencyUpdateEvent += OnCurrencyUpdated;
        EventManager.StageStateStartedEvent += OnStageStateStarted;
        EventManager.StageStateEndedEvent += OnStageStateEnded;
        OnCurrencyUpdated();
    }
    private void Update()
    {
        float t = StageManagerBase.GetTimerDisplay();
        waveTimer.text = t < 0 ? "" : t.ToString("F0");

        if (InputManager.Instance.GetReadyUpInput()) { EventManager.OnAllPlayersReady(); }
    }
    private void OnDisable()
    {
        EventManager.CurrencyUpdateEvent -= OnCurrencyUpdated;
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
    }
    private void OnStageStateStarted(StageStateBase s)
    {
        UpdateReadyInfoText(s);
    }
    private void OnStageStateEnded(StageStateBase s)
    {
        UpdateReadyInfoText(s);
    }
    private void UpdateReadyInfoText(StageStateBase s) 
    {
        readyInfoText.gameObject.SetActive(s.GetStateType() == StageStateBase.StateType.Rest);
    }
    private void OnCurrencyUpdated() 
    {
        currencyCounter.text = StageManagerBase.GetPlayerCurrency(0).ToString("F0");
    }
}
