using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TechSO", menuName = "Scriptable Objects/TechSO")]
public class TechSO : ScriptableObject
{
    [field: SerializeField] public string ID { get; private set; }
    [field: SerializeField] public string NameID { get; private set; }
    [field: SerializeField] public string DescID { get; private set; }
    [field: SerializeField] public int MaxLevel { get; private set; }
    [field: SerializeField] public GameEnums.TechRarity Rarity { get; private set; }
    [field: SerializeField] public List<GameEnums.Trait> TraitRequirement { get; private set; }

    [field: SerializeField] public string Script { get; private set; }
    [field: SerializeField] public List<SerializedPair<PlayerStatGroup.Stat, float>> BonusStats { get; private set; }
    [field: SerializeField] public List<PlayerStatGroup.PlayerFlags> BonusFlags { get; private set; }


}
