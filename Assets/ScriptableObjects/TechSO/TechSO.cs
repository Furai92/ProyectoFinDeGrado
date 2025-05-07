using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TechSO", menuName = "Scriptable Objects/TechSO")]
public class TechSO : ScriptableObject
{
    [field: SerializeField] public string ID { get; private set; }
    [field: SerializeField] public string NameID { get; private set; }
    [field: SerializeField] public string DescID { get; private set; }
    [field: SerializeField] public int MaxLevel { get; private set; }
    [field: SerializeField] public GameEnums.Rarity Rarity { get; private set; }
    [field: SerializeField] public PlayerTraitSO RelatedTrait { get; private set; }

    [field: SerializeField] public string Script { get; private set; }
    [field: SerializeField] public List<SerializedPair<PlayerStatGroup.Stat, float>> BonusStats { get; private set; }
    [field: SerializeField] public List<SerializedPair<StageStatGroup.StageStat, float>> BonusStageStats { get; private set; }
    [field: SerializeField] public List<PlayerStatGroup.PlayerFlags> BonusFlags { get; private set; }

    [field: SerializeField] public List<TechSODescDetail> DescriptionDetails { get; private set; }

    [System.Serializable]
    public class TechSODescDetail 
    {
        public enum DetailType { Neutral, Positive, Negative }

        [field: SerializeField] public string DescTextID { get; private set; }
        [field: SerializeField] public float DescValueBase { get; private set; }
        [field: SerializeField] public float DescValueScaling { get; private set; }
        [field: SerializeField] public DetailType DescDetailType { get; private set; }
    }
}
