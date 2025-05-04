using UnityEngine;

public class PlayerStatGroup
{
    public enum Stat {
        MaxHealth,
        Might, 
        Dexterity, 
        Intellect, 
        Endurance,  
        DashCount,
        DashBonusDuration,
        CritChance, 
        Speed, 
        Firerate, 
        DashRechargeRate, 
        PickupRadius, 
        CritBonusDamage,
        HeatFloor,
        HeatCap,
        DamageToLightConversion,
        DamageToShieldConversion,
        EchoChance,
        RandomSpread,
        HeatGenIncrease,
    }
    public enum PlayerFlags { NoHeal }

    private float[] stats;
    private int[] flags;

    private const int STAT_ARRAY_LENGHT = 20;
    private const int FLAG_ARRAY_LENGHT = 5;

    public PlayerStatGroup() 
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
