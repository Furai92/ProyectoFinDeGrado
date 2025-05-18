using UnityEngine;

public class TechBehaviourEnergyExtraction : TechBase
{
    private const float CHANCE = 15f;

    public override void OnTechAdded()
    {
        EventManager.EnemyStatusEffectAppliedEvent += OnEnemyStatusEffectApplied;
    }

    public override void OnTechRemoved()
    {
        EventManager.EnemyStatusEffectAppliedEvent -= OnEnemyStatusEffectApplied;
    }

    private void OnEnemyStatusEffectApplied(GameEnums.DamageElement elem, EnemyEntity e)
    {
        if (Random.Range(0, 101) < CHANCE * Group.Level) 
        {
            ObjectPoolManager.GetHealthPickupFromPool().SetUp(e.transform.position);
        }
    }

    public override void OnTechUpgraded()
    {

    }
}
