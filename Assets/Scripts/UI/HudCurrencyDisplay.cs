using UnityEngine;
using TMPro;
using System.Collections;

public class HudCurrencyDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private Transform activeParent;
    [SerializeField] private TextMeshProUGUI shopReminderText;

    private void OnEnable()
    {
        EventManager.StageStateStartedEvent += OnStageStateStarted;
        EventManager.UiMenuFocusChangedEvent += OnUiFocusChanged;
        EventManager.CurrencyUpdateEvent += OnCurrencyUpdated;
        OnCurrencyUpdated();
        UpdateVisibility();
    }
    private void OnDisable()
    {
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
        EventManager.UiMenuFocusChangedEvent -= OnUiFocusChanged;
        EventManager.CurrencyUpdateEvent -= OnCurrencyUpdated;
    }
    private void OnCurrencyUpdated() 
    {
        if (PlayerEntity.ActiveInstance != null) 
        {
            currencyText.text = PlayerEntity.ActiveInstance.Money.ToString("F0"); 
        }
        else 
        {
            currencyText.text = "";
        }
    }
    private void OnStageStateStarted(StageStateBase.GameState s) 
    {
        UpdateVisibility();
    }
    private void OnUiFocusChanged(IGameMenu m) 
    {
        UpdateVisibility();
    }
    private void UpdateVisibility() 
    {
        if (IngameMenuManager.GetActiveMenu() != null) { activeParent.gameObject.SetActive(false); return; }
        shopReminderText.gameObject.SetActive(StageManagerBase.GetCurrentStateType() == StageStateBase.GameState.Rest);

        switch (StageManagerBase.GetCurrentStateType()) 
        {
            case StageStateBase.GameState.Rest:
            case StageStateBase.GameState.Delay:
            case StageStateBase.GameState.EnemyWave:
                {
                    activeParent.gameObject.SetActive(true);
                    break;
                }
            default: 
                {
                    StopAllCoroutines();
                    activeParent.gameObject.SetActive(false);
                    break;
                }
        }
    }
}
