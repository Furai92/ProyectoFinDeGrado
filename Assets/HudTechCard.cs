using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HudTechCard : MonoBehaviour
{
    [SerializeField] private StringDatabaseSO sdb;
    [SerializeField] private TextMeshProUGUI techName;
    [SerializeField] private TextMeshProUGUI techDesc;
    [SerializeField] private TextMeshProUGUI ownedText;
    [SerializeField] private List<TextMeshProUGUI> detailTexts;
    [SerializeField] private List<Transform> detailPanels;

    public void SetUp(TechSO t) 
    {
        techName.text = sdb.GetString(t.NameID);
        techDesc.text = sdb.GetString(t.DescID);
        ownedText.gameObject.SetActive(false);
        for (int i = 0; i < detailPanels.Count; i++) 
        {
            // TEMP
            detailPanels[i].gameObject.SetActive(false);
        }
    }
}
