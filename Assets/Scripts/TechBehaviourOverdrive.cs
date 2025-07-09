using UnityEngine;

public class TechBehaviourOverdrive : TechBase
{

    public override void OnTechAdded()
    {
        EventManager.PlayerHeatNegatvedEvent += OnHeatNegated;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerHeatNegatvedEvent -= OnHeatNegated;
    }

    private void OnHeatNegated(float h) 
    {
        PlayerEntity.ActiveInstance.DealDamage(h, false, true, true);
    }

    public override void OnTechUpgraded()
    {

    }
}
