using UnityEngine;

public class StageStatGroup
{
    public enum StageStat { EnemyHealthMult, EnemyDamageMult, EnemySpawnRate }

    private float[] stats;

    private const int STAT_ARRAY_LENGHT = 10;

    public StageStatGroup()
    {
        stats = new float[STAT_ARRAY_LENGHT];
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
