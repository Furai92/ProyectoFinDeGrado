using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameDatabase", menuName = "Core/GameDatabase")]
public class GameDatabase : ScriptableObject
{
    [field: SerializeField] public List<InspectorPair<GameObject>> PlayerAttackPrefabs { get; private set; }
    [field: SerializeField] public List<InspectorPair<GameObject>> EnemyPrefabs { get; private set; }
    [field: SerializeField] public List<InspectorPair<GameObject>> EnemyAttackPrefabs { get; private set; }
    [Header("Ungrouped prefabs")]
    [field: SerializeField] public GameObject PickupPrefab { get; private set; }
}

[System.Serializable]
public class InspectorPair<T>
{
    [field: SerializeField] public string ID { get; private set; }
    [field: SerializeField] public T Data { get; private set; }
}
