using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HudTraitCard : MonoBehaviour
{
    [SerializeField] private StringDatabaseSO sdb;
    [SerializeField] private TextMeshProUGUI traitName;
    [SerializeField] private Image traitPanel;

    public void SetUp(PlayerTraitSO t) 
    {
        traitName.text = sdb.GetString(t.NameID);
        traitPanel.color = t.IconColor;
    }
}
