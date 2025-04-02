using UnityEngine;

public class EnemyAttackSlowOrb : EnemyAttackBase
{
    private float removeTime;
    private LayerMask terrainCollisionMask;

    private const float TERRAIN_COLLISION_RAYCAST_LENGHT = 1f;
    private const float ORB_SPEED = 7.5f;
    private const float LIFETIME = 10f;

    protected override void OnSetup(Vector3 pos, float dir)
    {
        gameObject.SetActive(true);
        terrainCollisionMask = LayerMask.GetMask("Walls");
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, dir, 0);
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
                        pe.DealDamage(50); 
                    }
                    gameObject.SetActive(false);
                    break;
                }
            case "Deflect": 
                {
                    gameObject.SetActive(false);
                    ObjectPoolManager.GetDeflectedAttackPool().SetUp(200, transform.position);
                    break;
                }
        }
    }
}
