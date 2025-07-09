using UnityEngine;

public class TechBehaviourKineticBlast : TechBase
{
    private const float HP_TO_DAMAGE_RATIO = 0.1f;
    private const float RADIUS = 5f;

    public override void OnTechAdded()
    {
        EventManager.PlayerDamageTakenEvent += OnPlayerDamageTaken;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerDamageTakenEvent -= OnPlayerDamageTaken;
    }

    private void OnPlayerDamageTaken(float mag)
    {
        TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData()
        {
            element = PlayerEntity.ActiveInstance.MeleeWeapon.Stats.Element,
            sizeMult = RADIUS,
            enemyIgnored = -1,
            magnitude = PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.MaxHealth) * HP_TO_DAMAGE_RATIO * Group.Level

        };
        ObjectPoolManager.GetTechCombatEffectFromPool("EXPLOSION").SetUp(PlayerEntity.ActiveInstance.transform.position, 0, sd);
    }

    public override void OnTechUpgraded()
    {

    }
}
