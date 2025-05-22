using UnityEngine;
using TMPro;
using System.Collections;

public class HudCurrencyDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private Transform activeParent;

    private void OnEnable()
    {
        EventManager.StageStateStartedEvent += UpdateVisibility;
        EventManager.CurrencyUpdateEvent += OnCurrencyUpdated;

        OnCurrencyUpdated();
        UpdateVisibility(StageManagerBase.GetCurrentStateType());
    }
    private void OnDisable()
    {
        EventManager.StageStateStartedEvent -= UpdateVisibility;
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
    private void UpdateVisibility(StageStateBase.GameState s) 
    {
        switch (s) 
        {
            case StageStateBase.GameState.Rest:
            case StageStateBase.GameState.EnemyWave:
            case StageStateBase.GameState.BossFight: 
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
