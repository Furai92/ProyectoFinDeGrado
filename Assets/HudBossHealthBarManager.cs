using UnityEngine;
using System.Collections.Generic;

public class HudBossHealthBarManager : MonoBehaviour
{
    [SerializeField] private GameObject elementPrefab;
    [SerializeField] private Transform barLG;

    private MonoBehaviourPool<HudBossHealthBarElement> elementPool;

    private void OnEnable()
    {
        elementPool = new MonoBehaviourPool<HudBossHealthBarElement>(elementPrefab, barLG);
        EventManager.EnemySpawnedEvent += OnEnemySpawned;
    }
    private void OnDisable()
    {
        EventManager.EnemySpawnedEvent -= OnEnemySpawned;
    }

    private void OnEnemySpawned(EnemyEntity e)
    {
        if (e.Rank != GameEnums.EnemyRank.Boss) { return; }

        HudBossHealthBarElement hpbar = elementPool.GetCopyFromPool();
        hpbar.SetUp(e);
    }
}

