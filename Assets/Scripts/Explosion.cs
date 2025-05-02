using UnityEngine;

public class Explosion : PlayerAttackBase
{
    [SerializeField] private Transform explosionVisualParent;

    private float animT;
    private const float LIFETIME = 0.35f;

    public override void SetUp(Vector3 pos, float dir, WeaponAttackSetupData sd, PlayerAttackBase parentAttack) {
        setupData = sd;
        gameObject.SetActive(true);
        transform.position = pos;
        transform.localScale = Vector3.one * sd.SizeMultiplier;
        animT = 0f;
        UpdateAnimation();
    }

    void Update()
    {
        animT += Time.deltaTime / LIFETIME;
        UpdateAnimation();
        if (animT > 1) { gameObject.SetActive(false); }   
    }
    private void UpdateAnimation() 
    {
        explosionVisualParent.localScale = Vector3.one * animT;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("EnemyHitbox"))
        {
            EnemyEntity e = collision.gameObject.GetComponentInParent<EnemyEntity>();
            if (e != null)
            {
                e.DealDirectDamage(setupData.Magnitude, setupData.CritChance, setupData.CritDamage, setupData.BuildupRate, setupData.Element, setupData.DamageType);
                Vector3 knockbackDir = (new Vector3(e.transform.position.x, 0, e.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
                e.Knockback(setupData.Knockback, knockbackDir);
            }
        }
    }
}
