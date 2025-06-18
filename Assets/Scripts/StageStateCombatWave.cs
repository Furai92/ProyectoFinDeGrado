using UnityEngine;
using System.Collections.Generic;

public class StageStateCombatWave : StageStateBase
{
    private List<WaveSpawnTimerData> spawnTimers;

    private Dictionary<string, int> trackedSpawns;
    private StageWaveSetupSO.EnemyWave waveData;
    private float waveDurationMax;
    private float waveDurationRemaining;
    private float nextChestSpawn;
    private float nextSpawnCheck;
    private bool canSpawnChests;

    private const float ENEMY_HEALTH_SCALING = 0.25f;
    private const float ENEMY_DAMAGE_SCALING = 0.05f;
    private const float CHEST_SPAWN_COOLDOWN = 10f;
    private const float SPAWN_CHECK_INTERVAL = 0.5f;
    private const float WEAPON_LEVEL_FACTOR_GAIN_PER_COMBAT = 0.5f;
    private const int MAX_ENEMIES = 20;

    public StageStateCombatWave(StageWaveSetupSO.EnemyWave w) 
    {
        waveData = w;
        nextSpawnCheck = Time.time + SPAWN_CHECK_INTERVAL;
        nextChestSpawn = Time.time + CHEST_SPAWN_COOLDOWN / StageManagerBase.GetStageStat(StageStatGroup.StageStat.ChestSpawnRate);
    }

    public override bool IsFinished()
    {
        return waveDurationRemaining <= 0;
    }

    public override void StateEnd()
    {
        StageManagerBase.ChangeStageStat(StageStatGroup.StageStat.EnemyDamageMult, StageManagerBase.GetStageStat(StageStatGroup.StageStat.EnemyDamageMult) * ENEMY_DAMAGE_SCALING);
        StageManagerBase.ChangeStageStat(StageStatGroup.StageStat.EnemyHealthMult, StageManagerBase.GetStageStat(StageStatGroup.StageStat.EnemyHealthMult) * ENEMY_HEALTH_SCALING);
        StageManagerBase.ChangeStageStat(StageStatGroup.StageStat.DropLevelFactor, WEAPON_LEVEL_FACTOR_GAIN_PER_COMBAT);
        EventManager.EnemyDisabledEvent -= OnEnemyDisabled;
        EventManager.EnemySpawnedEvent -= OnEnemySpawned;
    }
    private void OnEnemySpawned(EnemyEntity e) 
    {
        TrackEnemy(e.ID, true);
    }
    private void OnEnemyDisabled(EnemyEntity e, float overkill, GameEnums.EnemyRank rank, bool killcredit) 
    {
        TrackEnemy(e.ID, false);
    }
    private void TrackEnemy(string id, bool addedToStage) 
    {
        if (!trackedSpawns.ContainsKey(id)) { trackedSpawns.Add(id, 0); }
        trackedSpawns[id] += addedToStage ? 1 : -1;
    }
    private int GetEnemyCount(string id) 
    {
        if (!trackedSpawns.ContainsKey(id)) { return 0; }

        return trackedSpawns[id];
    }

    public override void StateStart()
    {
        trackedSpawns = new Dictionary<string, int>();
        waveDurationMax = waveData.Duration;
        spawnTimers = new List<WaveSpawnTimerData>();
        for (int i = 0; i < waveData.EnemySpawns.Count; i++) 
        {
            WaveSpawnTimerData wstd = new WaveSpawnTimerData();
            wstd.spawnID = waveData.EnemySpawns[i].ID;
            wstd.nextSpawn = waveData.EnemySpawns[i].Delay;
            wstd.spawnCooldown = (waveData.EnemySpawns[i].Cooldown / StageManagerBase.GetStageStat(StageStatGroup.StageStat.EnemySpawnRate));
            wstd.spawnCount = 0;
            wstd.maxWaveSpawns = waveData.EnemySpawns[i].MaxSpawnsDuringWave;
            wstd.maxConcurrentSpawns = waveData.EnemySpawns[i].MaxConcurrentSpawns;
            spawnTimers.Add(wstd);
        }
        canSpawnChests = waveData.CanSpawnChests;
        waveDurationRemaining = waveDurationMax * StageManagerBase.GetStageStat(StageStatGroup.StageStat.WaveDurationMult);

        EventManager.EnemyDisabledEvent += OnEnemyDisabled;
        EventManager.EnemySpawnedEvent += OnEnemySpawned;
    }

    public override void UpdateState()
    {
        if (Time.time > nextSpawnCheck) 
        {
            CheckSpawns();
            nextSpawnCheck = Time.time + SPAWN_CHECK_INTERVAL;
        }

        // Count down timer
        waveDurationRemaining -= Time.deltaTime;
    }
    private void CheckSpawns() 
    {
        // Spawn enemies
        for (int i = spawnTimers.Count - 1; i >= 0; i--)
        {
            if (Time.time < spawnTimers[i].nextSpawn) { continue; }
            if (spawnTimers[i].maxConcurrentSpawns > 0 && GetEnemyCount(spawnTimers[i].spawnID) >= spawnTimers[i].maxConcurrentSpawns) { continue; }

            SpawnEnemy(spawnTimers[i].spawnID);
            spawnTimers[i].spawnCount++;
            spawnTimers[i].nextSpawn = Time.time + spawnTimers[i].spawnCooldown;
            if (spawnTimers[i].maxWaveSpawns > 0 && spawnTimers[i].spawnCount >= spawnTimers[i].maxWaveSpawns) { spawnTimers.RemoveAt(i); }
        }

        // Spawn chests
        if (Time.time >= nextChestSpawn && canSpawnChests)
        {
            nextChestSpawn = Time.time + CHEST_SPAWN_COOLDOWN / StageManagerBase.GetStageStat(StageStatGroup.StageStat.ChestSpawnRate);
            StageManagerBase.SpawnChest();
        }
    }
    public override float GetTimerDisplay()
    {
        return waveDurationRemaining;
    }
    private void SpawnEnemy(string id)
    {
        if (StageManagerBase.GetEnemyCount() >= MAX_ENEMIES) { return; }

        Vector3 randomPos = StageManagerBase.GetRandomEnemySpawnPosition(0);
        ObjectPoolManager.GetEnemyFromPool(id).SetUp(randomPos);
    }

    public override GameState GetGameStateType()
    {
        return GameState.EnemyWave;
    }

    private class WaveSpawnTimerData 
    {
        public string spawnID;
        public float spawnCooldown;
        public float nextSpawn;
        public int spawnCount;
        public int maxWaveSpawns;
        public int maxConcurrentSpawns;
    }
}
