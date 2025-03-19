using UnityEngine;

public class AiStateWalkStraightToTarget : AiStateBase
{
    private float stateEndTime;
    private PlayerEntity target;

    private const float STATE_DURATION = 1f;

    public AiStateWalkStraightToTarget(EnemyEntity e) 
    {
        EnemyControlled = e;
    }

    public override void EndState()
    {

    }

    public override void FixedUpdateState()
    {
        EnemyControlled.TargetMovementPosition = target.transform.position;
        EnemyControlled.TargetLookPosition = target.transform.position;
    }

    public override bool IsFinished()
    {
        return Time.time > stateEndTime;
    }

    public override void StartState()
    {
        stateEndTime = Time.time + STATE_DURATION;
        target = StageManagerBase.GetClosestPlayer(EnemyControlled.transform.position);
    }
}
