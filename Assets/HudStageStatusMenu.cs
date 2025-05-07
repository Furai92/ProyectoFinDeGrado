using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class HudStageStatusMenu : MonoBehaviour, IGameMenu
{
    [SerializeField] private Transform menuParent;
    [SerializeField] private List<TextMeshProUGUI> statLabels;
    [SerializeField] private List<TextMeshProUGUI> statValues;

    private void OnEnable()
    {
        EventManager.StageStatsUpdatedEvent += UpdateDisplayedInfo;
    }
    private void OnDisable()
    {
        EventManager.StageStatsUpdatedEvent -= UpdateDisplayedInfo;
    }
    private void UpdateDisplayedInfo()
    {
        if (!IsOpen()) { return; }

        statValues[0].text = string.Format("{0}%", (StageManagerBase.GetStageStat(StageStatGroup.StageStat.EnemyDamageMult) * 100).ToString("F0"));
        statValues[1].text = string.Format("{0}%", (StageManagerBase.GetStageStat(StageStatGroup.StageStat.EnemyHealthMult) * 100).ToString("F0"));
        statValues[2].text = string.Format("{0}%", (StageManagerBase.GetStageStat(StageStatGroup.StageStat.EnemySpawnRate) * 100).ToString("F0"));
    }


    public bool CanBeOpened()
    {
        switch (StageManagerBase.GetCurrentStateType())
        {
            case StageStateBase.GameState.Rest:
            case StageStateBase.GameState.EnemyWave:
            case StageStateBase.GameState.BossFight:
                {
                    return true;
                }
            default:
                {
                    return false;
                }
        }
    }

    public void CloseMenu()
    {
        EventManager.OnUiMenuClosed(this);
        menuParent.gameObject.SetActive(false);
    }

    public bool IsOpen()
    {
        return menuParent.gameObject.activeInHierarchy;
    }

    public void OpenMenu()
    {
        menuParent.gameObject.SetActive(true);
        UpdateDisplayedInfo();
    }
}
