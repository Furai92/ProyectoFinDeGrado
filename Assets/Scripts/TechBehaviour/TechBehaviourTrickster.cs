using UnityEngine;

public class TechBehaviourTrickster : TechBase
{
    private bool effectActive;

    private const float RADIUS = 15f;
    private const float DEX_TO_DAMAGE = 10f;
    private const float BASE_DMG = 300f;

    public override void OnTechAdded()
    {
        EventManager.StageStateStartedEvent += OnStageStateStarted;
        EventManager.PlayerDashEndedEvent += OnPlayerDashEnded;
        effectActive = StageManagerBase.GetCurrentStateType() != StageStateBase.GameState.Rest;
    }

    public override void OnTechRemoved()
    {
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
        EventManager.PlayerDashEndedEvent -= OnPlayerDashEnded;
    }
    private void OnPlayerDashEnded(Vector3 pos, float dir) 
    {
        if (!effectActive) { return; }

        TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData() {
            sizeMult = RADIUS,
            element = PlayerEntity.ActiveInstance.RangedWeapon.Stats.Element,
            enemyIgnored = -1,
            magnitude = (DEX_TO_DAMAGE * Group.Level * PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.Dexterity)) + (Group.Level * BASE_DMG)
        };
        ObjectPoolManager.GetTechCombatEffectFromPool("LANDMINE").SetUp(pos, 0, sd);
    }
    private void OnStageStateStarted(StageStateBase.GameState s) 
    {
        effectActive = s != StageStateBase.GameState.Rest;
    }

    public override void OnTechUpgraded()
    {

    }
}
