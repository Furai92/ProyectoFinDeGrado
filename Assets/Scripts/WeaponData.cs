using UnityEngine;
using System.Collections.Generic;

public class WeaponData
{
    public WeaponSO BaseWeapon { get; private set; }
    public List<WeaponPartSO> Parts { get; private set; }

    public WeaponData(WeaponSO baseweapon, List<WeaponPartSO> p) 
    {
        BaseWeapon = baseweapon;
        Parts = p;
    }
    public WeaponStats GetStats() 
    {
        WeaponStats ws = new WeaponStats()
        {
            Element = BaseWeapon.Element,
            RangedAttackID = BaseWeapon.RangedComponentID,
            MeleeAttackID = BaseWeapon.MeleeComponentID,
            RangedMagnitude = BaseWeapon.RangedComponentDamage,
            MeleeMagnitude = BaseWeapon.MeleeComponentDamage,
            Firerate = BaseWeapon.Firerate,
            Multishoot = BaseWeapon.Multishoot,
            Arc = BaseWeapon.MultishootArc,
            Pierces = BaseWeapon.Pierces,
            Bounces = BaseWeapon.Bounces,
            Splash = BaseWeapon.Splash,
            CritMultiplier = BaseWeapon.CriticalDamage,
            BuildupRate = BaseWeapon.BuildupRate,
            Knockback = BaseWeapon.Knockback,
            HeatGen = BaseWeapon.HeatGeneration,
            Timescale = BaseWeapon.Timescale,
            SizeMultiplier = BaseWeapon.SizeMultiplier,
        };
        for (int i = 0; i < Parts.Count; i++) 
        {
            if (Parts[i].ElementOverride != GameEnums.DamageElement.None) { ws.Element = Parts[i].ElementOverride; }
            if (Parts[i].RangedAttackOverrideID != "") { ws.RangedAttackID = Parts[i].RangedAttackOverrideID; }
            if (Parts[i].MeleeAttackOverrideID != "") { ws.MeleeAttackID = Parts[i].MeleeAttackOverrideID; }
            ws.MeleeMagnitude *= Parts[i].DamageMultiplier;
            ws.RangedMagnitude *= Parts[i].DamageMultiplier;
            ws.Firerate *= Parts[i].FirerateMultiplier;
            ws.Multishoot = Mathf.Max(ws.Multishoot + Parts[i].MultishootModifier, 1);
            ws.Pierces = Mathf.Max(ws.Pierces + Parts[i].PiercesModifier, 0);
            ws.Bounces = Mathf.Max(ws.Bounces + Parts[i].BouncesModifier, 0);
            ws.Splash = Mathf.Max(ws.Splash + Parts[i].SplashModifier, 0);
            ws.Arc = Mathf.Max(ws.Arc + Parts[i].ArcModifier, 0);
            ws.CritMultiplier *= Parts[i].CriticalDamageMultiplier;
            ws.BuildupRate *= Parts[i].BuildupMultiplier;
            ws.Knockback += Mathf.Max(ws.Knockback + Parts[i].KnockbackMultiplier, 0);
            ws.HeatGen *= Parts[i].HeatGenerationMultiplier;
            ws.Timescale *= Parts[i].TimescaleMultiplier;
            ws.SizeMultiplier = Parts[i].SizeMultiplier;
        }

        return ws;
    }

    public struct WeaponStats 
    {
        public GameEnums.DamageElement Element;
        public string RangedAttackID;
        public string MeleeAttackID;
        public float RangedMagnitude;
        public float MeleeMagnitude;
        public float Firerate;
        public int Multishoot;
        public float Arc;
        public int Pierces;
        public int Bounces;
        public float Splash;
        public float CritMultiplier;
        public float BuildupRate;
        public float Knockback;
        public float HeatGen;
        public float Timescale;
        public float SizeMultiplier;
    }
}
