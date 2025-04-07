using UnityEngine;

public class AiStateIdle : AiStateBase
{
    private float stateEndTime;
    private PlayerEntity target;

    private const float STATE_DURATION = 0.5f;

    public AiStateIdle() 
    {
    }

    public override void EndState(EnemyEntity e)
    {

    }

    public override void FixedUpdateState(EnemyEntity e)
    {
        e.TargetMovementPosition = e.transform.position;
        e.TargetLookPosition = target.transform.position;
    }

    public override bool IsFinished(EnemyEntity e)
    {
        return Time.time > stateEndTime;
    }

    public override void StartState(EnemyEntity e)
    {
        stateEndTime = Time.time + STATE_DURATION;
        target = StageManagerBase.GetClosestPlayer(e.transform.position);
    }
}
