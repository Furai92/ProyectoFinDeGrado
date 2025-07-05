using UnityEngine;

public class GameEnums
{
    public const int DAMAGE_ELEMENTS = 10;
    public enum DamageElement { None, NonElemental, Fire, Frost, Thunder, Void, Incineration, Frostbite }
    public enum Rarity { Common, Rare, Exotic, Prototype, Special }
    public enum DamageType { Melee, Ranged, Tech, Deflect }
    public enum EnemyRank { Normal, Elite, Boss }
}
