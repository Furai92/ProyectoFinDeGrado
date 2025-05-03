using UnityEngine;

public class TechBehaviourInfusion : TechBase
{
    private const float HP_GAIN = 1f;

    public override void OnTechAdded()
    {
        EventManager.PlayerHealthOrbGatheredEvent += OnHealthOrbGathered;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerHealthOrbGatheredEvent -= OnHealthOrbGathered;
    }

    private void OnHealthOrbGathered(Vector3 pos)
    {
        PlayerEntity.ActiveInstance.ChangePermanentStat(PlayerStatGroup.Stat.MaxHealth, HP_GAIN * Group.Level);
        PlayerEntity.ActiveInstance.Heal(HP_GAIN);
    }

    public override void OnTechUpgraded()
    {

    }
}
