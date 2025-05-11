using UnityEngine;

public class TechBehaviourEyeOfTheStorm : TechBase
{

    private const string BUFF_ID = "EYE_OF_THE_STORM";

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

        
    }

    public override void OnTechUpgraded()
    {

    }
}
