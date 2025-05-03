using UnityEngine;

public class GameEnums
{
    public const int DAMAGE_ELEMENTS = 6;
    public enum DamageElement { NonElemental, Fire, Frost, Thunder, Void }
    public enum Rarity { Common, Rare, Exotic, Prototype, Special }
    public enum DamageType { Melee, Ranged, Tech, Deflect }
}
