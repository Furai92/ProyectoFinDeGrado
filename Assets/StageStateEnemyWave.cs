using UnityEngine;
using System.Collections.Generic;

public class StageStateEnemyWave : StageStateBase
{
    private float waveDuration;
    private float durationRemaining;
    private float nextEnemySpawn;
    private float nextChestSpawn;
    private const float ENEMY_SPAWN_INTERVAL = 2.5f;
    private const float CHEST_SPAWN_INTERVAL = 10f;
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
        nextChestSpawn = Time.time; // First one spawns instantly
        durationRemaining = waveDuration;
    }

    public override void UpdateState()
    {
        if (Time.time > nextEnemySpawn) { SpawnEnemy(); nextEnemySpawn = Time.time + ENEMY_SPAWN_INTERVAL; }
        if (Time.time > nextChestSpawn) { SpawnChest(); nextChestSpawn = Time.time + CHEST_SPAWN_INTERVAL; }
        durationRemaining -= Time.deltaTime;
    }
    public override float GetTimerDisplay()
    {
        return durationRemaining;
    }
    private void SpawnChest() 
    {
        List<Vector3> allPositions = StageManagerBase.GetChestSpawnPositions();
        Vector3 pos = allPositions[Random.Range(0, allPositions.Count)];
        ObjectPoolManager.GetChestFromPool().SetUp(pos);
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

    public override StateType GetStateType()
    {
        return StateType.Combat;
    }
}
