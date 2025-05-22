using UnityEngine;

public class AiStateBulletSpiral : AiStateBase
{
    private float shootTime;
    private int shootsPerformed;
    private float shootDirOffset;

    private const float INITIAL_SHOOT_DELAY = 1f;
    private const float CONSECUTIVE_SHOOT_DELAY = 0.25f;
    private const float SHOOT_ROTATION_SPEED = 20f;
    private const int MULTISHOOT = 3;
    private const int SHOOTS = 10;

    public AiStateBulletSpiral() 
    {

    }

    public override void EndState(EnemyEntity e)
    {

    }

    public override void FixedUpdateState(EnemyEntity e)
    {
        if (Time.time > shootTime) 
        {
            shootTime = Time.time + CONSECUTIVE_SHOOT_DELAY;
            shootsPerformed++;
            for (int i = 0; i < MULTISHOOT; i++) 
            {
                ObjectPoolManager.GetEnemyAttackFromPool("SLOW_ORB").SetUp(e, e.transform.position, shootDirOffset + i * (360/MULTISHOOT));
            }
            shootDirOffset += SHOOT_ROTATION_SPEED;
        }
    }

    public override bool IsFinished(EnemyEntity e)
    {
        return shootsPerformed >= SHOOTS;
    }

    public override void StartState(EnemyEntity e)
    {
        shootDirOffset = Random.Range(0, 361);
        shootTime = Time.time + INITIAL_SHOOT_DELAY;
        shootsPerformed = 0;

        e.TargetMovementPosition = e.transform.position;
        e.TargetLookPosition = PlayerEntity.ActiveInstance.transform.position;

        EventManager.OnCombatWarningDisplayed(HudCombatWarningElement.WarningType.Normal, e.transform.position);
    }
}
