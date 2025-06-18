using UnityEngine;

public class StageStatGroup
{
    public enum StageStat { EnemyHealthMult, 
        EnemyDamageMult, 
        EnemySpawnRate, 
        ChestSpawnRate, 
        ChestDropRate, 
        WeaponDropRate, 
        HealthOrbDropRate, 
        ShopPriceMult, 
        DropLevelFactor, 
        WaveDurationMult, 
        BossHealthMult,
        EnemySpeedMult,
        ShopRerollBaseCost,
        EliteChance,
        EnemyExtraHealthBarCount,
    }

    private float[] stats;

    private const int STAT_ARRAY_LENGHT = 20;

    public StageStatGroup()
    {
        stats = new float[STAT_ARRAY_LENGHT];

        stats[(int)StageStat.EnemyHealthMult] = 1;
        stats[(int)StageStat.EnemyDamageMult] = 1;
        stats[(int)StageStat.EnemySpawnRate] = 1;
        stats[(int)StageStat.ChestSpawnRate] = 1;
        stats[(int)StageStat.ChestDropRate] = 1;
        stats[(int)StageStat.WeaponDropRate] = 1;
        stats[(int)StageStat.HealthOrbDropRate] = 1;
        stats[(int)StageStat.ShopPriceMult] = 1;
        stats[(int)StageStat.DropLevelFactor] = 0;
        stats[(int)StageStat.WaveDurationMult] = 1;
        stats[(int)StageStat.BossHealthMult] = 1;
        stats[(int)StageStat.EnemySpeedMult] = 1;
        stats[(int)StageStat.ShopRerollBaseCost] = 0;
        stats[(int)StageStat.WaveDurationMult] = 1;
        stats[(int)StageStat.EliteChance] = 0;
        stats[(int)StageStat.EnemyExtraHealthBarCount] = 0;
    }
    public void ChangeStat(StageStat s, float change)
    {
        stats[(int)s] += change;
    }
    public float GetStat(StageStat s)
    {
        return stats[(int)s];
    }
}
