using UnityEngine;

public class TechBehaviourLikeTheWind : TechBase
{
    private const float DMG_PER_SPEED = 50f;

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
        if (dtype != GameEnums.DamageType.Melee) { return; }

        float damageDone = (PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.Speed)-1) * DMG_PER_SPEED * Group.Level;

        if (damageDone <= 0) { return; }

        target.DealDirectDamage(damageDone, 0, 1, GameTools.IntellectToBuildupMultiplier(PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.Intellect)), GameEnums.DamageElement.Thunder, GameEnums.DamageType.Tech);
    }

    public override void OnTechUpgraded()
    {

    }
}
