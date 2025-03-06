using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private GameDatabase database;

    private Dictionary<string, MonoBehaviourPool<PlayerAttackBase>> playerAttackPools;
    private Dictionary<string, MonoBehaviourPool<EnemyEntity>> enemyPools;
    private Dictionary<string, MonoBehaviourPool<EnemyAttackBase>> enemyAttackPools;
    private MonoBehaviourPool<CurrencyPickup> pickupPool;

    public void InitializePools() 
    {
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
        pickupPool = new MonoBehaviourPool<CurrencyPickup>(database.PickupPrefab, transform);
    }

    public PlayerAttackBase GetPlayerAttackFromPool(string id)
    {
        if (!playerAttackPools.ContainsKey(id)) { return null; }

        return playerAttackPools[id].GetCopyFromPool();
    }

    public EnemyEntity GetEnemyFromPool(string id)
    {
        if (!enemyPools.ContainsKey(id)) { return null; }

        return enemyPools[id].GetCopyFromPool();
    }
    public EnemyAttackBase GetEnemyAttackFromPool(string id)
    {
        if (!enemyAttackPools.ContainsKey(id)) { return null; }

        return enemyAttackPools[id].GetCopyFromPool();
    }
    public CurrencyPickup GetCurrencyPickupFromPool() 
    {
        return pickupPool.GetCopyFromPool();
    }

}
