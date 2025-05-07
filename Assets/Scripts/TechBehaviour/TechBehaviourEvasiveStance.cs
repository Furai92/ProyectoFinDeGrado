using UnityEngine;

public class TechBehaviourEvasiveStance : TechBase
{
    private const float DASH_CHARGES_RECOVERED = 0.1f;

    public override void OnTechAdded()
    {
        EventManager.EnemyDirectDamageTakenEvent += OnEnemyDamageTaken;
    }

    public override void OnTechRemoved()
    {
        EventManager.EnemyDirectDamageTakenEvent -= OnEnemyDamageTaken;
    }

    private void OnEnemyDamageTaken(float mag, int clevel, GameEnums.DamageElement elem, GameEnums.DamageType dtype, EnemyEntity target)
    {
        if (dtype != GameEnums.DamageType.Melee) { return; }

        PlayerEntity.ActiveInstance.RestoreDashCharges(DASH_CHARGES_RECOVERED * Group.Level);
    }

    public override void OnTechUpgraded()
    {

    }
}
