using UnityEngine;

public class GameEnums
{
    public const int DAMAGE_ELEMENTS = 7;
    public enum DamageElement { NonElemental, Fire, Frost, Thunder, Void, Incineration, Frostbite }
    public enum Rarity { Common, Rare, Exotic, Prototype, Special }
    public enum DamageType { Melee, Ranged, Tech, Deflect }
}
