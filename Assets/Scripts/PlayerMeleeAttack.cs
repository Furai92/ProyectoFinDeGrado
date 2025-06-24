using UnityEngine;
using UnityEngine.VFX;

public class PlayerMeleeAttack : PlayerAttackBase
{
    [SerializeField] private ColorDatabaseSO cdb;
    [SerializeField] private Transform rotationParent;
    [SerializeField] private MeshRenderer mrColored;
    [SerializeField] private MeshRenderer mrFlash;

    private static bool reversedAnim = false;

    private float direction;
    private float t;
    private float randomInclination;

    private const float ANIM_SIZE_MIN = 0.75f;
    private const float ANIM_SIZE_MAX = 1f;
    private const float LIFETIME = 0.2f;
    private const float ANIM_ROTATION = 65f;
    private const float RANDOM_INCLINATION_RANGE = 10f;

    public override void SetUp(Vector3 pos, float dir, WeaponAttackSetupData sd, PlayerAttackBase parentAttack)
    {
        reversedAnim = !reversedAnim;
        direction = dir;
        transform.rotation = Quaternion.Euler(0, dir, 0);
        transform.localScale = Vector3.one * sd.SizeMultiplier;
        setupData = sd;
        randomInclination = Random.Range(-RANDOM_INCLINATION_RANGE, RANDOM_INCLINATION_RANGE);
        t = 0;
        mrColored.material.SetColor("_Color", cdb.ElementToColor(sd.Element));
        UpdateAnimation(t);
        gameObject.SetActive(true);
    }
    private void FixedUpdate()
    {
        t += Time.fixedDeltaTime / LIFETIME;
        UpdateAnimation(t);
        if (t >= 1) { gameObject.SetActive(false); }
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
    private void UpdateAnimation(float animT) 
    {
        rotationParent.transform.localScale = Vector3.one * Mathf.Lerp(ANIM_SIZE_MIN, ANIM_SIZE_MAX, animT);
        rotationParent.transform.localRotation = reversedAnim ? Quaternion.Euler(0, Mathf.Lerp(-ANIM_ROTATION, ANIM_ROTATION, animT), randomInclination) : Quaternion.Euler(0, Mathf.Lerp(ANIM_ROTATION, -ANIM_ROTATION, animT), randomInclination);
        mrColored.material.SetFloat("_AnimT", animT);
        mrFlash.material.SetFloat("_AnimT", animT);
    }
}
