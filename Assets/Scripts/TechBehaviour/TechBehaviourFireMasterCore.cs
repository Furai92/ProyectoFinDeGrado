using UnityEngine;

public class TechBehaviourFireMasterCore : TechBase
{
    private const float DMG_PER_HEAT = 0.01f;

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
                    if (PlayerEntity.ActiveInstance.StatusHeatMelee <= 0) { return; }
                    float bonusDamage = (PlayerEntity.ActiveInstance.StatusHeatMelee) * DMG_PER_HEAT * mag;
                    target.DealDirectDamage(bonusDamage, 0, 1, GameTools.IntellectToBuildupMultiplier(PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.Intellect)),
                        GameEnums.DamageElement.Fire, GameEnums.DamageType.Tech);
                    break;
                }
            case GameEnums.DamageType.Ranged:
                {
                    if (PlayerEntity.ActiveInstance.StatusHeatRanged <= 0) { return; }
                    float bonusDamage = (PlayerEntity.ActiveInstance.StatusHeatRanged) * DMG_PER_HEAT * mag;
                    target.DealDirectDamage(bonusDamage, 0, 1, GameTools.IntellectToBuildupMultiplier(PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.Intellect)),
                        GameEnums.DamageElement.Fire, GameEnums.DamageType.Tech);
                    break;
                }
        }
    }

    public override void OnTechUpgraded()
    {

    }
}
