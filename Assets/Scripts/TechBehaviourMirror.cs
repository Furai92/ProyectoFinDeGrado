using UnityEngine;

public class TechBehaviourMirror : TechBase
{
    private const float RADIUS = 10f;
    private const float DAMAGE_REFLECT_PERCENT = 3f;

    public override void OnTechAdded()
    {
        EventManager.PlayerDamageAbsorbedEvent += OnDamageAbsorbed;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerDamageAbsorbedEvent -= OnDamageAbsorbed;
    }

    private void OnDamageAbsorbed(float d)
    {
        TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData()
        {
            sizeMult = RADIUS,
            element = GameEnums.DamageElement.NonElemental,
            enemyIgnored = -1,
            magnitude = d * DAMAGE_REFLECT_PERCENT * Group.Level
        };
        ObjectPoolManager.GetTechCombatEffectFromPool("EXPLOSION").SetUp(PlayerEntity.ActiveInstance.transform.position, 0, sd);
    }

    public override void OnTechUpgraded()
    {

    }
}
