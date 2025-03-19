using UnityEngine;

public abstract class PlayerState
{
    public abstract void UpdateState(PlayerEntity player);
    public abstract PlayerState GetNextState(PlayerEntity player);
}
