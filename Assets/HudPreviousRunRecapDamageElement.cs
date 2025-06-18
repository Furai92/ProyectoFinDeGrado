using UnityEngine;
using TMPro;

public class HudPreviousRunRecapDamageElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI directText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private StringDatabaseSO sdb;
    [SerializeField] private ColorDatabaseSO cdb;

    public void SetUp(GameEnums.DamageElement e, float direct, float status) 
    {
        gameObject.SetActive(direct != 0 || status != 0);

        directText.text = direct.ToString("F0");
        statusText.text = status.ToString("F0");
        typeText.text = sdb.ElementToName(e);
        typeText.colorGradientPreset = statusText.colorGradientPreset = directText.colorGradientPreset = cdb.ElementToGradient(e);
    }
}
