using UnityEngine;

public class AiStateBulletSpray : AiStateBase
{
    private float shootTime;
    private int shootsPerformed;

    private const float INITIAL_SHOOT_DELAY = 1f;
    private const float CONSECUTIVE_SHOOT_DELAY = 0.15f;
    private const float RANDOM_SPREAD = 10f;
    private const int SHOOTS = 30;

    public AiStateBulletSpray() 
    {

    }

    public override void EndState(EnemyEntity e)
    {

    }

    public override void FixedUpdateState(EnemyEntity e)
    {
        e.TargetMovementPosition = e.transform.position;
        e.TargetLookPosition = PlayerEntity.ActiveInstance.transform.position;

        if (Time.time > shootTime) 
        {
            shootTime = Time.time + CONSECUTIVE_SHOOT_DELAY;
            shootsPerformed++;
            float angleToPlayer = GameTools.AngleBetween(e.transform.position, PlayerEntity.ActiveInstance.transform.position);
            float randomSpread = Random.Range(-RANDOM_SPREAD, RANDOM_SPREAD);
            ObjectPoolManager.GetEnemyAttackFromPool("SLOW_ORB").SetUp(e, e.transform.position, angleToPlayer + randomSpread);
        }
    }

    public override bool IsFinished(EnemyEntity e)
    {
        return shootsPerformed >= SHOOTS;
    }

    public override void StartState(EnemyEntity e)
    {
        shootTime = Time.time + INITIAL_SHOOT_DELAY;
        shootsPerformed = 0;

        e.TargetMovementPosition = e.transform.position;
        e.TargetLookPosition = PlayerEntity.ActiveInstance.transform.position;

        EventManager.OnCombatWarningDisplayed(HudCombatWarningElement.WarningType.Normal, e.transform.position);
    }
}
