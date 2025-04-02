using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TechSO", menuName = "Scriptable Objects/TechSO")]
public class TechSO : ScriptableObject
{
    [field: SerializeField] public string ID { get; private set; }
    [field: SerializeField] public string Script { get; private set; }
    [field: SerializeField] public List<SerializedPair<StatGroup.Stat, float>> BonusStats { get; private set; }
    [field: SerializeField] public List<StatGroup.PlayerFlags> BonusFlags { get; private set; }
}
