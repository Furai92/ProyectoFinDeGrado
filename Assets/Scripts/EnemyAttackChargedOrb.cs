using UnityEngine;

public class EnemyAttackChargedOrb : EnemyAttackBase
{
    private float removeTime;
    private LayerMask terrainCollisionMask;

    private const float BASE_DAMAGE = 20f;
    private const float TERRAIN_COLLISION_RAYCAST_LENGHT = 1f;
    private const float ORB_SPEED = 7.5f;
    private const float LIFETIME = 10f;

    public override void Initialize(Vector3 pos, float direction)
    {
        gameObject.SetActive(true);
        terrainCollisionMask = LayerMask.GetMask("Walls");
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, direction, 0);
        removeTime = Time.time + LIFETIME;
    }
    private void FixedUpdate()
    {
        transform.Translate(ORB_SPEED * Time.fixedDeltaTime * Vector3.forward);
        if (Time.time > removeTime) { gameObject.SetActive(false); }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (Physics.Raycast(transform.position, transform.forward, TERRAIN_COLLISION_RAYCAST_LENGHT, terrainCollisionMask))
            {
                gameObject.SetActive(false);
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.collider.gameObject.tag)
        {
            case "Player":
                {
                    PlayerEntity pe = collision.gameObject.GetComponentInParent<PlayerEntity>();
                    if (pe != null)
                    {
                        pe.DealDamage(BASE_DAMAGE, true, false, false);
                    }
                    gameObject.SetActive(false);
                    break;
                }
            case "Deflect":
                {
                    gameObject.SetActive(false);

                    ObjectPoolManager.GetDeflectedAttackPool().SetUp(BASE_DAMAGE, transform.position);
                    break;
                }
        }
    }

    public override void OnUserStunned()
    {
        
    }

    public override void OnUserDefeated()
    {
        
    }

    public override void OnStageStateEnded()
    {
        gameObject.SetActive(false);
    }
}
