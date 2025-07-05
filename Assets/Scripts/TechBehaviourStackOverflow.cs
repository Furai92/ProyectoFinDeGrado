using UnityEngine;

public class TechBehaviourStackOverflow : TechBase
{
    private const float SHIELD_PERCENT_CONSUMED = 0.3f;
    private const float SHIELD_TO_DAMAGE_RATE = 5f;
    private const float RADIUS = 15f;

    public override void OnTechAdded()
    {
        EventManager.PlayerShieldCappedEvent += OnPlayerShieldCapped;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerShieldCappedEvent -= OnPlayerShieldCapped;
    }

    private void OnPlayerShieldCapped()
    {
        float explosionMag = PlayerEntity.ActiveInstance.CurrentShield * SHIELD_PERCENT_CONSUMED * Group.Level * SHIELD_TO_DAMAGE_RATE;
        TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData()
        {
            sizeMult = RADIUS,
            element = GameEnums.DamageElement.NonElemental,
            enemyIgnored = -1,
            magnitude = explosionMag
        };
        ObjectPoolManager.GetTechCombatEffectFromPool("EXPLOSION").SetUp(PlayerEntity.ActiveInstance.transform.position, 0, sd);
        PlayerEntity.ActiveInstance.RemoveShield(PlayerEntity.ActiveInstance.CurrentShield * SHIELD_PERCENT_CONSUMED * Group.Level);

    }

    public override void OnTechUpgraded()
    {

    }
}
