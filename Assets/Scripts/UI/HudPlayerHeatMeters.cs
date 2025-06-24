using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudPlayerHeatMeters : MonoBehaviour
{
    [SerializeField] private Transform activeParent;

    [Header("Ranged Weapon")]
    [SerializeField] private Image heatGaugeRanged;
    [SerializeField] private Image backgroundOverclockRanged;
    [SerializeField] private Image backgroundSubzeroRanged;
    [SerializeField] private TextMeshProUGUI heatPercentRanged;
    [SerializeField] private TextMeshProUGUI heatWarningRanged;
    [SerializeField] private TextMeshProUGUI overheatRanged;

    [Header("Melee Weapon")]
    [SerializeField] private Image heatGaugeMelee;
    [SerializeField] private Image backgroundOverclockMelee;
    [SerializeField] private Image backgroundSubzeroMelee;
    [SerializeField] private TextMeshProUGUI heatPercentMelee;
    [SerializeField] private TextMeshProUGUI heatWarningMelee;
    [SerializeField] private TextMeshProUGUI overheatMelee;

    private bool playerDefeated;

    private void OnEnable()
    {
        playerDefeated = false;

        EventManager.StageStateStartedEvent += OnStageStateStarted;
        EventManager.PlayerStatsUpdatedEvent += OnPlayerStatsUpdated;
        EventManager.UiMenuFocusChangedEvent += OnUiFocusChanged;
        EventManager.PlayerDefeatedEvent += OnPlayerDefeated;
        UpdateVisibility();
        UpdateBackgroundMarkers();
    }
    private void OnDisable()
    {
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
        EventManager.UiMenuFocusChangedEvent -= OnUiFocusChanged;
        EventManager.PlayerStatsUpdatedEvent -= OnPlayerStatsUpdated;
        EventManager.PlayerDefeatedEvent -= OnPlayerDefeated;
    }
    private void OnPlayerDefeated()
    {
        playerDefeated = true;
        UpdateVisibility();
    }
    private void OnPlayerStatsUpdated(PlayerEntity p) 
    {
        UpdateBackgroundMarkers();
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
        if (IngameMenuManager.GetActiveMenu() != null || playerDefeated) { activeParent.gameObject.SetActive(false); return; }

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
    private void UpdateBackgroundMarkers() 
    {
        if (PlayerEntity.ActiveInstance == null)
        {
            backgroundSubzeroMelee.fillAmount = backgroundSubzeroRanged.fillAmount = backgroundOverclockMelee.fillAmount = backgroundOverclockRanged.fillAmount = 0;
        }
        else 
        {
            float heatRange = PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.HeatFloor) + PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.HeatCap);
            backgroundOverclockMelee.fillAmount = backgroundOverclockRanged.fillAmount = (PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.HeatCap)-100) / heatRange * 0.25f;
            backgroundSubzeroMelee.fillAmount = backgroundSubzeroRanged.fillAmount = PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.HeatFloor) / heatRange * 0.25f;
        }
    }
    private void Update()
    {
        if (!activeParent.gameObject.activeInHierarchy) { return; }

        float heatRange = PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.HeatFloor) + PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.HeatCap);
        float rangedPercent = (PlayerEntity.ActiveInstance.StatusHeatRanged + PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.HeatFloor)) / heatRange; 
        float meleePercent = (PlayerEntity.ActiveInstance.StatusHeatMelee + PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.HeatFloor)) / heatRange;

        // Ranged
        heatGaugeRanged.fillAmount = Mathf.Lerp(0, 0.25f, rangedPercent);
        heatGaugeRanged.color = heatPercentRanged.color = Color.Lerp(Color.blue, Color.red, rangedPercent);
        heatPercentRanged.text = string.Format("{0}%", (PlayerEntity.ActiveInstance.StatusHeatRanged).ToString("F1"));
        heatWarningRanged.gameObject.SetActive(rangedPercent > 0.8f);
        overheatRanged.gameObject.SetActive(PlayerEntity.ActiveInstance.StatusOverheatRanged);

        // Melee
        heatGaugeMelee.fillAmount = Mathf.Lerp(0, 0.25f, meleePercent);
        heatGaugeMelee.color = heatPercentMelee.color = Color.Lerp(Color.blue, Color.red, meleePercent);
        heatPercentMelee.text = string.Format("{0}%", (PlayerEntity.ActiveInstance.StatusHeatMelee).ToString("F1"));
        heatWarningMelee.gameObject.SetActive(meleePercent > 0.8f);
        overheatMelee.gameObject.SetActive(PlayerEntity.ActiveInstance.StatusOverheatMelee);
    }


}
