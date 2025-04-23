using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HudWeaponCard : MonoBehaviour
{
    [SerializeField] private StringDatabaseSO sdb;
    [SerializeField] private ColorDatabaseSO cdb;

    [SerializeField] private TextMeshProUGUI nameText;

    [SerializeField] private TextMeshProUGUI slotText;

    [SerializeField] private TextMeshProUGUI elementNameText;
    [SerializeField] private TextMeshProUGUI elementDescText;
    [SerializeField] private Image elementPanelBackground;

    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI firerateText;
    [SerializeField] private TextMeshProUGUI buildupText;
    [SerializeField] private TextMeshProUGUI critDamageText;

    [SerializeField] private List<Transform> bonusStatParents;
    [SerializeField] private List<TextMeshProUGUI> bonusStatTexts;

    public void SetUp(WeaponData w) 
    {
        // Name panel
        nameText.text = sdb.GetString(w.BaseWeapon.NameID);

        // Slot panel
        slotText.text = w.BaseWeapon.Slot == WeaponSO.WeaponSlot.Melee ? sdb.GetString("WEAPON_SLOT_NAME_MELEE") : sdb.GetString("WEAPON_SLOT_NAME_RANGED");

        // Element panel
        elementNameText.text = sdb.ElementToName(w.GetStats().Element);
        elementDescText.text = sdb.ElementToDesc(w.GetStats().Element);
        Color elementColor = cdb.ElementToColor(w.GetStats().Element);
        elementPanelBackground.color = new Color(elementColor.r, elementColor.g, elementColor.b, elementPanelBackground.color.a);

        // Stats panel
        if (w.BaseWeapon.Slot == WeaponSO.WeaponSlot.Melee)
        {
            damageText.text = string.Format(sdb.GetString("WEAPON_CARD_DMG_MELEE_FORMAT"),
                w.GetStats().CleaveComponentMagnitude.ToString("F0"));
        } 
        else 
        {
            damageText.text = string.Format(sdb.GetString("WEAPON_CARD_DMG_RANGED_FORMAT"),
                w.GetStats().ProjectileComponentMagnitude.ToString("F0"),
                w.GetStats().Multishoot.ToString("F0"));
        }
        firerateText.text = string.Format(sdb.GetString("WEAPON_CARD_FIRERATE_FORMAT"), w.GetStats().Firerate.ToString("F1"));
        buildupText.text = string.Format(sdb.GetString("WEAPON_CARD_BUILDUP_FORMAT"), (w.GetStats().BuildupRate * 100).ToString("F0"));
        critDamageText.text = string.Format(sdb.GetString("WEAPON_CARD_CRITX_FORMAT"), (w.GetStats().CritMultiplier * 100).ToString("F0"));

        // Bonus stats panel
        bonusStatParents[0].gameObject.SetActive(w.GetStats().Bounces > 0 && w.BaseWeapon.Slot == WeaponSO.WeaponSlot.Ranged);
        bonusStatTexts[0].text = string.Format(sdb.GetString("WEAPON_CARD_BOUNCES_FORMAT"), (w.GetStats().Bounces).ToString("F0"));
        bonusStatParents[1].gameObject.SetActive(w.GetStats().Pierces > 0 && w.BaseWeapon.Slot == WeaponSO.WeaponSlot.Ranged);
        bonusStatTexts[1].text = string.Format(sdb.GetString("WEAPON_CARD_PIERCES_FORMAT"), (w.GetStats().Pierces).ToString("F0"));
        bonusStatParents[2].gameObject.SetActive(w.GetStats().Splash > 0 && w.BaseWeapon.Slot == WeaponSO.WeaponSlot.Ranged);
        bonusStatTexts[2].text = string.Format(sdb.GetString("WEAPON_CARD_SPLASH_FORMAT"), (w.GetStats().Splash).ToString("F0"));

        bonusStatParents[3].gameObject.SetActive(false); // TEMP
    }
}
