using UnityEngine;

public class TechBehaviourEndlessStorm : TechBase
{
    private float readyTime;

    private const float SEARCH_RADIUS = 20f;
    private const float CHANCE = 10f;
    private const float COOLDOWN = 0.5f;

    public override void OnTechAdded()
    {
        EventManager.EnemyStatusEffectAppliedEvent += OnEnemyStatusEffectApplied;
        readyTime = Time.time;
    }

    public override void OnTechRemoved()
    {
        EventManager.EnemyStatusEffectAppliedEvent -= OnEnemyStatusEffectApplied;
    }

    private void OnEnemyStatusEffectApplied(GameEnums.DamageElement elem, EnemyEntity e)
    {
        if (Time.time < readyTime) { return; }
        readyTime = Time.time + COOLDOWN;

        if (Random.Range(0, 101) < CHANCE * Group.Level)
        {
            TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData()
            {
                element = GameEnums.DamageElement.Thunder,
                enemyIgnored = e.EnemyInstanceID,
                magnitude = 0,
                sizeMult = SEARCH_RADIUS
            };
            ObjectPoolManager.GetTechCombatEffectFromPool("STATUS_TRANSFER").SetUp(e.transform.position, 0, sd);
        }
    }

    public override void OnTechUpgraded()
    {

    }
}
