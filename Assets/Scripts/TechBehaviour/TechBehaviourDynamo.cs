using UnityEngine;

public class TechBehaviourDynamo : TechBase
{
    private float readyTime;

    private const float SHIELD = 5f;
    private const float COOLDOWN = 0.25f;

    public override void OnTechAdded()
    {
        EventManager.EnemyDirectDamageTakenEvent += OnEnemyDamageTaken;
        readyTime = 0;
    }

    public override void OnTechRemoved()
    {
        EventManager.EnemyDirectDamageTakenEvent -= OnEnemyDamageTaken;
    }

    private void OnEnemyDamageTaken(float mag, int clevel, GameEnums.DamageElement elem, GameEnums.DamageType dtype, EnemyEntity target)
    {
        if (dtype != GameEnums.DamageType.Melee) { return; }
        if (Time.time < readyTime) { return; }

        PlayerEntity.ActiveInstance.AddShield(SHIELD * Group.Level);
        readyTime = Time.time + COOLDOWN;
    }

    public override void OnTechUpgraded()
    {

    }
}
