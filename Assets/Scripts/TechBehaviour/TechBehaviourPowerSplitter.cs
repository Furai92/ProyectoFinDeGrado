using UnityEngine;

public class TechBehaviourPowerSplitter : TechBase
{
    private const float CHANCE = 15f;

    public override void OnTechAdded()
    {
        EventManager.PlayerDeflectEvent += OnPlayerDeflect;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerDeflectEvent -= OnPlayerDeflect;
    }

    private void OnPlayerDeflect(Vector3 wpos)
    {
        if (Random.Range(1, 101) < CHANCE * Group.Level) { ObjectPoolManager.GetHealthPickupFromPool().SetUp(wpos); }
    }

    public override void OnTechUpgraded()
    {

    }
}
