using UnityEngine;

public class TechBehaviourThunderMasterCore : TechBase
{
    private const float BONUS_CRIT_DAMAGE = 0.5f;

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
        if (clevel < 1) { return; }

        target.DealDirectDamage(mag * BONUS_CRIT_DAMAGE, 0, 1, GameTools.IntellectToBuildupMultiplier(PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.Intellect)),
            GameEnums.DamageElement.Thunder, GameEnums.DamageType.Tech);
    }

    public override void OnTechUpgraded()
    {

    }
}
