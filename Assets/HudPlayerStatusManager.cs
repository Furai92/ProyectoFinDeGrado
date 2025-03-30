using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudPlayerStatusManager : MonoBehaviour
{
    [Header("Ranged Weapon")]
    [SerializeField] private Image heatGaugeRanged;
    [SerializeField] private TextMeshProUGUI heatPercentRanged;
    [SerializeField] private TextMeshProUGUI heatWarningRanged;
    [SerializeField] private TextMeshProUGUI overheatRanged;

    [Header("Melee Weapon")]
    [SerializeField] private Image heatGaugeMelee;
    [SerializeField] private TextMeshProUGUI heatPercentMelee;
    [SerializeField] private TextMeshProUGUI heatWarningMelee;
    [SerializeField] private TextMeshProUGUI overheatMelee;

    [Header("Health Bar")]
    [SerializeField] private Image healthFill;
    [SerializeField] private Image healthFillTrail;

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
        // Ranged
        heatGaugeRanged.fillAmount = Mathf.Lerp(0, 0.5f, playerReference.StatusHeatRanged);
        heatGaugeRanged.color = heatPercentRanged.color = Color.Lerp(Color.blue, Color.red, playerReference.StatusHeatRanged);
        heatPercentRanged.text = string.Format("{0}%", (playerReference.StatusHeatRanged*100).ToString("F1"));
        heatWarningRanged.gameObject.SetActive(playerReference.StatusHeatRanged > 0.8f);
        overheatRanged.gameObject.SetActive(playerReference.StatusOverheatRanged);

        // Melee
        heatGaugeMelee.fillAmount = Mathf.Lerp(0, 0.5f, playerReference.StatusHeatMelee);
        heatGaugeMelee.color = heatPercentMelee.color = Color.Lerp(Color.blue, Color.red, playerReference.StatusHeatMelee);
        heatPercentMelee.text = string.Format("{0}%", (playerReference.StatusHeatMelee * 100).ToString("F1"));
        heatWarningMelee.gameObject.SetActive(playerReference.StatusHeatMelee > 0.8f);
        overheatMelee.gameObject.SetActive(playerReference.StatusOverheatMelee);
    }
}
