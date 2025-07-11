using UnityEngine;

public class HealthPickup : AutoPickup
{
    private const float BASE_HEALING = 20;

    public override void OnPickup(PlayerEntity p)
    {
        p.Heal(BASE_HEALING);
        EventManager.OnPlayerHealthOrbGathered(p.transform.position);
    }
}
