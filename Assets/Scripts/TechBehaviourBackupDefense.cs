using UnityEngine;

public class TechBehaviourBackupDefense : TechBase
{
    private const float HP_TO_SHIELD_RATIO = 0.5f;

    public override void OnTechAdded()
    {
        EventManager.PlayerDamageTakenEvent += OnPlayerDamageTaken;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerDamageTakenEvent -= OnPlayerDamageTaken;
    }

    private void OnPlayerDamageTaken(float mag)
    {
        PlayerEntity.ActiveInstance.AddShield(HP_TO_SHIELD_RATIO * mag * Group.Level);
    }

    public override void OnTechUpgraded()
    {

    }
}
