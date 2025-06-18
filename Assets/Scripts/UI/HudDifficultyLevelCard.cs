using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HudDifficultyLevelCard : MonoBehaviour
{
    [SerializeField] private StringDatabaseSO sdb;
    [SerializeField] private ColorDatabaseSO cdb;
    [SerializeField] private TextMeshProUGUI difName;
    [SerializeField] private Image difPanel;

    private const float DIFFICULTY_COUNT = 10f; // Float for the lerp to work properly

    public void SetUp(DifficultyLevelSO d)
    {
        difName.text = string.Format("{0} {1}", sdb.GetString("DIF_BASE_NAME"), d.DifficultyIndex);
        difPanel.color = Color.Lerp(cdb.GetColor("DIF_LOW"), cdb.GetColor("DIF_HIGH"),d.DifficultyIndex / DIFFICULTY_COUNT);
    }
}
