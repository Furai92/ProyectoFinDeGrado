using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private GameDatabase database;

    private Dictionary<string, MonoBehaviourPool<PlayerAttackBase>> playerAttackPools;
    private Dictionary<string, MonoBehaviourPool<EnemyEntity>> enemyPools;
    private Dictionary<string, MonoBehaviourPool<EnemyAttackBase>> enemyAttackPools;
    private MonoBehaviourPool<AutoPickup> pickupPool;

    private static ObjectPoolManager instance;

    public void InitializePools() 
    {
        instance = this;
        // Player Attacks
        playerAttackPools = new Dictionary<string, MonoBehaviourPool<PlayerAttackBase>>();
        for (int i = 0; i < database.PlayerAttackPrefabs.Count; i++) 
        {
            playerAttackPools.Add(database.PlayerAttackPrefabs[i].ID, new MonoBehaviourPool<PlayerAttackBase>(database.PlayerAttackPrefabs[i].Data, transform));
        }
        // Enemies
        enemyPools = new Dictionary<string, MonoBehaviourPool<EnemyEntity>>();
        for (int i = 0; i < database.EnemyPrefabs.Count; i++) 
        {
            enemyPools.Add(database.EnemyPrefabs[i].ID, new MonoBehaviourPool<EnemyEntity>(database.EnemyPrefabs[i].Data, transform));
        }
        // Enemy Attacks
        enemyAttackPools = new Dictionary<string, MonoBehaviourPool<EnemyAttackBase>>();
        for (int i = 0; i < database.EnemyAttackPrefabs.Count; i++)
        {
            enemyAttackPools.Add(database.EnemyAttackPrefabs[i].ID, new MonoBehaviourPool<EnemyAttackBase>(database.EnemyAttackPrefabs[i].Data, transform));
        }
        // Pickups
        pickupPool = new MonoBehaviourPool<AutoPickup>(database.PickupPrefab, transform);
    }

    public static PlayerAttackBase GetPlayerAttackFromPool(string id)
    {
        if (instance == null) { return null; }
        if (!instance.playerAttackPools.ContainsKey(id)) { return null; }

        return instance.playerAttackPools[id].GetCopyFromPool();
    }

    public static EnemyEntity GetEnemyFromPool(string id)
    {
        if (instance == null) { return null; }
        if (!instance.enemyPools.ContainsKey(id)) { return null; }

        return instance.enemyPools[id].GetCopyFromPool();
    }
    public static EnemyAttackBase GetEnemyAttackFromPool(string id)
    {
        if (instance == null) { return null; }
        if (!instance.enemyAttackPools.ContainsKey(id)) { return null; }

        return instance.enemyAttackPools[id].GetCopyFromPool();
    }
    public static AutoPickup GetCurrencyPickupFromPool() 
    {
        if (instance == null) { return null; }

        return instance.pickupPool.GetCopyFromPool();
    }

}
