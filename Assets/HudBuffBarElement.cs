using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HudBuffBarElement : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI stackCountText;
    [SerializeField] private TextMeshProUGUI durationText;

    public void SetUp(PlayerEntity.ActiveBuff b) 
    {
        stackCountText.text = b.Stacks.ToString("F0");
        durationText.text = (b.GetDurationRemaining()+1).ToString("F0");
        icon.sprite = b.SO.Icon;
    }
}
