using UnityEngine;
using System.Collections.Generic;


public class ProceduralStagemanager : StageManagerBase
{
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private List<GameObject> roomPrefabs;
    [SerializeField] private List<GameObject> corridorPrefabs;
    [SerializeField] private List<GameObject> decoPrefabs;

    private MonoBehaviourPool<EnemyEntity> enemyPool;
    private float nextEnemySpawn;

    private const float ENEMY_SPAWN_INTERVAL = 2.5f;
    private const int MAX_ENEMIES = 20;
    private const float MIN_DIST_TO_SPAWN = 75f;
    private const float MAX_DIST_TO_SPAWN = 100f;

    public override IMapData GenerateMap(int seed)
    {
        return new ProceduralStageData(seed, 30, roomPrefabs, corridorPrefabs, decoPrefabs, transform);
    }

    public override void InitializeStage()
    {
        enemyPool = new MonoBehaviourPool<EnemyEntity>(enemyPrefab, transform);
        nextEnemySpawn = Time.time + ENEMY_SPAWN_INTERVAL;
    }
    private void Update()
    {
        if (Time.time > nextEnemySpawn) { SpawnEnemy(); nextEnemySpawn = Time.time + ENEMY_SPAWN_INTERVAL; }
    }
    private void SpawnEnemy() 
    {
        if (enemiesInStage.Count >= MAX_ENEMIES) { return; }

        List<Vector3> allSpawns = stageMapData.GetEnemySpawnPositions();
        List<Vector3> validSpawns = new List<Vector3>();
        Vector3 playerPos = GetRandomPlayerPosition();

        for (int i = 0; i < allSpawns.Count; i++) 
        {
            float dist = Vector3.Distance(allSpawns[i], playerPos);
            if (dist < MIN_DIST_TO_SPAWN || dist > MAX_DIST_TO_SPAWN) { continue; }
            validSpawns.Add(allSpawns[i]);
        }
        print(validSpawns.Count);
        enemyPool.GetCopyFromPool().SetUp(validSpawns[Random.Range(0, validSpawns.Count)]);
    }
}
