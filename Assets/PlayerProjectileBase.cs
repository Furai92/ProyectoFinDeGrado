using UnityEngine;

public abstract class PlayerProjectileBase : PlayerAttackBase
{
    protected abstract void UpdateBehaviour();
    protected abstract void OnWallImpact();
    protected abstract void OnEntityImpact();
    protected abstract void OnLifetimeExpired();

    private void FixedUpdate()
    {
        
    }
    private void OnCollisionStay(Collision collision)
    {
        
    }
}
