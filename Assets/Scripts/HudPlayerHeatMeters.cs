using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudPlayerHeatMeters : MonoBehaviour
{
    [SerializeField] private Transform activeParent;

    [Header("Ranged Weapon")]
    [SerializeField] private Image heatGaugeRanged;
    [SerializeField] private TextMeshProUGUI heatPercentRanged;
    [SerializeField] private TextMeshProUGUI heatWarningRanged;
    [SerializeField] private TextMeshProUGUI overheatRanged;

    [Header("Melee Weapon")]
    [SerializeField] private Image heatGaugeMelee;
    [SerializeField] private TextMeshProUGUI heatPercentMelee;
    [SerializeField] private TextMeshProUGUI heatWarningMelee;
    [SerializeField] private TextMeshProUGUI overheatMelee;

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

        // Ranged
        heatGaugeRanged.fillAmount = Mathf.Lerp(0, 0.5f, PlayerEntity.ActiveInstance.StatusHeatRanged);
        heatGaugeRanged.color = heatPercentRanged.color = Color.Lerp(Color.blue, Color.red, PlayerEntity.ActiveInstance.StatusHeatRanged);
        heatPercentRanged.text = string.Format("{0}%", (PlayerEntity.ActiveInstance.StatusHeatRanged * 100).ToString("F1"));
        heatWarningRanged.gameObject.SetActive(PlayerEntity.ActiveInstance.StatusHeatRanged > 0.8f);
        overheatRanged.gameObject.SetActive(PlayerEntity.ActiveInstance.StatusOverheatRanged);

        // Melee
        heatGaugeMelee.fillAmount = Mathf.Lerp(0, 0.5f, PlayerEntity.ActiveInstance.StatusHeatMelee);
        heatGaugeMelee.color = heatPercentMelee.color = Color.Lerp(Color.blue, Color.red, PlayerEntity.ActiveInstance.StatusHeatMelee);
        heatPercentMelee.text = string.Format("{0}%", (PlayerEntity.ActiveInstance.StatusHeatMelee * 100).ToString("F1"));
        heatWarningMelee.gameObject.SetActive(PlayerEntity.ActiveInstance.StatusHeatMelee > 0.8f);
        overheatMelee.gameObject.SetActive(PlayerEntity.ActiveInstance.StatusOverheatMelee);
    }


}
