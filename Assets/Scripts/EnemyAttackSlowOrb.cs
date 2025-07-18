using UnityEngine;

public class EnemyAttackSlowOrb : EnemyAttackBase
{
    private float removeTime;
    private LayerMask terrainCollisionMask;

    private const float BASE_DAMAGE = 25f;
    private const float TERRAIN_COLLISION_RAYCAST_LENGHT = 1f;
    private const float ORB_SPEED = 15f;
    private const float LIFETIME = 10f;
    private const float SPAWN_HEIGHT = 0.5f;

    public override void Initialize(Vector3 pos, float direction)
    {
        gameObject.SetActive(true);
        terrainCollisionMask = LayerMask.GetMask("Walls");
        transform.position = new Vector3(pos.x, SPAWN_HEIGHT, pos.z);
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
                    if (pe == null) { return; }

                    if (DamagePlayer(pe, BASE_DAMAGE)) { gameObject.SetActive(false); }
                    break;
                }
            case "Deflect": 
                {
                    gameObject.SetActive(false);
                    EventManager.OnPlayerDeflect(transform.position);
                    ObjectPoolManager.GetDeflectedAttackPool().SetUp(200, transform.position);
                    ObjectPoolManager.GetImpactEffectFromPool("DEFLECT").SetUp(transform.position, 0, GameEnums.DamageElement.NonElemental);
                    break;
                }
        }
    }

    public override void OnUserStunned()
    {
        // No actions
    }

    public override void OnUserDefeated()
    {
        // No actions
    }

    public override void OnStageStateEnded()
    {
        gameObject.SetActive(false);
    }
}
