using UnityEngine;

public class AiStateLaserArray : AiStateBase
{
    private float stateEndTime;


    private const float LASER_SPACING = 4f;
    private const float WAIT_AFTER_ATTACK = 4f;

    public AiStateLaserArray()
    {

    }

    public override void EndState(EnemyEntity e)
    {

    }

    public override void FixedUpdateState(EnemyEntity e)
    {
        e.TargetMovementPosition = e.transform.position;
    }

    public override bool IsFinished(EnemyEntity e)
    {
        return Time.time > stateEndTime;
    }

    public override void StartState(EnemyEntity e)
    {
        PlayerEntity target = PlayerEntity.ActiveInstance;
        e.TargetLookPosition = target.transform.position;
        stateEndTime = Time.time + WAIT_AFTER_ATTACK;
        float aimDirection = GameTools.AngleBetween(e.transform.position, target.transform.position);

        ObjectPoolManager.GetEnemyAttackFromPool("UNLINKED_BEAM").SetUp(e, e.transform.position, aimDirection);
        ObjectPoolManager.GetEnemyAttackFromPool("UNLINKED_BEAM").SetUp(e, e.transform.position + GameTools.OffsetFromAngle(-aimDirection, LASER_SPACING), aimDirection);
        ObjectPoolManager.GetEnemyAttackFromPool("UNLINKED_BEAM").SetUp(e, e.transform.position + GameTools.OffsetFromAngle(-aimDirection, -LASER_SPACING), aimDirection);
        ObjectPoolManager.GetEnemyAttackFromPool("UNLINKED_BEAM").SetUp(e, e.transform.position + GameTools.OffsetFromAngle(-aimDirection, LASER_SPACING * 2), aimDirection);
        ObjectPoolManager.GetEnemyAttackFromPool("UNLINKED_BEAM").SetUp(e, e.transform.position + GameTools.OffsetFromAngle(-aimDirection, -LASER_SPACING * 2), aimDirection);

        EventManager.OnCombatWarningDisplayed(HudCombatWarningElement.WarningType.Normal, e.transform.position);
        
    }
}
