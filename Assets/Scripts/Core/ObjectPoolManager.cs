using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private GameDatabase database;

    private Dictionary<string, MonoBehaviourPool<PlayerAttackBase>> attackPools;
    private Dictionary<string, MonoBehaviourPool<EnemyEntity>> enemyPools;
    private MonoBehaviourPool<CurrencyPickup> pickupPool;

    public void InitializePools() 
    {
        // Attacks
        attackPools = new Dictionary<string, MonoBehaviourPool<PlayerAttackBase>>();
        for (int i = 0; i < database.PlayerAttackPrefabs.Count; i++) 
        {
            attackPools.Add(database.PlayerAttackPrefabs[i].ID, new MonoBehaviourPool<PlayerAttackBase>(database.PlayerAttackPrefabs[i].Data, transform));
        }
        // Enemies
        enemyPools = new Dictionary<string, MonoBehaviourPool<EnemyEntity>>();
        for (int i = 0; i < database.EnemyPrefabs.Count; i++) 
        {
            enemyPools.Add(database.EnemyPrefabs[i].ID, new MonoBehaviourPool<EnemyEntity>(database.EnemyPrefabs[i].Data, transform));
        }
        // Pickups
        pickupPool = new MonoBehaviourPool<CurrencyPickup>(database.PickupPrefab, transform);
    }

    public PlayerAttackBase GetPlayerAttackFromPool(string id)
    {
        if (!attackPools.ContainsKey(id)) { return null; }

        return attackPools[id].GetCopyFromPool();
    }

    public EnemyEntity GetEnemyFromPool(string id)
    {
        if (!enemyPools.ContainsKey(id)) { return null; }

        return enemyPools[id].GetCopyFromPool();
    }
    public CurrencyPickup GetCurrencyPickupFromPool() 
    {
        return pickupPool.GetCopyFromPool();
    }

}
