using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameDatabase", menuName = "Core/GameDatabase")]
public class GameDatabase : ScriptableObject
{
    [field: SerializeField] public List<InspectorPair<PlayerAttackBase>> PlayerAttackPrefabs { get; private set; }
    [field: SerializeField] public List<InspectorPair<EnemyEntity>> EnemyPrefabs { get; private set; }
}

[System.Serializable]
public class InspectorPair<T>
{
    [field: SerializeField] public string ID { get; private set; }
    [field: SerializeField] public T Data { get; private set; }
}
