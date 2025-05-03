using UnityEngine;

public class TechBehaviourBeastStrenght : TechBase
{
    private const float CHANCE_TO_TRIGGER = 33f;
    private const float HEALING = 1f;

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
        if (Random.Range(0, 101) > CHANCE_TO_TRIGGER) { return; }

        PlayerEntity.ActiveInstance.Heal(HEALING * Group.Level);
    }

    public override void OnTechUpgraded()
    {

    }
}
