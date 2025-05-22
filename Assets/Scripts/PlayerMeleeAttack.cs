using UnityEngine;
using UnityEngine.VFX;

public class PlayerMeleeAttack : PlayerAttackBase
{
    [SerializeField] private ColorDatabaseSO cdb;

    private float removeTime;
    private float direction;

    private const float LIFETIME = 0.5f;

    public override void SetUp(Vector3 pos, float dir, WeaponAttackSetupData sd, PlayerAttackBase parentAttack)
    {
        direction = dir;
        transform.rotation = Quaternion.Euler(0, dir, 0);
        setupData = sd;
        removeTime = Time.time + LIFETIME;
        gameObject.SetActive(true);
    }
    private void FixedUpdate()
    {
        if (Time.time > removeTime) { gameObject.SetActive(false); }
        transform.position = setupData.User.transform.position;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("EnemyHitbox"))
        {
            EnemyEntity e = collision.gameObject.GetComponentInParent<EnemyEntity>();
            if (e != null)
            {
                PlayImpactEffects(e.transform.position, direction);
                e.DealDirectDamage(setupData.Magnitude, setupData.CritChance, setupData.CritDamage, setupData.BuildupRate, setupData.Element, setupData.DamageType);
                Vector3 knockbackDir = (new Vector3(e.transform.position.x, 0, e.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
                e.Knockback(setupData.Knockback, knockbackDir);
            }
        }
    }
}
