using UnityEngine;

public class StageStateVictory : StageStateBase
{
    private float stateEndTime;
    private float STATE_DURATION = 5f;

    public override GameState GetGameStateType()
    {
        return GameState.Victory;
    }

    public override float GetTimerDisplay()
    {
        return -1;
    }

    public override bool IsFinished()
    {
        return Time.time > stateEndTime;
    }

    public override void StateEnd()
    {
        
    }

    public override void StateStart()
    {
        stateEndTime = Time.time + STATE_DURATION;
    }

    public override void UpdateState()
    {

    }
}
