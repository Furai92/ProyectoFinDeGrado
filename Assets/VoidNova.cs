using UnityEngine;

public class VoidNova : MonoBehaviour
{
    [SerializeField] private Transform visualParent;

    private float damageMag;
    private float pullMag;
    private float animT;
    private EnemyEntity noKnockbackEnemy;

    private const float MIN_VISUAL_SCALE = 0.1f;
    private const float DURATION = 0.5f;

    public void SetUp(float _pullMag, float _damageMag, float sizeMult, Vector3 pos, EnemyEntity _noKnockbackEnemy) 
    {
        transform.localScale = Vector3.one * sizeMult;
        noKnockbackEnemy = _noKnockbackEnemy;
        pullMag = _pullMag;
        damageMag = _damageMag;
        transform.position = pos;
        animT = 0;
        visualParent.transform.localScale = Vector3.one;
        gameObject.SetActive(true);
    }
    private void Update()
    {
        animT += Time.deltaTime / DURATION;
        visualParent.transform.localScale = Vector3.one * Mathf.Lerp(1, MIN_VISUAL_SCALE, animT);

        if (animT > 1) { gameObject.SetActive(false); }
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
                    Vector3 knockbackDir = (new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(e.transform.position.x, 0, e.transform.position.z)).normalized;
                    e.Knockback(pullMag, knockbackDir);
                }
            }
        }
    }
}
