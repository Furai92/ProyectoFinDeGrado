using UnityEngine;

public class AiStateBeamAttack : AiStateBase
{
    private float stateEndTime;

    private const float WAIT_AFTER_ATTACK = 3f;

    public AiStateBeamAttack()
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
        PlayerEntity target = StageManagerBase.GetClosestPlayer(e.transform.position);
        e.TargetLookPosition = target.transform.position;
        stateEndTime = Time.time + WAIT_AFTER_ATTACK;

        EventManager.OnCombatWarningDisplayed(HudCombatWarningElement.WarningType.Normal, e.transform.position);
        ObjectPoolManager.GetEnemyAttackFromPool("BEAM").SetUp(e, e.transform.position, GameTools.AngleBetween(e.transform.position, target.transform.position));
    }
}
