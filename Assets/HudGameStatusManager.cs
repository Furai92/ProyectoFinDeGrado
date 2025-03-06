using UnityEngine;
using TMPro;

public class HudGameStatusManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyCounter;

    public void SetUp() 
    {
        EventManager.CurrencyUpdateEvent += OnCurrencyUpdated;
        OnCurrencyUpdated();
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
