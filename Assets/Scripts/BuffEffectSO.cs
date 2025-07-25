using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BuffEffectSO", menuName = "Scriptable Objects/BuffEffectSO")]
public class BuffEffectSO : ScriptableObject
{
    [field: SerializeField] public string ID { get; private set; }
    [field: SerializeField] public int MaxStacks { get; private set; }
    [field: SerializeField] public float Duration { get; private set; }
    [field: SerializeField] public Color BuffColor { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public List<SerializedPair<PlayerStatGroup.Stat, float>> Effects { get; private set; }
    [field: SerializeField] public List<PlayerStatGroup.PlayerFlags> Flags { get; private set; }
}
