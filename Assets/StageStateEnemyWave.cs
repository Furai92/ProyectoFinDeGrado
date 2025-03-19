using UnityEngine;
using System.Collections.Generic;

public class StageStateEnemyWave : StageStateBase
{
    private float waveDuration;
    private float durationRemaining;
    private float nextEnemySpawn;
    private const float ENEMY_SPAWN_INTERVAL = 2.5f;
    private const int MAX_ENEMIES = 20;
    private const float MIN_DIST_TO_SPAWN = 40f;
    private const float MAX_DIST_TO_SPAWN = 75f;

    public StageStateEnemyWave(float duration) 
    {
        waveDuration = duration;
    }

    public override bool IsFinished()
    {
        return durationRemaining <= 0;
    }

    public override void StateEnd()
    {
        StageManagerBase.DisableAllEnemies();
    }

    public override void StateStart()
    {
        nextEnemySpawn = Time.time + ENEMY_SPAWN_INTERVAL;
        durationRemaining = waveDuration;
    }

    public override void UpdateState()
    {
        if (Time.time > nextEnemySpawn) { SpawnEnemy(); nextEnemySpawn = Time.time + ENEMY_SPAWN_INTERVAL; }

        durationRemaining -= Time.deltaTime;
    }
    public override float GetTimerDisplay()
    {
        return durationRemaining;
    }
    private void SpawnEnemy()
    {
        if (StageManagerBase.GetEnemyCount() >= MAX_ENEMIES) { return; }

        List<Vector3> allSpawns = StageManagerBase.GetEnemySpawnPositions();
        List<Vector3> validSpawns = new List<Vector3>();
        Vector3 playerPos = StageManagerBase.GetRandomPlayerPosition();

        for (int i = 0; i < allSpawns.Count; i++)
        {
            float dist = Vector3.Distance(allSpawns[i], playerPos);
            if (dist < MIN_DIST_TO_SPAWN || dist > MAX_DIST_TO_SPAWN) { continue; }
            validSpawns.Add(allSpawns[i]);
        }

        ObjectPoolManager.GetEnemyFromPool("DEBUG_WALKING").SetUp(validSpawns[Random.Range(0, validSpawns.Count)]);
    }
}
