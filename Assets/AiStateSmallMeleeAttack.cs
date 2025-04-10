using UnityEngine;

public class AiStateSmallMeleeAttack : AiStateBase
{
    private float stateEndTime;
    private PlayerEntity target;
    private bool attackPerformed;

    private const float ATTACK_DURATION = 0.25f;

    public AiStateSmallMeleeAttack() 
    {

    }

    public override void EndState(EnemyEntity e)
    {

    }

    public override void FixedUpdateState(EnemyEntity e)
    {
        e.TargetMovementPosition = e.transform.position;
        e.TargetLookPosition = target.transform.position;
        if (Time.time > stateEndTime && !attackPerformed) 
        {
            EventManager.OnCombatWarningDisplayed(HudCombatWarningElement.WarningType.Normal, e.transform.position);
            attackPerformed = true;
            ObjectPoolManager.GetEnemyAttackFromPool("MELEE_SMALL").SetUp(e, e.transform.position, GameTools.AngleBetween(e.transform.position, target.transform.position));
        }
    }

    public override bool IsFinished(EnemyEntity e)
    {
        return attackPerformed;
    }

    public override void StartState(EnemyEntity e)
    {
        stateEndTime = Time.time + ATTACK_DURATION;
        target = StageManagerBase.GetClosestPlayer(e.transform.position);
        attackPerformed = false;
    }
}
