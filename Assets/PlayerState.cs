using UnityEngine;

public abstract class PlayerState
{
    public abstract void UpdateState(PlayerController player);
    public abstract PlayerState GetNextState(PlayerController player);
}
