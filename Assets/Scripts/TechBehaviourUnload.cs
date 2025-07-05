using UnityEngine;

public class TechBehaviourUnload : TechBase
{
    private bool effectEnabled;

    private const float CHANCE = 33f;
    private const string BUFF_ID = "UNLOAD";

    public override void OnTechAdded()
    {
        effectEnabled = StageManagerBase.GetCurrentStateType() != StageStateBase.GameState.Rest;
        EventManager.PlayerAttackStartedEvent += OnPlayerAttackStarted;
        EventManager.PlayerWeaponOverheatedEvent += OnWeaponOverheated;
        EventManager.StageStateStartedEvent += OnStageStateStarted;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerAttackStartedEvent -= OnPlayerAttackStarted;
        EventManager.PlayerWeaponOverheatedEvent -= OnWeaponOverheated;
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
    }

    private void OnPlayerAttackStarted(Vector3 pos, WeaponSO.WeaponSlot slot, float dir) 
    {
        if (!effectEnabled) { return; }
        if (Random.Range(1, 101) > CHANCE) { return; }

        PlayerEntity.ActiveInstance.ChangeBuff(BUFF_ID, Group.Level, 1);
    }
    private void OnWeaponOverheated(WeaponSO.WeaponSlot s) 
    {
        PlayerEntity.ActiveInstance.RemoveBuff(BUFF_ID);
    }
    private void OnStageStateStarted(StageStateBase.GameState s) 
    {
        effectEnabled = s != StageStateBase.GameState.Rest;
        PlayerEntity.ActiveInstance.RemoveBuff(BUFF_ID);
    }

    public override void OnTechUpgraded()
    {

    }
}
