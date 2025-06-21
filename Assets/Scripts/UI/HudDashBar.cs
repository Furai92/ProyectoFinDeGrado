using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HudDashBar : MonoBehaviour
{
    [SerializeField] private Transform activeParent;
    [SerializeField] private List<Image> barParents;
    [SerializeField] private List<Image> barFills;
    [SerializeField] private Image fillingDashBar;

    private bool playerDefeated;

    private void OnEnable()
    {
        playerDefeated = false;
        EventManager.PlayerStatsUpdatedEvent += OnPlayerStatsUpdated;
        EventManager.PlayerSpawnedEvent += OnPlayerSpawned;
        EventManager.StageStateStartedEvent += OnStageStateStarted;
        EventManager.UiMenuFocusChangedEvent += OnUiFocusChanged;
        EventManager.PlayerDefeatedEvent += OnPlayerDefeated;

        if (PlayerEntity.ActiveInstance != null)
        {
            OnPlayerSpawned(PlayerEntity.ActiveInstance);
            OnPlayerStatsUpdated(PlayerEntity.ActiveInstance);
        }

        UpdateVisibility();
    }
    private void OnDisable()
    {
        EventManager.PlayerStatsUpdatedEvent -= OnPlayerStatsUpdated;
        EventManager.PlayerSpawnedEvent -= OnPlayerSpawned;
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
        EventManager.UiMenuFocusChangedEvent -= OnUiFocusChanged;
        EventManager.PlayerDefeatedEvent -= OnPlayerDefeated;
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
    private void OnPlayerDefeated()
    {
        playerDefeated = true;
        UpdateVisibility();
    }
    private void OnPlayerSpawned(PlayerEntity p) 
    {
        activeParent.gameObject.SetActive(true);
    }
    private void OnPlayerStatsUpdated(PlayerEntity p) 
    {
        UpdateActiveBars((int)p.GetStat(PlayerStatGroup.Stat.DashCount));
    }
    private void UpdateActiveBars(int maxbars) 
    {
        for (int i = 0; i < barParents.Count; i++)
        {
            barParents[i].gameObject.SetActive(i < maxbars);
        }
    }
    private void Update()
    {
        if (!activeParent.gameObject.activeInHierarchy) { return; }

        int currentDashes = PlayerEntity.ActiveInstance.CurrentDashes;

        for (int i = 0; i < barFills.Count; i++) 
        {
            barFills[i].gameObject.SetActive(i < currentDashes);
        }
        fillingDashBar.fillAmount = PlayerEntity.ActiveInstance.DashRechargePercent;

        if (currentDashes < barParents.Count)
        {
            fillingDashBar.transform.position = barParents[currentDashes].transform.position;
            fillingDashBar.gameObject.SetActive(true);
        }
        else 
        {
            fillingDashBar.gameObject.SetActive(false);
        }

    }
}
