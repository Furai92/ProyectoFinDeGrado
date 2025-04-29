using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class HudTechCard : MonoBehaviour
{
    [SerializeField] private StringDatabaseSO sdb;
    [SerializeField] private ColorDatabaseSO cdb;
    [SerializeField] private TextMeshProUGUI techName;
    [SerializeField] private TextMeshProUGUI techDesc;
    [SerializeField] private TextMeshProUGUI ownedText;
    [SerializeField] private List<TextMeshProUGUI> detailTexts;
    [SerializeField] private List<Transform> detailPanels;
    [SerializeField] private Image traitHighlight;
    [SerializeField] private Image rarityHighlight;
    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField] private TextMeshProUGUI rarityText;


    public void SetUp(TechSO t, bool showOwned) 
    {
        rarityHighlight.color = cdb.TechRarityToColor(t.Rarity);
        traitHighlight.color = t.RelatedTrait.IconColor;
        categoryText.text = sdb.GetString(t.RelatedTrait.NameID);
        rarityText.text = sdb.RarityToName(t.Rarity);
        techName.text = sdb.GetString(t.NameID);
        techDesc.text = sdb.GetString(t.DescID);
        for (int i = 0; i < detailPanels.Count; i++) 
        {
            // TEMP
            detailPanels[i].gameObject.SetActive(false);
        }
        if (showOwned && PlayerEntity.ActiveInstance != null) 
        {
            if (t.MaxLevel > 0)
            {
                ownedText.text = string.Format("X {0}/{1}", PlayerEntity.ActiveInstance.GetTechLevel(t.ID), t.MaxLevel);
            } 
            else 
            {
                ownedText.text = string.Format("X {0}", PlayerEntity.ActiveInstance.GetTechLevel(t.ID));
            }

        }
        else 
        {
            ownedText.text = "";
        }
    }
}
