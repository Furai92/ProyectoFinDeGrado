using UnityEngine;

public class StageStateShortDelay : StageStateBase
{
    private float endTime;

    private const float DURATION = 4f;

    public override GameState GetGameStateType()
    {
        return GameState.Delay;
    }

    public override float GetTimerDisplay()
    {
        return -1;
    }

    public override bool IsFinished()
    {
        return Time.time > endTime;
    }

    public override void StateEnd()
    {
    }

    public override void StateStart()
    {
        endTime = Time.time + DURATION;
    }

    public override void UpdateState()
    {

    }
}
