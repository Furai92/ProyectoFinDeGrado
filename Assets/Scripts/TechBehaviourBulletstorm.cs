using UnityEngine;

public class TechBehaviourBulletstorm : TechBase
{
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
        TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData();
        ObjectPoolManager.GetTechCombatEffectFromPool("BULLETSTORM").SetUp(pos, dir, sd);
    }

    public override void OnTechUpgraded()
    {

    }
}
