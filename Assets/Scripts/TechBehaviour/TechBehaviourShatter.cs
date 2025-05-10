using UnityEngine;

public class TechBehaviourShatter : TechBase
{
    private const float MAG_TO_BONUS_DAMAGE_RATIO = 0.1f;

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
        if (target.GetStatusDuration(GameEnums.DamageElement.Frost) <= 0) { return; }

        target.DealDirectDamage(mag * MAG_TO_BONUS_DAMAGE_RATIO * Group.Level, 0, 1, 0, GameEnums.DamageElement.NonElemental, GameEnums.DamageType.Tech);
    }

    public override void OnTechUpgraded()
    {

    }
}
