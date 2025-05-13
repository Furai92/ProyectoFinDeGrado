using UnityEngine;

public class TechBehaviourSingularity : TechBase
{
    private float readyTime;

    private const float INT_TO_DAMAGE = 10f;
    private const float RADIUS = 12f;
    private const float BASE_COOLDOWN = 30f;
    private const float CDR_PER_LEVEL = 10f;

    public override void OnTechAdded()
    {
        EventManager.EnemyStatusEffectAppliedEvent += OnEnemyStatusEffectApplied;
        readyTime = 0;
    }

    public override void OnTechRemoved()
    {
        EventManager.EnemyStatusEffectAppliedEvent -= OnEnemyStatusEffectApplied;
    }

    private void OnEnemyStatusEffectApplied(GameEnums.DamageElement elem, EnemyEntity e)
    {
        if (elem != GameEnums.DamageElement.Void) { return; }
        if (Time.time < readyTime) { return; }
        if (e.GetStatusDuration(GameEnums.DamageElement.Void) <= EnemyEntity.STATUS_DURATION_STANDARD) { return; }

        readyTime = Time.time + BASE_COOLDOWN - ((Group.Level - 1) * CDR_PER_LEVEL);
        TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData() {
            sizeMult = RADIUS,
            element = GameEnums.DamageElement.Void,
            enemyIgnored = -1,
            magnitude = PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.Intellect) * INT_TO_DAMAGE
        };
        ObjectPoolManager.GetTechCombatEffectFromPool("SINGULARITY").SetUp(e.transform.position, 0, sd);
    }

    public override void OnTechUpgraded()
    {

    }
}
