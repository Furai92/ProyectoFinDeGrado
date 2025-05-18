using UnityEngine;

public class TechBehaviourKineticImpact : TechBase
{
    private const float SHIELD_PERCENT_TO_DAMAGE = 0.5f;
    private const float SIZE = 3f;

    public override void OnTechAdded()
    {
        EventManager.PlayerDashStartedEvent += OnPlayerDashStarted;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerDashStartedEvent -= OnPlayerDashStarted;
    }

    private void OnPlayerDashStarted(Vector3 pos, float dir)
    {
        TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData()
        {
            sizeMult = SIZE,
            element = GameEnums.DamageElement.NonElemental,
            enemyIgnored = -1,
            magnitude = SHIELD_PERCENT_TO_DAMAGE * PlayerEntity.ActiveInstance.CurrentShield * Group.Level
        };
        ObjectPoolManager.GetTechCombatEffectFromPool("DASH_DAMAGE").SetUp(pos, dir, sd);
    }

    public override void OnTechUpgraded()
    {

    }
}
