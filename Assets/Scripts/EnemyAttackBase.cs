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
        EventManager.StageStateStartedEvent += OnStageStateStarted;
    }
    private void OnDisable()
    {
        EventManager.EnemyStatusEffectAppliedEvent -= OnEnemyStatusApplied;
        EventManager.EnemyDefeatedEvent -= OnEnemyDefeated;
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
    }
    private void OnEnemyDefeated(EnemyEntity e) 
    {
        if (e == User) { OnUserDefeated(); }
    }
    private void OnEnemyStatusApplied(GameEnums.DamageElement elem, EnemyEntity e) 
    {
        if (e == User && elem == GameEnums.DamageElement.Frost) { OnUserStunned(); }
    }
    private void OnStageStateStarted(StageStateBase s) 
    {
        if (s.GetStateType() == StageStateBase.StateType.Rest) { OnWaveEnded(); }
    }

    public abstract void Initialize(Vector3 pos, float dir);
    public abstract void OnUserStunned();
    public abstract void OnUserDefeated();
    public abstract void OnWaveEnded();
}
