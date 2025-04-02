using UnityEngine;

public class StatGroup
{
    public enum Stat { MaxHealth, Might, Dexterity, Intellect, Endurance,  DashCount, CritChance, Speed, Firerate, DashRechargeRate, PickupRadius, CritBonusDamage }
    public enum PlayerFlags { NoHeal }

    private float[] stats;
    private int[] flags;

    private const int STAT_ARRAY_LENGHT = 10;
    private const int FLAG_ARRAY_LENGHT = 5;

    public StatGroup() 
    {
        stats = new float[STAT_ARRAY_LENGHT];
        flags = new int[FLAG_ARRAY_LENGHT];
    }
    public void ChangeStat(Stat s, float change) 
    {
        stats[(int)s] += change;
    }
    public void ChangeFlag(PlayerFlags f, int change)
    {
        flags[(int)f] += change;
    }
    public float GetStat(Stat s) 
    {
        return stats[(int)s];
    }
    public int GetFlag(PlayerFlags f) 
    {
        return flags[(int)f];
    }
}
