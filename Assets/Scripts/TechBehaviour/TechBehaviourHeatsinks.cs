using UnityEngine;

public class TechBehaviourHeatsinks : TechBase
{
    private const float COOLING_PERCENT = 0.15f;

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
        float heatRange = PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.HeatFloor) + PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.HeatCap);
        PlayerEntity.ActiveInstance.ChangeWeaponHeat(-heatRange * COOLING_PERCENT * Group.Level, s);
    }

    public override void OnTechUpgraded()
    {

    }
}
