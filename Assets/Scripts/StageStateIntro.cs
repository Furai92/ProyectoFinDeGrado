using UnityEngine;

public class StageStateIntro : StageStateBase
{
    private bool allPlayersReady;

    public override GameState GetGameStateType()
    {
        return GameState.Intro;
    }

    public override float GetTimerDisplay()
    {
        return -1;
    }

    public override bool IsFinished()
    {
        return allPlayersReady;
    }

    public override void StateEnd()
    {
        EventManager.AllPlayersReadyEvent -= OnAllPlayersReady;
    }

    public override void StateStart()
    {
        allPlayersReady = false;
        EventManager.AllPlayersReadyEvent += OnAllPlayersReady;
    }

    public override void UpdateState()
    {

    }

    private void OnAllPlayersReady()
    {
        allPlayersReady = true;
    }
}
