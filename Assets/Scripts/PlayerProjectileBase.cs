using UnityEngine;

public abstract class PlayerProjectileBase : PlayerAttackBase
{

    private float lifetimeRemaining;
    private LayerMask normalCheckRaycastMask;

    private const float NORMAL_CHECK_RAYCAST_LENGHT = 2f;
    private const float BASE_PROJECTILE_SPEED = 20f;
    private const float LIFETIME = 5f;

    protected abstract void UpdateBehaviour();
    protected abstract void OnWallImpact();
    protected abstract void OnEntityImpact();
    protected abstract void OnLifetimeExpired();
    protected abstract void OnSpawn();


    private void OnEnable()
    {
        normalCheckRaycastMask = LayerMask.GetMask("Walls");
        OnSpawn();
        lifetimeRemaining = LIFETIME;
    }
    private void FixedUpdate()
    {
        transform.Translate(BASE_PROJECTILE_SPEED * Time.fixedDeltaTime * SetupData.timescale * Vector3.forward);
        lifetimeRemaining -= Time.fixedDeltaTime * SetupData.timescale;
        if (lifetimeRemaining < 0) 
        {
            OnLifetimeExpired();
            gameObject.SetActive(false);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        RaycastHit h;
        Physics.Raycast(transform.position, transform.forward, out h, NORMAL_CHECK_RAYCAST_LENGHT, normalCheckRaycastMask);

        if (h.collider != null)
        {
            if (BouncesRemaining > 0)
            {
                Direction = GameTools.AngleReflection(Direction, GameTools.NormalToEuler(h.normal) + 90);
                transform.rotation = Quaternion.Euler(0, Direction, 0);
                BouncesRemaining--;
            }
            else 
            {
                gameObject.SetActive(false);
            }

        }
    }
}
