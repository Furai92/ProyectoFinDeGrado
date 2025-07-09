using UnityEngine;
using TMPro;

public class BlinkingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI blinkingText;

    private const float BLINK_SPEED = 3f;

    private void Update()
    {
        blinkingText.alpha = Mathf.Abs(Mathf.Sin(Time.time * BLINK_SPEED));
    }

}
