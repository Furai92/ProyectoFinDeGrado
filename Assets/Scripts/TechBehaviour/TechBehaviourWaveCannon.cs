using UnityEngine;

public class TechBehaviourWaveCannon : TechBase
{
    private const float SIZE_BASE = 1f;
    private const float SIZE_BONUS_PER_STACK = 0.2f;
    private const float DAMAGE_PER_STACK = 200f;
    private const string BUFF_ID = "WAVE_CANNON";

    public override void OnTechAdded()
    {
        EventManager.EnemyStatusEffectAppliedEvent += OnEnemyStatusEffectApplied;
        EventManager.StageStateEndedEvent += OnStageStateEnded;
        EventManager.PlayerAttackStartedEvent += OnPlayerAttackStarted;
    }

    public override void OnTechRemoved()
    {
        EventManager.EnemyStatusEffectAppliedEvent -= OnEnemyStatusEffectApplied;
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
        EventManager.PlayerAttackStartedEvent -= OnPlayerAttackStarted;
    }

    private void OnPlayerAttackStarted(Vector3 pos, WeaponSO.WeaponSlot s, float dir) 
    {
        if (s != WeaponSO.WeaponSlot.Melee) { return; }
        if (PlayerEntity.ActiveInstance.ActiveBuffDictionary.ContainsKey(BUFF_ID)) 
        {
            TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData()
            {
                element = GameEnums.DamageElement.NonElemental,
                sizeMult = SIZE_BASE + (PlayerEntity.ActiveInstance.ActiveBuffDictionary[BUFF_ID].Stacks * SIZE_BONUS_PER_STACK),
                enemyIgnored = -1,
                magnitude = PlayerEntity.ActiveInstance.ActiveBuffDictionary[BUFF_ID].Stacks * DAMAGE_PER_STACK
            };
            ObjectPoolManager.GetTechCombatEffectFromPool("WAVE_CANNON").SetUp(pos, dir, sd);
            PlayerEntity.ActiveInstance.RemoveBuff(BUFF_ID);
        }
    }
    private void OnEnemyStatusEffectApplied(GameEnums.DamageElement elem, EnemyEntity e)
    {
        if (elem != GameEnums.DamageElement.Void) { return; }

        PlayerEntity.ActiveInstance.ChangeBuff(BUFF_ID, 1, Group.Level);
    }
    private void OnStageStateEnded(StageStateBase.GameState s) 
    {
        PlayerEntity.ActiveInstance.RemoveBuff(BUFF_ID);
    }


    public override void OnTechUpgraded()
    {

    }
}
