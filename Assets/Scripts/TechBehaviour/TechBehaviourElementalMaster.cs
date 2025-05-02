using UnityEngine;

public class TechBehaviourElementalMaster : TechBase
{
    public override void OnTechAdded()
    {
        EventManager.EnemyStatusEffectAppliedEvent += OnEnemyStatusEffectApplied;
    }

    public override void OnTechRemoved()
    {
        EventManager.EnemyStatusEffectAppliedEvent -= OnEnemyStatusEffectApplied;
    }

    private void OnEnemyStatusEffectApplied(GameEnums.DamageElement elem, EnemyEntity enem)
    {
        switch (elem) 
        {
            case GameEnums.DamageElement.Fire: { PlayerEntity.ActiveInstance.ChangeBuff("ELEMENTAL_MASTER_FIRE", 1, 1); break; }
            case GameEnums.DamageElement.Frost: { PlayerEntity.ActiveInstance.ChangeBuff("ELEMENTAL_MASTER_FROST", 1, 1); break; }
            case GameEnums.DamageElement.Thunder: { PlayerEntity.ActiveInstance.ChangeBuff("ELEMENTAL_MASTER_THUNDER", 1, 1); break; }
            case GameEnums.DamageElement.Void: { PlayerEntity.ActiveInstance.ChangeBuff("ELEMENTAL_MASTER_VOID", 1, 1); break; }
        }
    }

    public override void OnTechUpgraded()
    {

    }
}
