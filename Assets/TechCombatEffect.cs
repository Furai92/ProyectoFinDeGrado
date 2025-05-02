using UnityEngine;

public abstract class TechCombatEffect : MonoBehaviour
{
    public struct TechCombatEffectSetupData 
    {
        public float magnitude;
        public GameEnums.DamageElement element;
        public int enemyIgnored;
    }
}
