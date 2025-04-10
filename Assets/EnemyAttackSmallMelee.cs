using UnityEngine;

public class EnemyAttackSmallMelee : EnemyAttackBase
{
    private float removeTime;

    private const float BASE_DAMAGE = 50f;
    private const float LIFETIME = 0.5f;

    public override void Initialize(Vector3 pos, float dir)
    {
        gameObject.SetActive(true);
        removeTime = Time.time + LIFETIME;
        transform.SetPositionAndRotation(User.transform.position, Quaternion.Euler(0, dir, 0));
    }

    private void FixedUpdate()
    {
        transform.position = User.transform.position;
        if (Time.time > removeTime) 
        {
            gameObject.SetActive(false);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            PlayerEntity pe = collision.gameObject.GetComponentInParent<PlayerEntity>();
            if (pe != null)
            {
                pe.DealDamage(BASE_DAMAGE);
            }
        }
    }

    public override void OnUserDefeated()
    {
        gameObject.SetActive(false);
    }

    public override void OnUserStunned()
    {
        gameObject.SetActive(false);
    }
}
