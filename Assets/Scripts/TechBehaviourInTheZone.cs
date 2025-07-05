using UnityEngine;

public class TechBehaviourInTheZone : TechBase
{
    private const string BUFF_ID = "IN_THE_ZONE";


    public override void OnTechAdded()
    {
        EventManager.PlayerDamageTakenEvent += OnPlayerDamageTaken;
        EventManager.StageStateEndedEvent += OnStageStateEnded;
        EventManager.PlayerEvasionEvent += OnPlayerEvasion;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerDamageTakenEvent -= OnPlayerDamageTaken;
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
        EventManager.PlayerEvasionEvent -= OnPlayerEvasion;
    }
    private void OnPlayerEvasion(Vector3 wpos) 
    {
        PlayerEntity.ActiveInstance.ChangeBuff(BUFF_ID, 1, Group.Level);
    }
    private void OnPlayerDamageTaken(float dmg) 
    {
        PlayerEntity.ActiveInstance.RemoveBuff(BUFF_ID);
    }
    private void OnStageStateEnded(StageStateBase.GameState s) 
    {
        PlayerEntity.ActiveInstance.RemoveBuff(BUFF_ID);
    }

    public override void OnTechUpgraded()
    {

    }
}
