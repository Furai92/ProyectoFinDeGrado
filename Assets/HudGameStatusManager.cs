using UnityEngine;
using TMPro;

public class HudGameStatusManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyCounter;
    [SerializeField] private TextMeshProUGUI waveTimer;

    public void SetUp() 
    {
        EventManager.CurrencyUpdateEvent += OnCurrencyUpdated;
        OnCurrencyUpdated();
    }
    private void Update()
    {
        float t = StageManagerBase.GetTimerDisplay();
        waveTimer.text = t < 0 ? "" : t.ToString("F0");
    }
    private void OnDisable()
    {
        EventManager.CurrencyUpdateEvent -= OnCurrencyUpdated;
    }
    private void OnCurrencyUpdated() 
    {
        currencyCounter.text = StageManagerBase.GetPlayerCurrency(0).ToString("F0");
    }
}
