using UnityEngine;

public abstract class AiStateBase
{
    protected EnemyEntity EnemyControlled { get; set; }

    public enum TransitionConditional { None, DistanceToPlayer, CurrentHealth }
    public enum TransitionComparator { LessThan, LessOrEqual, Equal, GreaterOrEqual, Greater }

    public abstract void StartState();
    public abstract void FixedUpdateState();
    public abstract void EndState();
    public abstract bool IsFinished();


    public void AddTransition(EnemyAiSO.EnemyAiInspectorTransition t) 
    {

    }
    public AiStateBase GetNextState() 
    {
        return this;
    }



}
