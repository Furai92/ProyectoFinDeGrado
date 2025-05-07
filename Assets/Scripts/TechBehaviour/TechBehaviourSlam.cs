using UnityEngine;

public class TechBehaviourSlam : TechBase
{
    private const float DAMAGE = 300f;
    private const float RADIUS = 8f;

    public override void OnTechAdded()
    {
        EventManager.PlayerWallSlamEvent += OnPlayerWallSlam;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerWallSlamEvent -= OnPlayerWallSlam;
    }

    private void OnPlayerWallSlam(Vector3 pos)
    {
        TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData()
        {
            sizeMult = RADIUS,
            element = PlayerEntity.ActiveInstance.MeleeWeapon.GetStats().Element,
            enemyIgnored = -1,
            magnitude = DAMAGE * Group.Level
        };
        ObjectPoolManager.GetTechCombatEffectFromPool("EXPLOSION").SetUp(pos, 0, sd);
    }

    public override void OnTechUpgraded()
    {

    }
}
