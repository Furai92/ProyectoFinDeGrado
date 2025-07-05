using UnityEngine;

public class AiStateWalkStraightToTargetLong : AiStateBase
{
    private float stateEndTime;
    private PlayerEntity target;

    private const float STATE_DURATION = 1.5f;

    public AiStateWalkStraightToTargetLong() 
    {

    }

    public override void EndState(EnemyEntity e)
    {

    }

    public override void FixedUpdateState(EnemyEntity e)
    {
        e.TargetMovementPosition = target.transform.position;
        e.TargetLookPosition = target.transform.position;
    }

    public override bool IsFinished(EnemyEntity e)
    {
        return Time.time > stateEndTime;
    }

    public override void StartState(EnemyEntity e)
    {
        stateEndTime = Time.time + STATE_DURATION;
        target = PlayerEntity.ActiveInstance;
    }
}
