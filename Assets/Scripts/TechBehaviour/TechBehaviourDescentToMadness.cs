using UnityEngine;

public class TechBehaviourDescentToMadness : TechBase
{
    private bool effectEnabled;

    private const int STACKS_TO_LOSE_CONTROL = 10;
    private const string STACKABLE_BUFF_ID = "DESCENT";
    private const string LOSE_CONTROL_BUFF_ID = "MADNESS";

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
        if (slot != WeaponSO.WeaponSlot.Ranged) { return; }

        PlayerEntity.ActiveInstance.ChangeBuff(STACKABLE_BUFF_ID, Group.Level, 1);
        if (PlayerEntity.ActiveInstance.ActiveBuffDictionary[STACKABLE_BUFF_ID].Stacks == STACKS_TO_LOSE_CONTROL) 
        {
            PlayerEntity.ActiveInstance.ChangeBuff(LOSE_CONTROL_BUFF_ID, Group.Level, 1);
        }
    }
    private void OnWeaponOverheated(WeaponSO.WeaponSlot s) 
    {
        if (s != WeaponSO.WeaponSlot.Ranged) { return; }

        PlayerEntity.ActiveInstance.RemoveBuff(STACKABLE_BUFF_ID);
        PlayerEntity.ActiveInstance.RemoveBuff(LOSE_CONTROL_BUFF_ID);
    }
    private void OnStageStateStarted(StageStateBase.GameState s) 
    {
        effectEnabled = s != StageStateBase.GameState.Rest;
        PlayerEntity.ActiveInstance.RemoveBuff(STACKABLE_BUFF_ID);
        PlayerEntity.ActiveInstance.RemoveBuff(LOSE_CONTROL_BUFF_ID);
    }

    public override void OnTechUpgraded()
    {

    }
}
