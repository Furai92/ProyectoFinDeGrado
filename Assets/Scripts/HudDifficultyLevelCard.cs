using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HudDifficultyLevelCard : MonoBehaviour
{
    [SerializeField] private StringDatabaseSO sdb;
    [SerializeField] private TextMeshProUGUI difName;
    [SerializeField] private Image difPanel;

    public void SetUp(DifficultyLevelSO d)
    {
        difName.text = sdb.GetString(d.NameID);
        difPanel.color = d.IconColor;
    }
}
