using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudPlayerHealthBarManager : MonoBehaviour
{
    [SerializeField] private Image healthFill;
    [SerializeField] private Image healthFillTrail;
    [SerializeField] private Image shieldFill;
    [SerializeField] private Image lightDamageFill;
    [SerializeField] private Transform activeParent;
    [SerializeField] private TextMeshProUGUI healthNumber;

    private const float HEALTH_FILL_TRAIL_SPEED = 2f;

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
        switch (s) 
        {
            case StageStateBase.GameState.EnemyWave:
            case StageStateBase.GameState.Rest:
            case StageStateBase.GameState.BossFight: 
                {
                    activeParent.gameObject.SetActive(true);
                    break;
                }
            default: 
                {
                    activeParent.gameObject.SetActive(false);
                    break;
                }
        }
    }
    private void Update()
    {
        if (!activeParent.gameObject.activeInHierarchy) { return; }

        healthFill.fillAmount = PlayerEntity.ActiveInstance.GetHealthPercent();
        shieldFill.fillAmount = PlayerEntity.ActiveInstance.GetShieldPercent();
        lightDamageFill.fillAmount = PlayerEntity.ActiveInstance.GetLightDamagePercent();
        healthFillTrail.fillAmount = Mathf.MoveTowards(healthFillTrail.fillAmount, healthFill.fillAmount, Time.deltaTime * HEALTH_FILL_TRAIL_SPEED);
        if (PlayerEntity.ActiveInstance.CurrentShield > 0)
        {
            healthNumber.text = string.Format("{0} +{1}", PlayerEntity.ActiveInstance.CurrentHealth.ToString("F0"), PlayerEntity.ActiveInstance.CurrentShield.ToString("F0"));
        }
        else 
        {
            healthNumber.text = PlayerEntity.ActiveInstance.CurrentHealth.ToString("F0");
        }

    }
}
