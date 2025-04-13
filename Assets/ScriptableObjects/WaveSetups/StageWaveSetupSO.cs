using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageWaveSetup", menuName = "Scriptable Objects/StageWaveSetupSO")]
public class StageWaveSetupSO : ScriptableObject
{
    [field: SerializeField] public List<EnemyWave> Waves { get; private set; }

    [System.Serializable]
    public class EnemyWave
    {
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float ChestSpawnRate { get; private set; }
        [field: SerializeField] public List<EnemySpawnData> EnemySpawns { get; private set; }
    }
    [System.Serializable]
    public class EnemySpawnData
    {
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public float Cooldown { get; private set; }
        [field: SerializeField] public float Delay { get; private set; }
        [field: SerializeField] public int MaxSpawns { get; private set; }
    }
}
