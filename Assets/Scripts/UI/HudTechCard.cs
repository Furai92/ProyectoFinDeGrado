using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HudTechCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
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
        transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseoverScaleParent.transform.localScale = Vector3.one;
    }
    private void OnEnable()
    {
        mouseoverScaleParent.transform.localScale = Vector3.one;
    }

    public void SetUp(TechSO t, int levelDisplayed, bool compareWithPrevious, bool showOwned) 
    {
        rarityHighlight.color = bottomBorder.color = cdb.RarityToColor(t.Rarity);
        traitHighlight.color = topBorder.color = t.RelatedTrait.IconColor;
        categoryText.text = sdb.GetString(t.RelatedTrait.NameID);
        rarityText.text = sdb.RarityToName(t.Rarity);
        techName.text = sdb.GetString(t.NameID);
        techDesc.text = sdb.GetString(t.DescID);
        if (t.MaxLevel > 0 && levelDisplayed > t.MaxLevel) { levelDisplayed = t.MaxLevel; }

        for (int i = 0; i < detailPanels.Count; i++) 
        {
            if (i < t.DescriptionDetails.Count)
            {
                detailPanels[i].gameObject.SetActive(true);
                detailTexts[i].color = cdb.DescDetailTypeToColor(t.DescriptionDetails[i].DescDetailType);

                if (t.DescriptionDetails[i].DescValueScaling != 0 || t.DescriptionDetails[i].DescValueBase != 0)
                {
                    if (compareWithPrevious && levelDisplayed > 1 && t.DescriptionDetails[i].DescValueScaling != 0)
                    {
                        float previousLevel = t.DescriptionDetails[i].DescValueBase + ((levelDisplayed-1) * t.DescriptionDetails[i].DescValueScaling);
                        float newLevel = t.DescriptionDetails[i].DescValueBase + levelDisplayed * t.DescriptionDetails[i].DescValueScaling;
                        string deltaShown = string.Format("{0} -> {1}", previousLevel, newLevel);
                        string stringDisplayed = sdb.GetString(t.DescriptionDetails[i].DescTextID);
                        detailTexts[i].text = string.Format(stringDisplayed, deltaShown);
                    }
                    else
                    {
                        float valueDisplayed;
                        if (t.DescriptionDetails[i].DescValueBase == 0)
                        {
                            valueDisplayed = levelDisplayed * t.DescriptionDetails[i].DescValueScaling;
                        }
                        else 
                        {
                            valueDisplayed = t.DescriptionDetails[i].DescValueBase + ((levelDisplayed - 1) * t.DescriptionDetails[i].DescValueScaling);
                        } 
                        detailTexts[i].text = string.Format(sdb.GetString(t.DescriptionDetails[i].DescTextID), valueDisplayed);
                    }
                }
                else 
                {
                    detailTexts[i].text = sdb.GetString(t.DescriptionDetails[i].DescTextID);
                }

            }
            else 
            {
                detailPanels[i].gameObject.SetActive(false);
            }

        }
        // Owned Text
        if (showOwned)
        {
            if (t.MaxLevel > 0)
            {
                ownedText.text = string.Format("X {0}/{1}", levelDisplayed, t.MaxLevel);
            }
            else
            {
                ownedText.text = string.Format("X {0}", levelDisplayed);
            }
        }
        else 
        {
            ownedText.text = "";
        }
    }
}
