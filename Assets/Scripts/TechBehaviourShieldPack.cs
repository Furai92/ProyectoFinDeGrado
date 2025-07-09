using UnityEngine;

public class TechBehaviourShieldPack : TechBase
{
    private const float SHIELD_RESTORED = 20f;


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
        PlayerEntity.ActiveInstance.AddShield(SHIELD_RESTORED * Group.Level);
    }

    public override void OnTechUpgraded()
    {

    }
}
