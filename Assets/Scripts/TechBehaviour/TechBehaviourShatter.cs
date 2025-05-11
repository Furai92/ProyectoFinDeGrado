using UnityEngine;

public class TechBehaviourShatter : TechBase
{
    private const float DURATION_TO_DAMAGE = 250f;

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
        if (target.GetStatusDuration(GameEnums.DamageElement.Frost) <= 0) { return; }

        target.DealDirectDamage(DURATION_TO_DAMAGE * target.GetStatusDuration(GameEnums.DamageElement.Frost) * Group.Level, 0, 1, 0, GameEnums.DamageElement.NonElemental, GameEnums.DamageType.Tech);
        target.RemoveStatus(GameEnums.DamageElement.Frost);
    }

    public override void OnTechUpgraded()
    {

    }
}
