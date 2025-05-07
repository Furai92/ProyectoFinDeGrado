using UnityEngine;
using System.Collections.Generic;

public class StageStateCombatWave : StageStateBase
{
    private List<WaveSpawnTimerData> spawnTimers;

    private StageWaveSetupSO.EnemyWave waveData;
    private float waveDurationMax;
    private float waveDurationRemaining;
    private float nextChestSpawn;
    private float nextSpawnCheck;

    private const float CHEST_SPAWN_COOLDOWN = 10f;
    private const float SPAWN_CHECK_INTERVAL = 0.5f;
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
        StageManagerBase.DisableAllEnemies();
    }

    public override void StateStart()
    {
        waveDurationMax = waveData.Duration;
        spawnTimers = new List<WaveSpawnTimerData>();
        for (int i = 0; i < waveData.EnemySpawns.Count; i++) 
        {
            WaveSpawnTimerData wstd = new WaveSpawnTimerData();
            wstd.spawnID = waveData.EnemySpawns[i].ID;
            wstd.nextSpawn = waveData.EnemySpawns[i].Cooldown + waveData.EnemySpawns[i].Delay;
            wstd.spawnCooldown = waveData.EnemySpawns[i].Delay + (waveData.EnemySpawns[i].Cooldown / StageManagerBase.GetStageStat(StageStatGroup.StageStat.EnemySpawnRate));
            wstd.spawnCount = 0;
            wstd.maxSpawns = waveData.EnemySpawns[i].MaxSpawns;
            spawnTimers.Add(wstd);
        }
        waveDurationRemaining = waveDurationMax;
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
            if (Time.time > spawnTimers[i].nextSpawn)
            {
                SpawnEnemy(spawnTimers[i].spawnID);
                spawnTimers[i].spawnCount++;
                spawnTimers[i].nextSpawn = Time.time + spawnTimers[i].spawnCooldown;
                if (spawnTimers[i].maxSpawns > 0 && spawnTimers[i].spawnCount >= spawnTimers[i].maxSpawns) { spawnTimers.RemoveAt(i); }
            }
        }

        // Spawn chests
        if (Time.time >= nextChestSpawn)
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
        public int maxSpawns;
    }
}
