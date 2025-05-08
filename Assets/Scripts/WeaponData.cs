using UnityEngine;
using System.Collections.Generic;

public class WeaponData
{
    public WeaponSO BaseWeapon { get; private set; }
    public WeaponPartSO CorePart { get; private set; }
    public List<WeaponPartSO> SpecialParts { get; private set; }
    public GameEnums.Rarity Rarity { get; private set; }
    public WeaponStats Stats { get; private set; }

    private const float RARITY_CHANCE_RARE = 25f;
    private const float RARITY_CHANCE_EXOTIC = 5f;
    private const float RARITY_CHANCE_PROTOTYPE = 1f;

    public WeaponData(WeaponSO baseWeapon) 
    {
        BaseWeapon = baseWeapon;
        RollRarity();
        RollParts();
    }
    public WeaponData(WeaponSO baseweapon, GameEnums.Rarity forcedRarity) 
    {
        BaseWeapon = baseweapon;
        Rarity = forcedRarity;
        RollParts();
    }
    private void RollRarity() 
    {
        int rarityRoll = Random.Range(0, 101);
        if (rarityRoll < RARITY_CHANCE_RARE)
        {
            if (rarityRoll < RARITY_CHANCE_EXOTIC)
            {
                Rarity = rarityRoll < RARITY_CHANCE_PROTOTYPE ? Rarity = GameEnums.Rarity.Prototype : Rarity = GameEnums.Rarity.Exotic;
            }
            else
            {
                Rarity = GameEnums.Rarity.Rare;
            }
        }
        else
        {
            Rarity = GameEnums.Rarity.Common; // TEMP
        }
    }
    private void RollParts() 
    {
        CorePart = BaseWeapon.CoreParts[Random.Range(0, BaseWeapon.CoreParts.Count)];

        SpecialParts = new List<WeaponPartSO>();
        if ((int)Rarity >= (int)GameEnums.Rarity.Rare && BaseWeapon.RareParts.Count > 0) { SpecialParts.Add(BaseWeapon.RareParts[Random.Range(0, BaseWeapon.RareParts.Count)]); }
        if ((int)Rarity >= (int)GameEnums.Rarity.Exotic && BaseWeapon.ExoticParts.Count > 0) { SpecialParts.Add(BaseWeapon.ExoticParts[Random.Range(0, BaseWeapon.ExoticParts.Count)]); }
        if ((int)Rarity >= (int)GameEnums.Rarity.Prototype && BaseWeapon.PrototypeParts.Count > 0) { SpecialParts.Add(BaseWeapon.PrototypeParts[Random.Range(0, BaseWeapon.PrototypeParts.Count)]); }

        GenerateStats();
    }
    public void GenerateStats() 
    {
        List<WeaponPartSO> usedParts = new List<WeaponPartSO>();
        usedParts.Add(CorePart);
        for (int i = 0; i < SpecialParts.Count; i++) { usedParts.Add(SpecialParts[i]); }

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
            RandomSpread = BaseWeapon.RandomSpread,
            Pierces = BaseWeapon.Pierces,
            Bounces = BaseWeapon.Bounces,
            Splash = BaseWeapon.Splash,
            CritMultiplier = BaseWeapon.CriticalDamage,
            BuildupRate = BaseWeapon.BuildupRate,
            Knockback = BaseWeapon.Knockback,
            HeatGen = BaseWeapon.HeatGeneration,
            Timescale = BaseWeapon.Timescale,
            SizeMultiplier = BaseWeapon.SizeMultiplier,
            ImpactEffectID = BaseWeapon.ImpactEffectID
        };
        for (int i = 0; i < usedParts.Count; i++) 
        {
            if (usedParts[i].ElementOverride != GameEnums.DamageElement.NonElemental) { ws.Element = usedParts[i].ElementOverride; }
            if (usedParts[i].ProjectileComponentIDOverride != "") { ws.ProjectileComponentID = usedParts[i].ProjectileComponentIDOverride; }
            if (usedParts[i].CleaveComponentIDOverride != "") { ws.CleaveComponentID = usedParts[i].CleaveComponentIDOverride; }
            if (usedParts[i].ProjectileComponentDamageOverride != 0) { ws.ProjectileComponentMagnitude = usedParts[i].ProjectileComponentDamageOverride; }
            if (usedParts[i].CleaveComponentDamageOverride != 0) { ws.CleaveComponentMagnitude = usedParts[i].CleaveComponentDamageOverride; }

            ws.CleaveComponentMagnitude *= usedParts[i].DamageMultiplier;
            ws.ProjectileComponentMagnitude *= usedParts[i].DamageMultiplier;
            ws.Firerate *= usedParts[i].FirerateMultiplier;
            ws.Multishoot = Mathf.Max(ws.Multishoot + usedParts[i].MultishootModifier, 1);
            ws.Pierces = Mathf.Max(ws.Pierces + usedParts[i].PiercesModifier, 0);
            ws.Bounces = Mathf.Max(ws.Bounces + usedParts[i].BouncesModifier, 0);
            ws.Splash = Mathf.Max(ws.Splash + usedParts[i].SplashModifier, 0);
            ws.Arc = Mathf.Max(ws.Arc + usedParts[i].ArcModifier, 0);
            ws.RandomSpread += usedParts[i].SpreadModifier;
            ws.CritMultiplier *= usedParts[i].CriticalDamageMultiplier;
            ws.BuildupRate *= usedParts[i].BuildupMultiplier;
            ws.Knockback += Mathf.Max(ws.Knockback + usedParts[i].KnockbackModifier, 0);
            ws.HeatGen *= usedParts[i].HeatGenerationMultiplier;
            ws.Timescale *= usedParts[i].TimescaleMultiplier;
            ws.SizeMultiplier = usedParts[i].SizeMultiplier;
        }
        // Fix possible inspector strings to null bugs
        if (ws.CleaveComponentID == null) { ws.CleaveComponentID = ""; }
        if (ws.ProjectileComponentID == null) { ws.ProjectileComponentID = ""; }

        Stats = ws;
    }

    public struct WeaponStats 
    {
        public GameEnums.DamageElement Element;
        public string ProjectileComponentID;
        public string CleaveComponentID;
        public string ImpactEffectID;
        public float ProjectileComponentMagnitude;
        public float CleaveComponentMagnitude;
        public float Firerate;
        public int Multishoot;
        public float Arc;
        public int Pierces;
        public int Bounces;
        public float RandomSpread;
        public float Splash;
        public float CritMultiplier;
        public float BuildupRate;
        public float Knockback;
        public float HeatGen;
        public float Timescale;
        public float SizeMultiplier;
    }
}
