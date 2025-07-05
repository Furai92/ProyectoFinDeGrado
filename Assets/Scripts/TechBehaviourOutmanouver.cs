using UnityEngine;

public class TechBehaviourOutmanouver : TechBase
{
    private const string BUFF_ID = "OUTMANOUVER";

    public override void OnTechAdded()
    {
        EventManager.PlayerDashStartedEvent += OnPlayerDashStarted;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerDashStartedEvent -= OnPlayerDashStarted;
    }

    private void OnPlayerDashStarted(Vector3 pos, float dir)
    {
        PlayerEntity.ActiveInstance.ChangeBuff(BUFF_ID, Group.Level, 1);
    }

    public override void OnTechUpgraded()
    {

    }
}
