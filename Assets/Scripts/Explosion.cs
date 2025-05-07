using UnityEngine;

public class Explosion : PlayerAttackBase
{
    [SerializeField] private Transform explosionVisualParent;
    [SerializeField] private ParticleSystem PS0;
    [SerializeField] private ParticleSystem PS1;
    [SerializeField] private ColorDatabaseSO cdb;

    private float removeTime;
    private const float LIFETIME = 0.5f;
 

    public override void SetUp(Vector3 pos, float dir, WeaponAttackSetupData sd, PlayerAttackBase parentAttack) {
        setupData = sd;
        gameObject.SetActive(true);
        transform.position = pos;
        transform.localScale = Vector3.one * sd.SizeMultiplier;
        removeTime = Time.time + LIFETIME;

        ParticleSystem.MainModule m0 = PS0.main; m0.startColor = cdb.ElementToColor(sd.Element);
        ParticleSystem.MainModule m1 = PS1.main; m1.startColor = cdb.ElementToColor(sd.Element);

        PS0.Stop();
        PS0.Play();
        PS1.Stop();
        PS1.Play();
    }

    void Update()
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
                e.DealDirectDamage(setupData.Magnitude, setupData.CritChance, setupData.CritDamage, setupData.BuildupRate, setupData.Element, setupData.DamageType);
                Vector3 knockbackDir = (new Vector3(e.transform.position.x, 0, e.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
                e.Knockback(setupData.Knockback, knockbackDir);
            }
        }
    }
}
