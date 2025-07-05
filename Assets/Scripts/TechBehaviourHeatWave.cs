using UnityEngine;

public class TechBehaviourHeatWave : TechBase
{
    private float readyTime;

    private const float DAMAGE = 400f;
    private const float COOLDOWN = 5f;
    private const float WAVE_SIZE = 5f;
    private const float HEAT_GENERATED = 10f;


    public override void OnTechAdded()
    {
        EventManager.PlayerAttackStartedEvent += OnPlayerAttackStarted;
        readyTime = 0;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerAttackStartedEvent -= OnPlayerAttackStarted;
    }

    private void OnPlayerAttackStarted(Vector3 pos, WeaponSO.WeaponSlot slot, float dir)
    {
        if (slot != WeaponSO.WeaponSlot.Ranged) { return; }
        if (Time.time < readyTime) { return; }

        readyTime = Time.time + COOLDOWN;
        TechCombatEffect.TechCombatEffectSetupData sd = new TechCombatEffect.TechCombatEffectSetupData()
        {
            sizeMult = WAVE_SIZE,
            element = GameEnums.DamageElement.Fire,
            enemyIgnored = -1,
            magnitude = DAMAGE
        };
        ObjectPoolManager.GetTechCombatEffectFromPool("WAVE").SetUp(pos, dir, sd);
        PlayerEntity.ActiveInstance.ChangeWeaponHeat(HEAT_GENERATED * Group.Level, WeaponSO.WeaponSlot.Ranged);
    }


    public override void OnTechUpgraded()
    {

    }
}
