using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DifficultyLevel", menuName = "Scriptable Objects/DifficultyLevel")]
public class DifficultyLevelSO : ScriptableObject
{
    [field: SerializeField] public int DifficultyIndex { get; private set; }
    [field: SerializeField] public string DescID { get; private set; }
    [field: SerializeField] public List<DifficultyEffectPair> Effects { get; private set; }

    [System.Serializable]
    public class DifficultyEffectPair 
    {
        [field: SerializeField] public StageStatGroup.StageStat Stat { get; private set; }
        [field: SerializeField] public float Value { get; private set; }
    }
}
