using UnityEngine;

public class DeflectedAttack : MonoBehaviour
{
    private float magnitude;
    private float removeTime;
    private LayerMask terrainCollisionMask;

    private const float TRACKING_RANGE = 40f;
    private const float TERRAIN_COLLISION_RAYCAST_LENGHT = 1f;
    private const float SPEED = 15f;
    private const float LIFETIME = 10f;

    public void SetUp(float mag, Vector3 pos)
    {
        transform.position = pos;
        magnitude = mag;
        EnemyEntity closestEnemy = StageManagerBase.GetClosestEnemyInRange(null, transform.position, TRACKING_RANGE);
        float direction = closestEnemy == null ? Random.Range(0, 361) : GameTools.AngleBetween(transform.position, closestEnemy.transform.position);
        transform.rotation = Quaternion.Euler(0, direction, 0);
        gameObject.SetActive(true);
        terrainCollisionMask = LayerMask.GetMask("Walls");
        removeTime = Time.time + LIFETIME;
    }
    private void FixedUpdate()
    {
        transform.Translate(SPEED * Time.fixedDeltaTime * Vector3.forward);
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
        switch (collision.gameObject.tag)
        {
            case "EnemyHitbox":
                {
                    collision.gameObject.GetComponentInParent<EnemyEntity>().DealDirectDamage(magnitude, 0, 2, 0, GameEnums.DamageElement.Physical);
                    gameObject.SetActive(false);
                    break;
                }
        }
    }
}
