using UnityEngine;

public class TechBehaviourFusionCharge : TechBase
{
    private const string BUFF_ID = "FUSION_CHARGE";

    public override void OnTechAdded()
    {
        EventManager.PlayerWeaponOverheatedEvent += OnPlayerWeaponOverheated;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerWeaponOverheatedEvent -= OnPlayerWeaponOverheated;
    }

    private void OnPlayerWeaponOverheated()
    {
        PlayerEntity.ActiveInstance.ChangeBuff(BUFF_ID, Group.Level, 1);
    }

    public override void OnTechUpgraded()
    {

    }
}
