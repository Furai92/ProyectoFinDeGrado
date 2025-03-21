using UnityEngine;

public class StageStateRest : StageStateBase
{
    private bool allPlayersReady;

    public override StateType GetStateType()
    {
        return StateType.Rest;
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
