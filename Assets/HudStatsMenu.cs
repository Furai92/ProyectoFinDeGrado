using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class HudStatsMenu : IngameMenuBase
{
    [SerializeField] private Transform menuParent;
    [SerializeField] private List<TextMeshProUGUI> statLabels;
    [SerializeField] private List<TextMeshProUGUI> statValues;

    private PlayerEntity playerRef;

    public void SetUp(PlayerEntity p)
    {
        gameObject.SetActive(true);
        playerRef = p;
        UpdateDisplayedInfo(p);
        EventManager.PlayerStatsUpdatedEvent += UpdateDisplayedInfo;
    }
    private void OnDisable()
    {
        EventManager.PlayerStatsUpdatedEvent -= UpdateDisplayedInfo;
    }
    private void UpdateDisplayedInfo(PlayerEntity p) 
    {
        if (!IsOpen()) { return; }

        statValues[0].text = p.GetStat(PlayerStatGroup.Stat.MaxHealth).ToString("F0");
        statValues[1].text = p.GetStat(PlayerStatGroup.Stat.Might).ToString("F0");
        statValues[2].text = p.GetStat(PlayerStatGroup.Stat.Dexterity).ToString("F0");
        statValues[3].text = p.GetStat(PlayerStatGroup.Stat.Intellect).ToString("F0");
        statValues[4].text = string.Format("{0}%", (p.GetStat(PlayerStatGroup.Stat.Speed) * 100).ToString("F0"));
        statValues[5].text = string.Format("{0}%", (p.GetStat(PlayerStatGroup.Stat.Firerate) * 100).ToString("F0"));
        statValues[6].text = string.Format("{0}%", (p.GetStat(PlayerStatGroup.Stat.CritChance)).ToString("F0"));
        statValues[7].text = string.Format("{0}%", (p.GetStat(PlayerStatGroup.Stat.CritBonusDamage) * 100).ToString("F0"));
        statValues[8].text = p.GetStat(PlayerStatGroup.Stat.DashCount).ToString("F0");
        statValues[9].text = string.Format("{0}%", (p.GetStat(PlayerStatGroup.Stat.DashRechargeRate) * 100).ToString("F0"));
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
        UpdateDisplayedInfo(playerRef);
    }
}
