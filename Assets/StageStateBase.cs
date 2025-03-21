using UnityEngine;

public abstract class StageStateBase
{
    public enum StateType { Combat, Rest }

    private StageStateBase nextState;

    public void SetNextState(StageStateBase ss) { nextState = ss; }
    public StageStateBase GetNextState() { return nextState == null ? this : nextState; }

    public abstract void UpdateState();
    public abstract void StateStart();
    public abstract void StateEnd();
    public abstract bool IsFinished();
    public abstract float GetTimerDisplay();
    public abstract StateType GetStateType();
}
