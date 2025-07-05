using UnityEngine;

public class AiStateMeleeSpin : AiStateBase
{
    private float stateEndTime;
    private PlayerEntity target;

    private const float ATTACK_DURATION = 2.75f;

    public AiStateMeleeSpin() 
    {

    }

    public override void EndState(EnemyEntity e)
    {

    }

    public override void FixedUpdateState(EnemyEntity e)
    {

    }

    public override bool IsFinished(EnemyEntity e)
    {
        return Time.time > stateEndTime;
    }

    public override void StartState(EnemyEntity e)
    {
        stateEndTime = Time.time + ATTACK_DURATION;
        target = PlayerEntity.ActiveInstance;
        e.TargetLookPosition = target.transform.position;
        e.TargetMovementPosition = e.transform.position;
        ObjectPoolManager.GetEnemyAttackFromPool("MELEE_SPIN").SetUp(e, e.transform.position, 0);

        EventManager.OnCombatWarningDisplayed(HudCombatWarningElement.WarningType.Normal, e.transform.position);
    }
}
