using UnityEngine;

public class TechBehaviourAdaptation : TechBase
{
    private const string BUFF_HIGH_HP = "ADAPTATION_MIGHT";
    private const string BUFF_LOW_HP = "ADAPTATION_END";
    private const float HEALTH_PERCENT_THS = 0.5f;

    private string previousBuff;

    public override void OnTechAdded()
    {
        EventManager.PlayerFixedTimeIntervalEvent += OnFixedTimeInterval;
        previousBuff = "";
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerFixedTimeIntervalEvent -= OnFixedTimeInterval;
    }

    private void OnFixedTimeInterval() 
    {
        if (PlayerEntity.ActiveInstance.GetHealthPercent() < HEALTH_PERCENT_THS)
        {
            if (previousBuff != BUFF_LOW_HP) 
            {
                PlayerEntity.ActiveInstance.RemoveBuff(BUFF_HIGH_HP);
                PlayerEntity.ActiveInstance.ChangeBuff(BUFF_LOW_HP, Group.Level, 1);
                previousBuff = BUFF_LOW_HP;
            } 
        }
        else 
        {
            if (previousBuff != BUFF_HIGH_HP)
            {
                PlayerEntity.ActiveInstance.RemoveBuff(BUFF_LOW_HP);
                PlayerEntity.ActiveInstance.ChangeBuff(BUFF_HIGH_HP, Group.Level, 1);
                previousBuff = BUFF_HIGH_HP;
            }
        }

    }

    public override void OnTechUpgraded()
    {

    }
}
