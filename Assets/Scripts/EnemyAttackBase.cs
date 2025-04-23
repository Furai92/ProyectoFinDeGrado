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
        EventManager.EnemyDefeatedEvent += OnEnemyDefeated;
        EventManager.StageStateEndedEvent += OnStageStateEnded;
    }
    private void OnDisable()
    {
        EventManager.EnemyStatusEffectAppliedEvent -= OnEnemyStatusApplied;
        EventManager.EnemyDefeatedEvent -= OnEnemyDefeated;
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
    }
    private void OnEnemyDefeated(EnemyEntity e) 
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

    public abstract void Initialize(Vector3 pos, float dir);
    public abstract void OnUserStunned();
    public abstract void OnUserDefeated();
    public abstract void OnStageStateEnded();
}
