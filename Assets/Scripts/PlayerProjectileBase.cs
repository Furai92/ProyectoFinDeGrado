using UnityEngine;

public abstract class PlayerProjectileBase : PlayerAttackBase
{
    private WeaponAttackSetupData setupData;
    private float lifetimeRemaining;
    protected int bouncesRemaining;
    protected int piercesRemaining;
    protected float currentDirection;
    private LayerMask normalCheckRaycastMask;

    private const float NORMAL_CHECK_RAYCAST_LENGHT = 2f;
    private const float BASE_PROJECTILE_SPEED = 20f;
    private const float LIFETIME = 5f;

    protected abstract void UpdateBehaviour();
    protected abstract void OnWallImpact();
    protected abstract void OnEntityImpact();
    protected abstract void OnLifetimeExpired();
    protected abstract void OnSpawn();


    public override void SetUp(Vector3 pos, float dir, WeaponAttackSetupData sd) 
    {
        setupData = sd;
        transform.position = pos;
        bouncesRemaining = sd.bounces;
        piercesRemaining = sd.pierces;
        SetNewDirection(dir);
        normalCheckRaycastMask = LayerMask.GetMask("Walls");
        OnSpawn();
        lifetimeRemaining = LIFETIME;
        gameObject.gameObject.SetActive(true);
    }
    private void SetNewDirection(float newdir) 
    {
        currentDirection = newdir;
        transform.rotation = Quaternion.Euler(0, currentDirection, 0);
    }
    private void OnEnable()
    {

    }
    private void FixedUpdate()
    {
        transform.Translate(BASE_PROJECTILE_SPEED * Time.fixedDeltaTime * setupData.timescale * Vector3.forward);
        lifetimeRemaining -= Time.fixedDeltaTime * setupData.timescale;
        if (lifetimeRemaining < 0) 
        {
            OnLifetimeExpired();
            gameObject.SetActive(false);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("EnemyHitbox")) 
        {
            EnemyEntity e = collision.gameObject.GetComponentInParent<EnemyEntity>();
            if (e != null)
            {
                e.DealDamage(setupData.magnitude, setupData.critchance, setupData.critdamage, setupData.builduprate, setupData.element);
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            RaycastHit h;
            Physics.Raycast(transform.position, transform.forward, out h, NORMAL_CHECK_RAYCAST_LENGHT, normalCheckRaycastMask);

            if (h.collider != null)
            {
                if (bouncesRemaining > 0)
                {
                    SetNewDirection(GameTools.AngleReflection(currentDirection, GameTools.NormalToEuler(h.normal) + 90));
                    bouncesRemaining--;
                }
                else
                {
                    gameObject.SetActive(false);
                }

            }

        }

    }
}
