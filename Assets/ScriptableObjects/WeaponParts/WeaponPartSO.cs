using UnityEngine;

[CreateAssetMenu(fileName = "WeaponPartSO", menuName = "Scriptable Objects/WeaponPartSO")]
public class WeaponPartSO : ScriptableObject
{
    [field: SerializeField] public string ProjectileComponentIDOverride { get; private set; }
    [field: SerializeField] public string CleaveComponentIDOverride { get; private set; }
    [field: SerializeField] public float ProjectileComponentDamageOverride { get; private set; }
    [field: SerializeField] public float CleaveComponentDamageOverride { get; private set; }
    [field: SerializeField] public GameEnums.DamageElement ElementOverride { get; private set; }
    [field: SerializeField] public float DamageMultiplier { get; private set; }
    [field: SerializeField] public int MultishootModifier { get; private set; }
    [field: SerializeField] public float ArcModifier { get; private set; }
    [field: SerializeField] public float FirerateMultiplier { get; private set; }
    [field: SerializeField] public float BuildupMultiplier { get; private set; }
    [field: SerializeField] public float CriticalDamageMultiplier { get; private set; }
    [field: SerializeField] public float SizeMultiplier { get; private set; }
    [field: SerializeField] public float HeatGenerationMultiplier { get; private set; }
    [field: SerializeField] public float SplashModifier { get; private set; }
    [field: SerializeField] public float SpreadModifier { get; private set; }
    [field: SerializeField] public int BouncesModifier { get; private set; }
    [field: SerializeField] public int PiercesModifier { get; private set; }
    [field: SerializeField] public float TimescaleMultiplier { get; private set; }
    [field: SerializeField] public float KnockbackModifier { get; private set; }
}
