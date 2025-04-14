using UnityEngine;

public class CurrencyPickup : AutoPickup
{
    public override void OnPickup(PlayerEntity p)
    {
        StageManagerBase.ChangeCurrency(1);
    }
}
