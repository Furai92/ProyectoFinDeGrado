using UnityEngine;

public abstract class EnemyAttackBase : MonoBehaviour
{
    protected EnemyEntity User;

    public void SetUp(EnemyEntity u, Vector3 pos, float direction) 
    {
        User = u;
        Initialize(pos, direction);
    }
    private void OnEnable()
    {
        EventManager.EnemyStatusEffectAppliedEvent += OnEnemyStatusApplied;
        EventManager.EnemyDisabledEvent += OnEnemyDisabled;
        EventManager.StageStateEndedEvent += OnStageStateEnded;
    }
    private void OnDisable()
    {
        EventManager.EnemyStatusEffectAppliedEvent -= OnEnemyStatusApplied;
        EventManager.EnemyDisabledEvent -= OnEnemyDisabled;
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
    }
    private void OnEnemyDisabled(EnemyEntity e, float overkill, GameEnums.EnemyRank rank, bool killcredit) 
    {
        if (e == User) { OnUserDefeated(); }
    }
    private void OnEnemyStatusApplied(GameEnums.DamageElement elem, EnemyEntity e) 
    {
        if (e == User && elem == GameEnums.DamageElement.Frost) { OnUserStunned(); }
    }
    private void OnStageStateEnded(StageStateBase.GameState s) 
    {
        OnStageStateEnded();
    }
    protected bool DamagePlayer(PlayerEntity p, float magnitude) 
    {
        if (p.IsEvading()) { EventManager.OnPlayerEvasion(p.transform.position); return false; }
        else { p.DealDamage(magnitude * StageManagerBase.GetStageStat(StageStatGroup.StageStat.EnemyDamageMult), true, false, false); return true; }
    }

    public abstract void Initialize(Vector3 pos, float dir);
    public abstract void OnUserStunned();
    public abstract void OnUserDefeated();
    public abstract void OnStageStateEnded();
}
