using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "Scriptable Objects/WeaponSO")]
public class WeaponSO : ScriptableObject
{
    public enum WeaponSlot { Melee, Ranged }

    [field: SerializeField] public WeaponSlot Slot { get; private set; }
    [field: SerializeField] public string ID { get; private set; }
    [field: SerializeField] public string NameID { get; private set; }
    [field: SerializeField] public List<string> Pools { get; private set; }
    [field: SerializeField] public string ProjectileComponentID { get; private set; }
    [field: SerializeField] public string CleaveComponentID { get; private set; }

    [field: SerializeField] public GameEnums.DamageElement Element { get; private set; }
    [field: SerializeField] public float ProjectileComponentDamage { get; private set; }
    [field: SerializeField] public float CleaveComponentDamage { get; private set; }
    [field: SerializeField] public int Multishoot { get; private set; }
    [field: SerializeField] public float MultishootArc { get; private set; }
    [field: SerializeField] public float Firerate { get; private set; }
    [field: SerializeField] public float BuildupRate { get; private set; }
    [field: SerializeField] public float CriticalDamage { get; private set; }
    [field: SerializeField] public float SizeMultiplier { get; private set; }
    [field: SerializeField] public float HeatGeneration { get; private set; }
    [field: SerializeField] public float Splash { get; private set; }
    [field: SerializeField] public int Bounces { get; private set; }
    [field: SerializeField] public int Pierces { get; private set; }
    [field: SerializeField] public float Timescale { get; private set; }
    [field: SerializeField] public float Knockback { get; private set; }
    [field: SerializeField] public List<WeaponSOPartSlot> PartSlots { get; private set; }

    [System.Serializable]
    public class WeaponSOPartSlot 
    {
        [field: SerializeField] public string SlotName { get; private set; }
        [field: SerializeField] public List<WeaponPartSO> CompatibleParts { get; private set; }
    }
}
