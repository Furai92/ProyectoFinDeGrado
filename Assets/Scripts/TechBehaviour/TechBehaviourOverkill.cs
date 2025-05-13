using UnityEngine;

public class TechBehaviourOverkill : TechBase
{
    private const float OVERKILL_TO_DAMAGE = 0.2f;
    private const float RADIUS = 6f;

    public override void OnTechAdded()
    {
        EventManager.EnemyDisabledEvent += OnEnemyDisabled;
    }

    public override void OnTechRemoved()
    {
        EventManager.EnemyDisabledEvent -= OnEnemyDisabled;
    }

    private void OnEnemyDisabled(EnemyEntity e, float overkill, bool killcredit)
    {
        if (!killcredit) { return; }
        if (overkill <= 0) { return; }

        TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData()
        {
            element = GameEnums.DamageElement.Void,
            sizeMult = RADIUS,
            enemyIgnored = -1,
            magnitude = OVERKILL_TO_DAMAGE * Group.Level * overkill
        };
        ObjectPoolManager.GetTechCombatEffectFromPool("EXPLOSION").SetUp(e.transform.position, 0, sd);
    }

    public override void OnTechUpgraded()
    {

    }
}
