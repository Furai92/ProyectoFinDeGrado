using UnityEngine;

public class TechBehaviourLifestealer : TechBase
{
    private float readyTime;

    private const float CHANCE = 5f;
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
        if (Random.Range(0, 101) < CHANCE * Group.Level) 
        {
            readyTime = Time.time + COOLDOWN;
            ObjectPoolManager.GetHealthPickupFromPool().SetUp(target.transform.position);
        }


    }

    public override void OnTechUpgraded()
    {

    }
}
