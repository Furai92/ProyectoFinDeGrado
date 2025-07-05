using UnityEngine;

public class TechBehaviourStaticCharge : TechBase
{
    private const string BUFF_ID = "STATIC_CHARGE";

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
        if (clevel < 1) { return; }
        if (dtype != GameEnums.DamageType.Melee) { return; }

        PlayerEntity.ActiveInstance.ChangeBuff(BUFF_ID, Group.Level, 1);
    }

    public override void OnTechUpgraded()
    {

    }
}
