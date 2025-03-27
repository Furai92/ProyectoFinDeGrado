using UnityEngine;

public class PlayerHeatLightController : MonoBehaviour
{
    [SerializeField] private PlayerEntity p;
    [SerializeField] private Light l;

    private const float HEAT_TO_RANGE = 15f;
    private const float HEAT_TO_COLOR_LERP = 1f;

    private void Update()
    {
        float maxHeat = Mathf.Max(p.StatusHeatRanged, p.StatusHeatMelee);
        l.range = maxHeat * HEAT_TO_RANGE;
        l.color = Color.Lerp(Color.clear, Color.red, maxHeat * HEAT_TO_COLOR_LERP);
    }
}
