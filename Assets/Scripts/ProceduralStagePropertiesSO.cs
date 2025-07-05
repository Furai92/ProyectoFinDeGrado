using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ProceduralStagePropertiesSO", menuName = "Scriptable Objects/ProceduralStagePropertiesSO")]
public class ProceduralStagePropertiesSO : ScriptableObject
{
    [field: SerializeField] public List<GameObject> RoomPrefabs;
    [field: SerializeField] public List<GameObject> CorridorPrefabs;
    [field: SerializeField] public List<GameObject> DecoPrefabs;
    [field: SerializeField] public GameObject GroundPlanePrefab;
}
