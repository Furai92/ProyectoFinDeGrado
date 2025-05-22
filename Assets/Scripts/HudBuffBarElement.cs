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
        stackCountText.text = string.Format("x{0}", b.Stacks.ToString("F0"));
        durationText.text = b.Infinite ? "" : (b.GetDurationRemaining()+1).ToString("F0");
        icon.color = b.SO.BuffColor;
        icon.sprite = b.SO.Icon;
    }
}
