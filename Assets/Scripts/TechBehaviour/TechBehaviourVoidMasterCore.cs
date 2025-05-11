using UnityEngine;

public class TechBehaviourVoidMasterCore : TechBase
{
    private const float SPLASH_TO_DAMAGE_PERCENT = 5f;

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
        switch (dtype) 
        {
            case GameEnums.DamageType.Melee:
                {
                    float damage = mag * SPLASH_TO_DAMAGE_PERCENT * PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.BonusSplash) + PlayerEntity.ActiveInstance.MeleeWeapon.Stats.Splash;
                    if (damage <= 0) { return; }
                    target.DealDirectDamage(damage, 0, 1, GameTools.IntellectToBuildupMultiplier(PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.Intellect)), GameEnums.DamageElement.Void, GameEnums.DamageType.Tech);
                    break;
                }
            case GameEnums.DamageType.Ranged: 
                {
                    float damage = mag * SPLASH_TO_DAMAGE_PERCENT * PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.BonusSplash) + PlayerEntity.ActiveInstance.RangedWeapon.Stats.Splash;
                    if (damage <= 0) { return; }
                    target.DealDirectDamage(damage, 0, 1, GameTools.IntellectToBuildupMultiplier(PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.Intellect)), GameEnums.DamageElement.Void, GameEnums.DamageType.Tech);
                    break;
                }
        }
    }

    public override void OnTechUpgraded()
    {

    }
}
