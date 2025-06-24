using UnityEngine;

public class AiStateFireWarpStrike : AiStateBase
{
    private float stateEndTime;
    private PlayerEntity target;

    public AiStateFireWarpStrike() 
    {

    }

    public override void EndState(EnemyEntity e)
    {

    }

    public override void FixedUpdateState(EnemyEntity e)
    {
        e.TargetLookPosition = PlayerEntity.ActiveInstance.transform.position;
        e.TargetMovementPosition = e.transform.position;
    }

    public override bool IsFinished(EnemyEntity e)
    {
        return Time.time > stateEndTime;
    }

    public override void StartState(EnemyEntity e)
    {
        stateEndTime = Time.time + EnemyAttackFireWarpStrike.WARNING_DURATION + EnemyAttackFireWarpStrike.LINGER_DURATION;
        target = PlayerEntity.ActiveInstance;
        e.TargetLookPosition = target.transform.position;
        e.TargetMovementPosition = e.transform.position;
        ObjectPoolManager.GetEnemyAttackFromPool("FIRE_WARP").SetUp(e, e.transform.position, 0);

        EventManager.OnCombatWarningDisplayed(HudCombatWarningElement.WarningType.Normal, e.transform.position);
    }
}
