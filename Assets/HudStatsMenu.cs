using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class HudStatsMenu : MonoBehaviour, IGameMenu
{
    [SerializeField] private Transform menuParent;
    [SerializeField] private List<TextMeshProUGUI> statLabels;
    [SerializeField] private List<TextMeshProUGUI> statValues;
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
        statValues[4].text = string.Format("{0}%", (p.GetStat(PlayerStatGroup.Stat.Speed) * 100).ToString("F0"));
        statValues[5].text = string.Format("{0}%", (p.GetStat(PlayerStatGroup.Stat.Firerate) * 100).ToString("F0"));
        statValues[6].text = string.Format("{0}%", (p.GetStat(PlayerStatGroup.Stat.CritChance)).ToString("F0"));
        statValues[7].text = string.Format("{0}%", (p.GetStat(PlayerStatGroup.Stat.CritBonusDamage) * 100).ToString("F0"));
        statValues[8].text = p.GetStat(PlayerStatGroup.Stat.DashCount).ToString("F0");
        statValues[9].text = string.Format("{0}%", (p.GetStat(PlayerStatGroup.Stat.DashRechargeRate) * 100).ToString("F0"));
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
