using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameDatabase", menuName = "Core/GameDatabase")]
public class GameDatabaseSO : ScriptableObject
{
    [field: SerializeField] public List<InspectorPair<GameObject>> PlayerAttackPrefabs { get; private set; }
    [field: SerializeField] public List<InspectorPair<GameObject>> EnemyPrefabs { get; private set; }
    [field: SerializeField] public List<InspectorPair<GameObject>> EnemyAttackPrefabs { get; private set; }
    [field: SerializeField] public List<WeaponSO> Weapons { get; private set; }

    [Header("Ungrouped prefabs")]
    [field: SerializeField] public GameObject DeflectedAttackPrefab { get; private set; }
    [field: SerializeField] public GameObject AutoPickupPrefab { get; private set; }
    [field: SerializeField] public GameObject WeaponPickupPrefab { get; private set; }
    [field: SerializeField] public GameObject ChestPrefab { get; private set; }
}



