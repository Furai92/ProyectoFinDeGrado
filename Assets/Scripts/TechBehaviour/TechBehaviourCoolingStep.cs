using UnityEngine;

public class TechBehaviourCoolingStep : TechBase
{
    private const float BASE_COOLING_RATE = 0.05f;

    public override void OnTechAdded()
    {
        EventManager.PlayerEvasionEvent += OnPlayerEvasion;
    }

    public override void OnTechRemoved()
    {
        EventManager.PlayerEvasionEvent -= OnPlayerEvasion;
    }

    private void OnPlayerEvasion(Vector3 pos) 
    {
        PlayerRef.CoolDownWeapons(BASE_COOLING_RATE * Group.Level);
    }

    public override void OnTechUpgraded()
    {

    }
}
