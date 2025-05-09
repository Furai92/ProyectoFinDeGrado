using UnityEngine;

public class TechBehaviourBackfire : TechBase
{
    private const float RADIUS = 10f;
    private const float HEAT_TO_DAMAGE = 10f;

    public override void OnTechAdded()
    {
        EventManager.PlayerWeaponOverheatedEvent += OnPlayerWeaponOverheated;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerWeaponOverheatedEvent -= OnPlayerWeaponOverheated;
    }

    private void OnPlayerWeaponOverheated(WeaponSO.WeaponSlot s)
    {
        TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData()
        {
            sizeMult = RADIUS,
            element = GameEnums.DamageElement.Fire,
            enemyIgnored = -1,
            magnitude = HEAT_TO_DAMAGE * PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.HeatCap)
        };
        ObjectPoolManager.GetTechCombatEffectFromPool("EXPLOSION").SetUp(PlayerEntity.ActiveInstance.transform.position, 0, sd);
    }

    public override void OnTechUpgraded()
    {

    }
}
