using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HudBuffBar : MonoBehaviour
{
    [SerializeField] private Transform activeParent;
    [SerializeField] private List<HudBuffBarElement> buffBarElements;

    private bool playerDefeated;
    private float buffUpdateTime;

    private const float BUFF_UPDATE_INTERVAL = 0.2f;


    private void OnEnable()
    {
        playerDefeated = false;
        EventManager.PlayerSpawnedEvent += OnPlayerSpawned;
        EventManager.StageStateStartedEvent += OnStageStateStarted;
        EventManager.UiMenuFocusChangedEvent += OnUiFocusChanged;
        EventManager.PlayerDefeatedEvent += OnPlayerDefeated;

        if (PlayerEntity.ActiveInstance != null)
        {
            OnPlayerSpawned(PlayerEntity.ActiveInstance);
        }
        buffUpdateTime = Time.time + BUFF_UPDATE_INTERVAL;
        UpdateBuffs();
        UpdateVisibility();
    }
    private void OnDisable()
    {
        EventManager.PlayerSpawnedEvent -= OnPlayerSpawned;
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
        EventManager.UiMenuFocusChangedEvent -= OnUiFocusChanged;
        EventManager.PlayerDefeatedEvent -= OnPlayerDefeated;
    }
    private void OnPlayerDefeated() 
    {
        playerDefeated = true;
        UpdateVisibility();
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
    private void OnPlayerSpawned(PlayerEntity p)
    {
        activeParent.gameObject.SetActive(true);
    }
    private void UpdateBuffs() 
    {
        if (PlayerEntity.ActiveInstance == null) { return; }

        for (int i = 0; i < buffBarElements.Count; i++) 
        {
            if (i < PlayerEntity.ActiveInstance.ActiveBuffs.Count)
            {
                buffBarElements[i].gameObject.SetActive(true);
                buffBarElements[i].SetUp(PlayerEntity.ActiveInstance.ActiveBuffs[i]);
            }
            else 
            {
                buffBarElements[i].gameObject.SetActive(false);
            }
        }
    }
    private void Update()
    {
        if (!activeParent.gameObject.activeInHierarchy) { return; }

        if (Time.time > buffUpdateTime) 
        {
            UpdateBuffs();
            buffUpdateTime = Time.time + BUFF_UPDATE_INTERVAL;
        }
    }
}
