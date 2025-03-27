using UnityEngine;
using System.Collections.Generic;

public class WeaponData
{
    public WeaponSO BaseWeapon { get; private set; }
    public List<WeaponPartSO> Parts { get; private set; }

    public WeaponData(WeaponSO baseweapon) 
    {
        BaseWeapon = baseweapon;
        Parts = new List<WeaponPartSO>();
        for (int i = 0; i < baseweapon.PartSlots.Count; i++)
        {
            Parts.Add(baseweapon.PartSlots[i].CompatibleParts[Random.Range(0, baseweapon.PartSlots[i].CompatibleParts.Count)]);
        }
    }
    public WeaponStats GetStats() 
    {
        WeaponStats ws = new WeaponStats()
        {
            Element = BaseWeapon.Element,
            ProjectileComponentID = BaseWeapon.ProjectileComponentID,
            CleaveComponentID = BaseWeapon.CleaveComponentID,
            ProjectileComponentMagnitude = BaseWeapon.ProjectileComponentDamage,
            CleaveComponentMagnitude = BaseWeapon.CleaveComponentDamage,
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
            if (Parts[i].ProjectileComponentIDOverride != "") { ws.ProjectileComponentID = Parts[i].ProjectileComponentIDOverride; }
            if (Parts[i].CleaveComponentIDOverride != "") { ws.CleaveComponentID = Parts[i].CleaveComponentIDOverride; }
            if (Parts[i].ProjectileComponentDamageOverride != 0) { ws.ProjectileComponentMagnitude = Parts[i].ProjectileComponentDamageOverride; }
            if (Parts[i].CleaveComponentDamageOverride != 0) { ws.CleaveComponentMagnitude = Parts[i].CleaveComponentDamageOverride; }

            ws.CleaveComponentMagnitude *= Parts[i].DamageMultiplier;
            ws.ProjectileComponentMagnitude *= Parts[i].DamageMultiplier;
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
        public string ProjectileComponentID;
        public string CleaveComponentID;
        public float ProjectileComponentMagnitude;
        public float CleaveComponentMagnitude;
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
