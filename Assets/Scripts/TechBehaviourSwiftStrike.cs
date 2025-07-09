using UnityEngine;

public class TechBehaviourSwiftStrike : TechBase
{
    private const float DAMAGE = 400f;
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
        if (PlayerEntity.ActiveInstance.CurrentShield <= 0) { return; }

        TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData()
        {
            sizeMult = SIZE,
            element = PlayerEntity.ActiveInstance.MeleeWeapon.Stats.Element,
            enemyIgnored = -1,
            magnitude = DAMAGE * Group.Level
        };
        ObjectPoolManager.GetTechCombatEffectFromPool("DASH_DAMAGE").SetUp(pos, dir, sd);
    }

    public override void OnTechUpgraded()
    {

    }
}
