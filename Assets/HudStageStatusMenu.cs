using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class HudStageStatusMenu : IngameMenuBase
{
    [SerializeField] private Transform menuParent;
    [SerializeField] private List<TextMeshProUGUI> statLabels;
    [SerializeField] private List<TextMeshProUGUI> statValues;

    public void SetUp()
    {
        gameObject.SetActive(true);
        UpdateDisplayedInfo();
        EventManager.StageStatsUpdatedEvent += UpdateDisplayedInfo;
    }
    private void OnDisable()
    {
        EventManager.StageStatsUpdatedEvent -= UpdateDisplayedInfo;
    }
    private void UpdateDisplayedInfo()
    {
        if (!IsOpen()) { return; }

        StageStatGroup ssg = StageManagerBase.GetStageStats();

        statValues[0].text = string.Format("{0}%", (ssg.GetStat(StageStatGroup.StageStat.EnemyDamageMult) * 100).ToString("F0"));
        statValues[1].text = string.Format("{0}%", (ssg.GetStat(StageStatGroup.StageStat.EnemyHealthMult) * 100).ToString("F0"));
        statValues[2].text = string.Format("{0}%", (ssg.GetStat(StageStatGroup.StageStat.EnemySpawnRate) * 100).ToString("F0"));
    }


    public override bool CanBeOpened()
    {
        return true; // Can always be opened, has no wave type or gamestate restrictions
    }

    public override void CloseMenu()
    {
        menuParent.gameObject.SetActive(false);
    }

    public override bool IsOpen()
    {
        return menuParent.gameObject.activeInHierarchy;
    }

    public override void OpenMenu()
    {
        menuParent.gameObject.SetActive(true);
        UpdateDisplayedInfo();
    }
}
