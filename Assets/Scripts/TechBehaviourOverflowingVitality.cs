using UnityEngine;

public class TechBehaviourOverflowingVitality : TechBase
{
    private const float OVERHEALING_TO_DAMAGE_RATE = 50f;
    private const float RADIUS = 10f;

    public override void OnTechAdded()
    {
        EventManager.PlayerOverhealedEvent += OnPlayerOverhealed;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerOverhealedEvent -= OnPlayerOverhealed;
    }

    private void OnPlayerOverhealed(float h)
    {
        TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData()
        {
            sizeMult = RADIUS,
            element = GameEnums.DamageElement.NonElemental,
            enemyIgnored = -1,
            magnitude = OVERHEALING_TO_DAMAGE_RATE * Group.Level * h
        };
        ObjectPoolManager.GetTechCombatEffectFromPool("EXPLOSION").SetUp(PlayerEntity.ActiveInstance.transform.position, 0, sd);
    }

    public override void OnTechUpgraded()
    {

    }
}
