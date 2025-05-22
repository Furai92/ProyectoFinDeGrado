using UnityEngine;

public abstract class TechCombatEffect : MonoBehaviour
{
    protected TechCombatEffectSetupData SetupData;

    public struct TechCombatEffectSetupData 
    {
        public float magnitude;
        public GameEnums.DamageElement element;
        public int enemyIgnored;
        public float sizeMult;
    }
    protected abstract void Initialize(Vector3 pos, float dir);
    public void SetUp(Vector3 pos, float dir, TechCombatEffectSetupData sd) 
    {
        SetupData = sd;
        Initialize(pos, dir);
    }

    public void DamageEnemy(EnemyEntity e) 
    {
        if (e.EnemyInstanceID == SetupData.enemyIgnored) { return; }

        e.DealDirectDamage(SetupData.magnitude, 0, 1, GameTools.IntellectToBuildupMultiplier(PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.Intellect)), 
            SetupData.element, GameEnums.DamageType.Tech);
    }
}
