using UnityEngine;

public class TechBehaviourVoidTouch : TechBase
{
    private const float NEGATIVE_END_TO_DAMAGE = 0.1f;

    public override void OnTechAdded()
    {
        EventManager.EnemyDirectDamageTakenEvent += OnEnemyDamageTaken;
    }

    public override void OnTechRemoved()
    {
        EventManager.EnemyDirectDamageTakenEvent -= OnEnemyDamageTaken;
    }

    private void OnEnemyDamageTaken(float mag, int clevel, GameEnums.DamageElement elem, GameEnums.DamageType dtype, EnemyEntity target)
    {
        if (dtype != GameEnums.DamageType.Melee && dtype != GameEnums.DamageType.Ranged) { return; }
        if (PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.Endurance) >= 0) { return; }

        target.DealDirectDamage(NEGATIVE_END_TO_DAMAGE * Group.Level * -PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.Endurance), 0, 1,
            GameTools.IntellectToBuildupMultiplier(PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.Intellect)), GameEnums.DamageElement.Void, GameEnums.DamageType.Tech);
    }

    public override void OnTechUpgraded()
    {

    }
}
