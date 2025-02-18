using UnityEngine;

public abstract class PlayerProjectileBase : PlayerAttackBase
{

    private float lifetimeRemaining;

    private const float BASE_PROJECTILE_SPEED = 10f;
    private const float LIFETIME = 5f;

    protected abstract void UpdateBehaviour();
    protected abstract void OnWallImpact();
    protected abstract void OnEntityImpact();
    protected abstract void OnLifetimeExpired();
    protected abstract void OnSpawn();


    private void OnEnable()
    {
        OnSpawn();
        lifetimeRemaining = LIFETIME;
    }
    private void FixedUpdate()
    {
        transform.Translate(BASE_PROJECTILE_SPEED * Time.fixedDeltaTime * Timescale * Vector3.forward);
        lifetimeRemaining -= Time.fixedDeltaTime * Timescale;
        if (lifetimeRemaining < 0) 
        {
            OnLifetimeExpired();
            gameObject.SetActive(false);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        gameObject.SetActive(false);
    }
}
