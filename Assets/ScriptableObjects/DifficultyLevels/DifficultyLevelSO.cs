using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyLevel", menuName = "Scriptable Objects/DifficultyLevel")]
public class DifficultyLevelSO : ScriptableObject
{
    [field: SerializeField] public string NameID { get; private set; }
    [field: SerializeField] public string DescID { get; private set; }
    [field: SerializeField] public Color IconColor { get; private set; }
    [field: SerializeField] public float BaseEnemyHealthMult { get; private set; }
    [field: SerializeField] public float BaseEnemyDamageMult { get; private set; }
    [field: SerializeField] public float EnemyHealthScaling { get; private set; }
    [field: SerializeField] public float EnemyDamageScaling { get; private set; }
    [field: SerializeField] public float EnemySpawnRateMult { get; private set; }
    [field: SerializeField] public int MaxEnemyCount { get; private set; }
    [field: SerializeField] public float ShopPriceMult { get; private set; }
}
