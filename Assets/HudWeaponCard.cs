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

    [SerializeField] private Transform bonusPartsHeaderParent;
    [SerializeField] private List<Transform> bonusPartParents;
    [SerializeField] private List<TextMeshProUGUI> bonusPartTexts;

    [SerializeField] private Image elementHighlight;
    [SerializeField] private Image rarityHighlight;
    [SerializeField] private TextMeshProUGUI rarityText;

    public void SetUp(WeaponData w) 
    {
        // Name panel
        nameText.text = sdb.GetString(w.BaseWeapon.NameID);

        // Slot panel
        slotText.text = w.BaseWeapon.Slot == WeaponSO.WeaponSlot.Melee ? sdb.GetString("WEAPON_SLOT_NAME_MELEE") : sdb.GetString("WEAPON_SLOT_NAME_RANGED");

        // Element panel
        Color elementColor = cdb.ElementToColor(w.Stats.Element);
        elementNameText.text = sdb.ElementToName(w.Stats.Element);
        elementDescText.text = sdb.ElementToDesc(w.Stats.Element);
        elementHighlight.color = elementColor;
        elementPanelBackground.color = new Color(elementColor.r, elementColor.g, elementColor.b, elementPanelBackground.color.a);

        // Rarity
        rarityText.text = sdb.RarityToName(w.Rarity);
        rarityHighlight.color = cdb.RarityToColor(w.Rarity);

        // Stats panel
        if (w.BaseWeapon.Slot == WeaponSO.WeaponSlot.Melee)
        {
            damageText.text = string.Format(sdb.GetString("WEAPON_CARD_DMG_MELEE_FORMAT"),
                w.Stats.CleaveComponentMagnitude.ToString("F0"));
        } 
        else 
        {
            damageText.text = string.Format(sdb.GetString("WEAPON_CARD_DMG_RANGED_FORMAT"),
                w.Stats.ProjectileComponentMagnitude.ToString("F0"),
                w.Stats.Multishoot.ToString("F0"));
        }
        firerateText.text = string.Format(sdb.GetString("WEAPON_CARD_FIRERATE_FORMAT"), w.Stats.Firerate.ToString("F1"));
        buildupText.text = string.Format(sdb.GetString("WEAPON_CARD_BUILDUP_FORMAT"), (w.Stats.BuildupRate * 100).ToString("F0"));
        critDamageText.text = string.Format(sdb.GetString("WEAPON_CARD_CRITX_FORMAT"), (w.Stats.CritMultiplier * 100).ToString("F0"));

        // Parts panel
        bonusPartsHeaderParent.gameObject.SetActive(w.SpecialParts.Count > 0);

        for (int i = 0; i < bonusPartParents.Count; i++) 
        {
            if (i < w.SpecialParts.Count)
            {
                bonusPartTexts[i].text = sdb.GetString(w.SpecialParts[i].DescID);
                bonusPartParents[i].gameObject.SetActive(true);
            }
            else 
            {
                bonusPartParents[i].gameObject.SetActive(false);
            }
        }
    }
}
