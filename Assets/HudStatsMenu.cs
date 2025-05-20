using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class HudStatsMenu : MonoBehaviour, IGameMenu
{
    [SerializeField] private StringDatabaseSO sdb;
    [SerializeField] private Transform menuParent;
    [SerializeField] private List<TextMeshProUGUI> statLabels;
    [SerializeField] private List<TextMeshProUGUI> statValues;
    [SerializeField] private List<TextMeshProUGUI> statDescriptions;
    [SerializeField] private HudWeaponCard meleeWeapon;
    [SerializeField] private HudWeaponCard rangedWeapon;

    private void OnEnable()
    {
        EventManager.PlayerStatsUpdatedEvent += UpdateDisplayedInfo;
    }
    private void OnDisable()
    {
        EventManager.PlayerStatsUpdatedEvent -= UpdateDisplayedInfo;
    }
    private void UpdateDisplayedInfo(PlayerEntity p) 
    {
        if (!IsOpen()) { return; }

        meleeWeapon.SetUp(p.MeleeWeapon);
        rangedWeapon.SetUp(p.RangedWeapon);

        statValues[0].text = p.GetStat(PlayerStatGroup.Stat.MaxHealth).ToString("F0");
        statValues[1].text = p.GetStat(PlayerStatGroup.Stat.Might).ToString("F0");
        statValues[2].text = p.GetStat(PlayerStatGroup.Stat.Dexterity).ToString("F0");
        statValues[3].text = p.GetStat(PlayerStatGroup.Stat.Intellect).ToString("F0");
        statValues[4].text = p.GetStat(PlayerStatGroup.Stat.Endurance).ToString("F0");
        statValues[5].text = string.Format("{0}%", (p.GetStat(PlayerStatGroup.Stat.Speed) * 100).ToString("F0"));
        statValues[6].text = string.Format("{0}%", (p.GetStat(PlayerStatGroup.Stat.Firerate) * 100).ToString("F0"));
        statValues[7].text = string.Format("{0}%", (p.GetStat(PlayerStatGroup.Stat.CritChance)).ToString("F0"));
        statValues[8].text = string.Format("{0}%", (p.GetStat(PlayerStatGroup.Stat.CritBonusDamage) * 100).ToString("F0"));
        statValues[9].text = p.GetStat(PlayerStatGroup.Stat.DashCount).ToString("F0");
        statValues[10].text = string.Format("{0}%", (p.GetStat(PlayerStatGroup.Stat.DashRechargeRate) * 100).ToString("F0"));

        if (p.GetStat(PlayerStatGroup.Stat.Might) >= 0)
        {
            string valueDisplayed = ((GameTools.MightToDamageMultiplier(p.GetStat(PlayerStatGroup.Stat.Might))-1) * 100).ToString("F0");
            statDescriptions[0].text = string.Format(sdb.GetString("STAT_DESC_MIGHT_POSITIVE"), valueDisplayed);
        }
        else 
        {
            string valueDisplayed = ((1 - GameTools.MightToDamageMultiplier(p.GetStat(PlayerStatGroup.Stat.Might))) * 100).ToString("F0");
            statDescriptions[0].text = string.Format(sdb.GetString("STAT_DESC_MIGHT_NEGATIVE"), valueDisplayed);
        }

        if (p.GetStat(PlayerStatGroup.Stat.Dexterity) >= 0)
        {
            string valueDisplayed = ((GameTools.DexterityToDamageMultiplier(p.GetStat(PlayerStatGroup.Stat.Dexterity))-1) * 100).ToString("F0");
            statDescriptions[1].text = string.Format(sdb.GetString("STAT_DESC_DEX_POSITIVE"), valueDisplayed);
        }
        else
        {
            string valueDisplayed = ((1 - GameTools.DexterityToDamageMultiplier(p.GetStat(PlayerStatGroup.Stat.Dexterity))) * 100).ToString("F0");
            statDescriptions[1].text = string.Format(sdb.GetString("STAT_DESC_DEX_NEGATIVE"), valueDisplayed);
        }

        if (p.GetStat(PlayerStatGroup.Stat.Intellect) >= 0)
        {
            string valueDisplayed = ((GameTools.IntellectToBuildupMultiplier(p.GetStat(PlayerStatGroup.Stat.Intellect))-1) * 100).ToString("F0");
            statDescriptions[2].text = string.Format(sdb.GetString("STAT_DESC_INT_POSITIVE"), valueDisplayed);
        }
        else
        {
            string valueDisplayed = ((1 - GameTools.IntellectToBuildupMultiplier(p.GetStat(PlayerStatGroup.Stat.Intellect))) * 100).ToString("F0");
            statDescriptions[2].text = string.Format(sdb.GetString("STAT_DESC_INT_NEGATIVE"), valueDisplayed);
        }

        if (p.GetStat(PlayerStatGroup.Stat.Endurance) >= 0)
        {
            string valueDisplayed = ((1 - GameTools.EnduranceToDamageMultiplier(p.GetStat(PlayerStatGroup.Stat.Endurance))) * 100).ToString("F0");
            statDescriptions[3].text = string.Format(sdb.GetString("STAT_DESC_END_POSITIVE"), valueDisplayed);
        }
        else
        {
            string valueDisplayed = ((GameTools.EnduranceToDamageMultiplier(p.GetStat(PlayerStatGroup.Stat.Endurance))-1) * 100).ToString("F0");
            statDescriptions[3].text = string.Format(sdb.GetString("STAT_DESC_END_NEGATIVE"), valueDisplayed);
        }

    }


    public bool CanBeOpened()
    {
        return PlayerEntity.ActiveInstance != null;
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
        UpdateDisplayedInfo(PlayerEntity.ActiveInstance);
    }
}
