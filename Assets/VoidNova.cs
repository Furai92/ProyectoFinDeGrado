using UnityEngine;

public class VoidNova : MonoBehaviour
{
    [SerializeField] private ParticleSystem PS0;
    [SerializeField] private ParticleSystem PS1;

    private float damageMag;
    private EnemyEntity noKnockbackEnemy;
    private float removeTime;

    private const float PULL_MAGNITUDE = 8f;
    private const float DURATION = 0.5f;

    public void SetUp(float _damageMag, float sizeMult, Vector3 pos, EnemyEntity _noKnockbackEnemy) 
    {
        transform.localScale = Vector3.one * sizeMult;
        noKnockbackEnemy = _noKnockbackEnemy;
        damageMag = _damageMag;
        transform.position = pos;
        removeTime = Time.time + DURATION;

        gameObject.SetActive(true);
        PS0.Stop();
        PS0.Play();
        PS1.Stop();
        PS1.Play();
    }
    private void Update()
    {
        if (Time.time > removeTime) { gameObject.SetActive(false); }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("EnemyHitbox"))
        {
            EnemyEntity e = collision.gameObject.GetComponentInParent<EnemyEntity>();
            if (e != null)
            {
                e.DealStatusDamage(damageMag, GameEnums.DamageElement.Void);

                if (e != noKnockbackEnemy) 
                {
                    e.AddPullForce(PULL_MAGNITUDE, transform.position, DURATION);
                }
            }
        }
    }
}
