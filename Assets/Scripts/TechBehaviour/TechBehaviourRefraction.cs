using UnityEngine;

public class TechBehaviourRefraction : TechBase
{
    private const float DAMAGE_MULT = 1.5f;

    public override void OnTechAdded()
    {
        EventManager.PlayerProjectileBounceEvent += OnPlayerProjectileBounce;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerProjectileBounceEvent -= OnPlayerProjectileBounce;
    }

    private void OnPlayerProjectileBounce(PlayerAttackBase attack, bool entityBounce)
    {
        if (entityBounce) { return; }

        attack.AmplifyDamage(DAMAGE_MULT);
    }

    public override void OnTechUpgraded()
    {

    }
}
