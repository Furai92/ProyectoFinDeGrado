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
        PlayerEntity.ActiveInstance.ChangeWeaponHeat(-PlayerEntity.ActiveInstance.GetHeatRange() * COOLING_PERCENT * Group.Level, s);
    }

    public override void OnTechUpgraded()
    {

    }
}
