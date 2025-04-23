using UnityEngine;
using System.Collections.Generic;

public abstract class AiStateBase
{
    private List<AiStateTransition> transitions;

    public enum TransitionConditional { None, DistanceToPlayer, CurrentHealth }
    public enum TransitionComparator { LessThan, LessOrEqual, Equal, GreaterOrEqual, Greater }

    public abstract void StartState(EnemyEntity e);
    public abstract void FixedUpdateState(EnemyEntity e);
    public abstract void EndState(EnemyEntity e);
    public abstract bool IsFinished(EnemyEntity e);


    public void AddTransition(EnemyAiSO.EnemyAiInspectorTransition t, AiStateBase to)
    {
        if (transitions == null) { transitions = new List<AiStateTransition>(); }

        AiStateTransition newt = new AiStateTransition(t, to);
        transitions.Add(newt);
    }
    public AiStateBase GetNextState(EnemyEntity enemyControlled)
    {
        AiStateTransition bestTransition = null;
        float highestPriority = -Mathf.NegativeInfinity;
        for (int i = 0; i < transitions.Count; i++) 
        {
            if (transitions[i].Priority > highestPriority) { continue; }
            if (!transitions[i].Evaluate(enemyControlled)) { continue; }

            bestTransition = transitions[i];
            highestPriority = transitions[i].Priority;
        }

        if (bestTransition == null) { return this; }
        return bestTransition.NextState;
    }

    private class AiStateTransition 
    {
        public AiStateBase NextState { get; private set; }
        public float Priority { get; private set; }

        private TransitionConditional conditional;
        private TransitionComparator comparator;
        private float floatParam;
        private bool boolParam;

        public AiStateTransition(EnemyAiSO.EnemyAiInspectorTransition t, AiStateBase to) 
        {
            conditional = t.Condition;
            comparator = t.Comparator;
            floatParam = t.ConditionalValue;
            boolParam = t.ConditionalBool;
            NextState = to;
            Priority = t.Priority;
        }
        public bool Evaluate(EnemyEntity enemyControlled) 
        {
            float value;
            switch (conditional) 
            {
                case TransitionConditional.CurrentHealth: { value = enemyControlled.GetHealthPercent(); break; }
                case TransitionConditional.DistanceToPlayer: { value = Vector3.Distance(enemyControlled.transform.position, PlayerEntity.ActiveInstance.transform.position); break; }
                default: { return true; }
            }

            switch (comparator) 
            {
                case TransitionComparator.Equal: { return value == floatParam; }
                case TransitionComparator.Greater: { return value > floatParam; }
                case TransitionComparator.GreaterOrEqual: { return value >= floatParam; }
                case TransitionComparator.LessOrEqual: { return value <= floatParam; }
                case TransitionComparator.LessThan: { return value < floatParam; }
            }
            return false;
        }
    }

}
