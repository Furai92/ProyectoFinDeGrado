using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEnemyAI", menuName = "Scriptable Objects/EnemyAi", order = 1)]
public class EnemyAiSO : ScriptableObject
{
    [field: SerializeField] public string Entry { get; private set; }
    [field: SerializeField] public List<EnemyAiInspectorNode> Nodes { get; private set; }
    [field: SerializeField] public List<EnemyAiInspectorTransition> Transitions { get; private set; }

    [System.Serializable]
    public class EnemyAiInspectorNode
    {
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public string Script { get; private set; }
    }
    public AiStateBase GenerateAiTree() 
    {
        Dictionary<string, AiStateBase> states = new Dictionary<string, AiStateBase>();
        for (int i = 0; i < Nodes.Count; i++) 
        {
            AiStateBase newState = System.Activator.CreateInstance(System.Type.GetType(Nodes[i].Script)) as AiStateBase;
            states.Add(Nodes[i].ID, newState);
        }

        for (int i = 0; i < Transitions.Count; i++) 
        {
            if (!states.ContainsKey(Transitions[i].From)) { continue; }
            if (!states.ContainsKey(Transitions[i].To)) { continue; }

            states[Transitions[i].From].AddTransition(Transitions[i], states[Transitions[i].To]);
        }
        return states[Entry];
    }
    [System.Serializable]
    public class EnemyAiInspectorTransition
    {

        [field: SerializeField] public string From { get; private set; }
        [field: SerializeField] public string To { get; private set; }
        [field: SerializeField] public int Priority { get; private set; }
        [field: SerializeField] public AiStateBase.TransitionConditional Condition { get; private set; }
        [field: SerializeField] public AiStateBase.TransitionComparator Comparator { get; private set; }
        [field: SerializeField] public float ConditionalValue { get; private set; }
        [field: SerializeField] public bool ConditionalBool { get; private set; }
    }
}

