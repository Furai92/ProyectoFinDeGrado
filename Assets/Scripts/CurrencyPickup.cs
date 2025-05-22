using UnityEngine;

public class CurrencyPickup : AutoPickup
{
    public override void OnPickup(PlayerEntity p)
    {
        p.AddMoney(1);
    }
}
