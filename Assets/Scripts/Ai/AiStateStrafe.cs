using UnityEngine;

public class AiStateStrafe : AiStateBase
{
    private float stateEndTime;
    private PlayerEntity target;

    private const float STRAFE_DISTANCE = 5f;
    private const float STATE_DURATION = 1f;
    private const float ANGLE_OFFSET_MIN = 70f;
    private const float ANGLE_OFFSET_MAX = 110;

    public AiStateStrafe() 
    {

    }

    public override void EndState(EnemyEntity e)
    {

    }

    public override void FixedUpdateState(EnemyEntity e)
    {
        e.TargetLookPosition = target.transform.position;
        if (e.transform.position == e.TargetMovementPosition) { stateEndTime = -1; }
    }

    public override bool IsFinished(EnemyEntity e)
    {
        return Time.time > stateEndTime;
    }

    public override void StartState(EnemyEntity e)
    {
        stateEndTime = Time.time + STATE_DURATION;
        target = StageManagerBase.GetClosestPlayer(e.transform.position);
        float angleToTarget = GameTools.AngleBetween(e.transform.position, target.transform.position);
        float angleOffset = Random.Range(ANGLE_OFFSET_MIN, ANGLE_OFFSET_MAX);
        if (Random.Range(0, 2) == 1) { angleOffset *= -1; }

        e.TargetMovementPosition = e.transform.position + GameTools.AngleToVector(angleToTarget + angleOffset) * STRAFE_DISTANCE;
    }
}
