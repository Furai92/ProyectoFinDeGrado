using UnityEngine;

public class TechBehaviourColdSynergy : TechBase
{
    private const float COOLING_PERCENT = 0.1f;


    public override void OnTechAdded()
    {
        EventManager.EnemyStatusEffectAppliedEvent += OnEnemyStatusEffectApplied;
    }

    public override void OnTechRemoved()
    {
        EventManager.EnemyStatusEffectAppliedEvent -= OnEnemyStatusEffectApplied;
    }

    private void OnEnemyStatusEffectApplied(GameEnums.DamageElement elem, EnemyEntity e)
    {
        if (elem != GameEnums.DamageElement.Frost) { return; }

        PlayerEntity.ActiveInstance.ChangeWeaponHeat(PlayerEntity.ActiveInstance.GetHeatRange() * -COOLING_PERCENT * Group.Level, WeaponSO.WeaponSlot.Melee);
        PlayerEntity.ActiveInstance.ChangeWeaponHeat(PlayerEntity.ActiveInstance.GetHeatRange() * -COOLING_PERCENT * Group.Level, WeaponSO.WeaponSlot.Ranged);
    }

    public override void OnTechUpgraded()
    {

    }
}
