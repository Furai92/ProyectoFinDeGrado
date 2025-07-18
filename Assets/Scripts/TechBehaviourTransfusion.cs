using UnityEngine;

public class TechBehaviourTransfusion : TechBase
{
    private const float PREV_HEALING_TO_SHIELD_RATE = 3f;

    public override void OnTechAdded()
    {
        EventManager.PlayerHealingNegatedEvent += OnHealingPrevented;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerHealingNegatedEvent -= OnHealingPrevented;
    }

    private void OnHealingPrevented(float h) 
    {
        PlayerEntity.ActiveInstance.AddShield(h * PREV_HEALING_TO_SHIELD_RATE);
    }

    public override void OnTechUpgraded()
    {

    }
}
