using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudPlayerStatusManager : MonoBehaviour
{
    [SerializeField] private Image heatGaugeRanged;
    [SerializeField] private Image heatGaugeMelee;
    [SerializeField] private TextMeshProUGUI heatPercentRanged;
    [SerializeField] private TextMeshProUGUI heatWarningRanged;
    [SerializeField] private TextMeshProUGUI overheatRanged;

    private PlayerEntity playerReference;

    public void SetUp(PlayerEntity p) 
    {
        playerReference = p;
        gameObject.SetActive(true);
    }
    private void Update()
    {
        UpdateHeat();
    }
    private void UpdateHeat() 
    {
        heatGaugeRanged.fillAmount = Mathf.Lerp(0, 0.5f, playerReference.StatusHeatRanged);
        heatGaugeRanged.color = heatPercentRanged.color = Color.Lerp(Color.blue, Color.red, playerReference.StatusHeatRanged);
        heatPercentRanged.text = string.Format("{0}%", (playerReference.StatusHeatRanged*100).ToString("F1"));
        heatWarningRanged.gameObject.SetActive(playerReference.StatusHeatRanged > 0.8f);
        overheatRanged.gameObject.SetActive(playerReference.StatusOverheatRanged);
    }
}
