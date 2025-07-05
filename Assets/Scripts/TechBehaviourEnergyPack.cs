using UnityEngine;

public class TechBehaviourEnergyPack : TechBase
{
    private const float DASH_CHARGES_RESTORED = 0.2f;


    public override void OnTechAdded()
    {
        EventManager.PlayerHealthOrbGatheredEvent += OnPlayerHealthOrbGathered;

    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerHealthOrbGatheredEvent -= OnPlayerHealthOrbGathered;
    }

    private void OnPlayerHealthOrbGathered(Vector3 wpos) 
    {
        PlayerEntity.ActiveInstance.RestoreDashCharges(DASH_CHARGES_RESTORED * Group.Level);
    }

    public override void OnTechUpgraded()
    {

    }
}
