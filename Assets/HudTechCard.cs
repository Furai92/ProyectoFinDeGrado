using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HudTechCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum TechDisplayMode { Collection, Shop, ActiveTech }

    [SerializeField] private Transform mouseoverScaleParent;

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
    [SerializeField] private Image topBorder;
    [SerializeField] private Image bottomBorder;

    private const float MOUSEOVER_SCALE = 1.3f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseoverScaleParent.transform.localScale = Vector3.one * MOUSEOVER_SCALE;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseoverScaleParent.transform.localScale = Vector3.one;
    }
    private void OnEnable()
    {
        mouseoverScaleParent.transform.localScale = Vector3.one;
    }

    public void SetUp(TechSO t, TechDisplayMode mode) 
    {
        rarityHighlight.color = bottomBorder.color = cdb.RarityToColor(t.Rarity);
        traitHighlight.color = topBorder.color = t.RelatedTrait.IconColor;
        categoryText.text = sdb.GetString(t.RelatedTrait.NameID);
        rarityText.text = sdb.RarityToName(t.Rarity);
        techName.text = sdb.GetString(t.NameID);
        techDesc.text = sdb.GetString(t.DescID);

        int maxTechLevel = t.MaxLevel;
        int currentTechLevel = 0;
        switch (mode) 
        {
            case TechDisplayMode.Collection: { currentTechLevel = 1; break; }
            case TechDisplayMode.Shop: { currentTechLevel = PlayerEntity.ActiveInstance.GetTechLevel(t.ID) + 1; break; }
            case TechDisplayMode.ActiveTech: { currentTechLevel = PlayerEntity.ActiveInstance.GetTechLevel(t.ID); break; }
        }


        for (int i = 0; i < detailPanels.Count; i++) 
        {
            if (i < t.DescriptionDetails.Count)
            {
                detailPanels[i].gameObject.SetActive(true);
                float valueDisplayed = t.DescriptionDetails[i].DescValueBase + currentTechLevel * t.DescriptionDetails[i].DescValueScaling;
                string stringDisplayed = sdb.GetString(t.DescriptionDetails[i].DescTextID);
                detailTexts[i].text = string.Format(stringDisplayed, valueDisplayed);
                detailTexts[i].color = cdb.DescDetailTypeToColor(t.DescriptionDetails[i].DescDetailType);
            }
            else 
            {
                detailPanels[i].gameObject.SetActive(false);
            }

        }
        if (mode != TechDisplayMode.Collection && PlayerEntity.ActiveInstance != null) 
        {
            if (maxTechLevel > 0)
            {
                ownedText.text = string.Format("X {0}/{1}", currentTechLevel, maxTechLevel);
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
