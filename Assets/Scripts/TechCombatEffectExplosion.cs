using UnityEngine;

public class TechCombatEffectExplosion : TechCombatEffect
{
    [SerializeField] private ParticleSystem PS0;
    [SerializeField] private ParticleSystem PS1;
    [SerializeField] private ColorDatabaseSO cdb;

    private float damageEndTime;
    private float removeTime;

    private const float LIFETIME = 1f;
    private const float ACTIVE_DAMAGE_DURATION = 0.5f;

    protected override void Initialize(Vector3 pos, float dir)
    {
        transform.position = pos;
        removeTime = Time.time + LIFETIME;
        damageEndTime = Time.time + ACTIVE_DAMAGE_DURATION;
        transform.localScale = Vector3.one * SetupData.sizeMult;
        gameObject.SetActive(true);

        ParticleSystem.MainModule m0 = PS0.main; m0.startColor = cdb.ElementToColor(SetupData.element);
        ParticleSystem.MainModule m1 = PS1.main; m1.startColor = cdb.ElementToColor(SetupData.element);

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
        if (Time.time > damageEndTime) { return; }

        if (collision.gameObject.CompareTag("EnemyHitbox"))
        {
            EnemyEntity e = collision.gameObject.GetComponentInParent<EnemyEntity>();
            if (e != null)
            {
                DamageEnemy(e);
            }
        }
    }
}
