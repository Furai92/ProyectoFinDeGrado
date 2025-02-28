using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEnemyAI", menuName = "ScriptableObjects/EnemyAi", order = 1)]
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

    [System.Serializable]
    public class EnemyAiInspectorTransition
    {

        [field: SerializeField] public string From { get; private set; }
        [field: SerializeField] public string To { get; private set; }
        [field: SerializeField] public int Priority { get; private set; }
        [field: SerializeField] public AiStateBase.TransitionConditional Condition { get; private set; }
        [field: SerializeField] public AiStateBase.TransitionComparator Comparator { get; private set; }
        [field: SerializeField] public float ConditionalValue { get; private set; }
        [field: SerializeField] public string ConditionalBool { get; private set; }
    }
}

