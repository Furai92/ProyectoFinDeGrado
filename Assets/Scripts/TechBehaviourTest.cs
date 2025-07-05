using UnityEngine;

public class TechBehaviourTest : TechBase
{


    public override void OnTechAdded()
    {
        EventManager.PlayerAttackStartedEvent += OnPlayerAttackStarted;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerAttackStartedEvent -= OnPlayerAttackStarted;
    }

    private void OnPlayerAttackStarted(Vector3 pos, WeaponSO.WeaponSlot s, float dir) 
    {
        if (s == WeaponSO.WeaponSlot.Melee)
        {
            PlayerEntity.ActiveInstance.ChangeBuff("TEST", 2, 1);
        }
        else 
        {
            PlayerEntity.ActiveInstance.ChangeBuff("TEST", 1, 1);

        }
    }


    public override void OnTechUpgraded()
    {

    }
}
