using UnityEngine;

public class AiStateSpinWave : AiStateBase
{
    private float aimDir;
    private int wavesCreated;
    private float nextStepTime;
    private bool finished;

    private const float SPIN_ADVANCE = 8f;
    private const int SPIN_ITERATIONS = 5;
    private const int SPIN_COUNT = 5;
    private const float WAIT_AFTER_END = 3f;
    private const float WAIT_AFTER_SPIN = 0.3f;

    public AiStateSpinWave()
    {

    }

    public override void EndState(EnemyEntity e)
    {

    }

    public override void FixedUpdateState(EnemyEntity e)
    {
        e.TargetMovementPosition = e.transform.position;

        if (Time.time > nextStepTime) 
        {
            if (wavesCreated < SPIN_COUNT) { CreateSpinWave(e); } else { finished = true; }
        }
    }

    public override bool IsFinished(EnemyEntity e)
    {
        return finished;
    }
    private void CreateSpinWave(EnemyEntity eRef) 
    {
        EventManager.OnCombatWarningDisplayed(HudCombatWarningElement.WarningType.Normal, eRef.transform.position);
        float patternAngle = 0;
        for (int i = 0; i < SPIN_COUNT; i++) 
        {
            ObjectPoolManager.GetEnemyAttackFromPool("UNLINKED_MELEE_SPIN").SetUp(eRef, eRef.transform.position + GameTools.OffsetFromAngle(aimDir + patternAngle, SPIN_ADVANCE * wavesCreated), 0);
            patternAngle += 360 / SPIN_ITERATIONS;
        }
        wavesCreated++;
        nextStepTime = wavesCreated >= SPIN_COUNT ? Time.time + WAIT_AFTER_END : Time.time + WAIT_AFTER_SPIN;
    }

    public override void StartState(EnemyEntity e)
    {
        PlayerEntity target = PlayerEntity.ActiveInstance;
        e.TargetLookPosition = target.transform.position;
        aimDir = Random.Range(0, 361);
        wavesCreated = 0;
        finished = false;

        CreateSpinWave(e);
    }
}
