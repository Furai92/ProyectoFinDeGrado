using UnityEngine;

public class AiStateBulletWave : AiStateBase
{
    private float stateEndTime;
    private PlayerEntity target;
    private bool shootPerformed;

    private const float CHARGE_DURATION = 1f;
    private const float SHOOT_ARC = 50f;
    private const int SHOOTS = 5;

    public AiStateBulletWave() 
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

            float shootDirOffset = -SHOOT_ARC/2;
            for (int i = 0; i < SHOOTS; i++) 
            {
                ObjectPoolManager.GetEnemyAttackFromPool("SLOW_ORB").SetUp(e, e.transform.position, GameTools.AngleBetween(e.transform.position, target.transform.position) + shootDirOffset);
                shootDirOffset += SHOOT_ARC / (SHOOTS - 1);
            }
            
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
