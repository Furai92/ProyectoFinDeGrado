using UnityEngine;

public class AiStateChargeRangedAttack : AiStateBase
{
    private float stateEndTime;
    private PlayerEntity target;
    private bool shootPerformed;

    private const float CHARGE_DURATION = 1f;

    public AiStateChargeRangedAttack() 
    {

    }

    public override void EndState(EnemyEntity e)
    {

    }

    public override void FixedUpdateState(EnemyEntity e)
    {
        e.TargetMovementPosition = e.transform.position;
        e.TargetLookPosition = target.transform.position;
        if (Time.time > stateEndTime && !shootPerformed) 
        {
            EventManager.OnCombatWarningDisplayed(HudCombatWarningElement.WarningType.Normal, e.transform.position);
            shootPerformed = true;
            ObjectPoolManager.GetEnemyAttackFromPool("SLOW_ORB").SetUp(e, e.transform.position, GameTools.AngleBetween(e.transform.position, target.transform.position));
        }
    }

    public override bool IsFinished(EnemyEntity e)
    {
        return shootPerformed;
    }

    public override void StartState(EnemyEntity e)
    {
        stateEndTime = Time.time + CHARGE_DURATION;
        target = PlayerEntity.ActiveInstance;
        shootPerformed = false;
    }
}
