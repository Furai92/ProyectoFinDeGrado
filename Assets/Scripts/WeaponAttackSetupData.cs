using UnityEngine;

public class WeaponAttackSetupData
{
    public PlayerEntity User { get; set; }
    public string ImpactEffectID { get; set; }
    public GameEnums.DamageElement Element { get; set; }
    public float Magnitude { get; set; }
    public float CritChance { get; set; }
    public float CritDamage { get; set; }
    public float Timescale { get; set; }
    public float SizeMultiplier { get; set; }
    public float BuildupRate { get; set; }
    public int Bounces { get; set; }
    public int Pierces { get; set; }
    public float Splash { get; set; }
    public float Knockback { get; set; }
    public int EnemyIgnored { get; set; }

    public WeaponAttackSetupData() 
    {
        User = null;
        Element = GameEnums.DamageElement.Physical;
        Magnitude = 0;
        CritChance = 0;
        CritDamage = 1;
        Timescale = 1;
        SizeMultiplier = 1;
        BuildupRate = 1;
        Bounces = 0;
        Pierces = 0;
        Splash = 0;
        Knockback = 0;
        EnemyIgnored = -1;
    }
    public WeaponAttackSetupData(WeaponAttackSetupData sd) 
    {
        User = sd.User;
        Element = sd.Element;
        Magnitude = sd.Magnitude;
        CritChance = sd.CritChance;
        CritDamage = sd.CritDamage;
        Timescale = sd.Timescale;
        SizeMultiplier = sd.SizeMultiplier;
        BuildupRate = sd.BuildupRate;
        Bounces = sd.Bounces;
        Pierces = sd.Pierces;
        Splash = sd.Splash;
        Knockback = sd.Knockback;
        EnemyIgnored = sd.EnemyIgnored;
        ImpactEffectID = sd.ImpactEffectID;
    }

}
