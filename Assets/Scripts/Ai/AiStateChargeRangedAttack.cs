using UnityEngine;

public class AiStateChargeRangedAttack : AiStateBase
{
    private float stateEndTime;
    private PlayerController target;
    private bool shootPerformed;

    private const float CHARGE_DURATION = 1f;

    public AiStateChargeRangedAttack(EnemyEntity e) 
    {
        EnemyControlled = e;
    }

    public override void EndState()
    {

    }

    public override void FixedUpdateState()
    {
        EnemyControlled.TargetMovementPosition = EnemyControlled.transform.position;
        EnemyControlled.TargetLookPosition = target.transform.position;
        if (Time.time > stateEndTime && !shootPerformed) 
        {
            shootPerformed = true;
            StageManagerBase.GetObjectPool().GetEnemyAttackFromPool("SLOW_ORB").SetUp(EnemyControlled.transform.position, GameTools.AngleBetween(EnemyControlled.transform.position, target.transform.position));
        }
    }

    public override bool IsFinished()
    {
        return shootPerformed;
    }

    public override void StartState()
    {
        stateEndTime = Time.time + CHARGE_DURATION;
        target = StageManagerBase.GetClosestPlayer(EnemyControlled.transform.position);
        shootPerformed = false;
    }
}
