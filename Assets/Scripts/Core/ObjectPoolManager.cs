using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private GameDatabaseSO database;

    private Dictionary<string, MonoBehaviourPool<PlayerAttackBase>> playerAttackPools;
    private Dictionary<string, MonoBehaviourPool<EnemyEntity>> enemyPools;
    private Dictionary<string, MonoBehaviourPool<EnemyAttackBase>> enemyAttackPools;
    private Dictionary<string, MonoBehaviourPool<PlayerAttackImpactEffect>> impactEffectPools;
    private Dictionary<string, MonoBehaviourPool<TechCombatEffect>> techCombatEffectPools;

    private MonoBehaviourPool<CurrencyPickup> currencyPickupPool;
    private MonoBehaviourPool<HealthPickup> healthPickupPool;
    private MonoBehaviourPool<WeaponPickup> weaponPickupPool;
    private MonoBehaviourPool<Chest> chestPool;
    private MonoBehaviourPool<DeflectedAttack> deflectedAttackPool;
    private MonoBehaviourPool<VoidNova> voidNovaPool;

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
        // Impact Effects
        impactEffectPools = new Dictionary<string, MonoBehaviourPool<PlayerAttackImpactEffect>>();
        for (int i = 0; i < database.ImpactEffects.Count; i++)
        {
            impactEffectPools.Add(database.ImpactEffects[i].ID, new MonoBehaviourPool<PlayerAttackImpactEffect>(database.ImpactEffects[i].Data, transform));
        }
        // Tech Combat Effects
        techCombatEffectPools = new Dictionary<string, MonoBehaviourPool<TechCombatEffect>>();
        for (int i = 0; i < database.TechEffectPrefabs.Count; i++)
        {
            techCombatEffectPools.Add(database.TechEffectPrefabs[i].ID, new MonoBehaviourPool<TechCombatEffect>(database.TechEffectPrefabs[i].Data, transform));
        }
        // Currency Pickups
        currencyPickupPool = new MonoBehaviourPool<CurrencyPickup>(database.CurrencyPickupPrefab, transform);
        // Currency Pickups
        healthPickupPool = new MonoBehaviourPool<HealthPickup>(database.HealthPickupPrefab, transform);
        // Weapon Pickups
        weaponPickupPool = new MonoBehaviourPool<WeaponPickup>(database.WeaponPickupPrefab, transform);
        // Chests
        chestPool = new MonoBehaviourPool<Chest>(database.ChestPrefab, transform);
        // Deflected attacks
        deflectedAttackPool = new MonoBehaviourPool<DeflectedAttack>(database.DeflectedAttackPrefab, transform);
        // Void Nova
        voidNovaPool = new MonoBehaviourPool<VoidNova>(database.VoidNovaPrefab, transform);
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
    public static PlayerAttackImpactEffect GetImpactEffectFromPool(string id)
    {
        if (instance == null) { return null; }
        if (!instance.impactEffectPools.ContainsKey(id)) { return null; }

        return instance.impactEffectPools[id].GetCopyFromPool();
    }
    public static TechCombatEffect GetTechCombatEffectFromPool(string id)
    {
        if (instance == null) { return null; }
        if (!instance.techCombatEffectPools.ContainsKey(id)) { return null; }

        return instance.techCombatEffectPools[id].GetCopyFromPool();
    }
    public static CurrencyPickup GetCurrencyPickupFromPool()
    {
        if (instance == null) { return null; }

        return instance.currencyPickupPool.GetCopyFromPool();
    }
    public static HealthPickup GetHealthPickupFromPool() 
    {
        if (instance == null) { return null; }

        return instance.healthPickupPool.GetCopyFromPool();
    }
    public static WeaponPickup GetWeaponPickupFromPool()
    {
        if (instance == null) { return null; }

        return instance.weaponPickupPool.GetCopyFromPool();
    }
    public static Chest GetChestFromPool()
    {
        if (instance == null) { return null; }

        return instance.chestPool.GetCopyFromPool();
    }
    public static DeflectedAttack GetDeflectedAttackPool()
    {
        if (instance == null) { return null; }

        return instance.deflectedAttackPool.GetCopyFromPool();
    }
    public static VoidNova GetVoidNovaFromPool()
    {
        if (instance == null) { return null; }

        return instance.voidNovaPool.GetCopyFromPool();
    }

}
