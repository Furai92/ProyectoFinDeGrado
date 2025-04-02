using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameDatabase", menuName = "Core/GameDatabase")]
public class GameDatabaseSO : ScriptableObject
{
    [field: SerializeField] public List<SerializedIdentifiedPair<GameObject>> PlayerAttackPrefabs { get; private set; }
    [field: SerializeField] public List<SerializedIdentifiedPair<GameObject>> EnemyPrefabs { get; private set; }
    [field: SerializeField] public List<SerializedIdentifiedPair<GameObject>> EnemyAttackPrefabs { get; private set; }
    [field: SerializeField] public List<WeaponSO> Weapons { get; private set; }

    [Header("Ungrouped prefabs")]
    [field: SerializeField] public GameObject DeflectedAttackPrefab { get; private set; }
    [field: SerializeField] public GameObject CurrencyPickupPrefab { get; private set; }
    [field: SerializeField] public GameObject HealthPickupPrefab { get; private set; }
    [field: SerializeField] public GameObject WeaponPickupPrefab { get; private set; }
    [field: SerializeField] public GameObject ChestPrefab { get; private set; }
}



