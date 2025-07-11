using UnityEngine;

public class EnemyAttackFireWarpStrike : EnemyAttackBase
{
    [SerializeField] private CombatWarningCircle warning;
    [SerializeField] private Transform attackParent;
    [SerializeField] private Transform visualParent;
    [SerializeField] private Transform poolParent;
    [SerializeField] private ParticleSystem landingPS;
    [SerializeField] private ParticleSystem poolPS;
    [SerializeField] private MeshRenderer mr0;
    [SerializeField] private MeshRenderer mr1;
    [SerializeField] private MeshRenderer mr2;
    [SerializeField] private Collider poolCollider;

    private float phaseT;
    private int phase;
    private float nextPoolDamageTick;
    private float randomizedPoolYScale;

    private const float POOL_DAMAGE = 25f;
    private const float POOL_DAMAGE_TICKRATE = 0.5f;
    private const float POOL_GROWING_SIZE_MIN = 0.1f;
    private const float POOL_GROWING_SIZE_MAX = 50f;
    private const float POOL_GROWING_DURATION = 10f;
    private const float POOL_LINGER_TIME = 45f;
    private const float POOL_FADING_DURATION = 1f;
    private const float VISUAL_HEIGHT_START = 8f;
    private const float VISUAL_HEIGHT_END = 0.1f;
    private const float VISUAL_WIDTH_START = 0.1f;
    private const float VISUAL_WIDTH_END = 1.2f;
    public const float WARNING_DURATION = 1.75f;
    public const float LINGER_DURATION = 0.5f;
    private const float WARNING_PHASE_FOLLOW_PLAYER_PERCENT = 0.4f;
    private const float WARP_DAMAGE = 75f;
    private const float SIZE = 12f;
    private const float SPAWN_HEIGHT = 0.1f;
    private const float MIN_Y_SCALE = 0.95f;
    private const float MAX_Y_SCALE = 1.1f;


    public override void Initialize(Vector3 pos, float dir)
    {
        attackParent.gameObject.SetActive(false);
        visualParent.gameObject.SetActive(false);
        poolParent.gameObject.SetActive(false);
        poolPS.gameObject.SetActive(false);
        gameObject.SetActive(true);
        warning.SetUp(transform.position, SIZE, WARNING_DURATION);
        randomizedPoolYScale = Random.Range(MIN_Y_SCALE, MAX_Y_SCALE);
        attackParent.localScale = Vector3.one * SIZE;
        visualParent.localScale = Vector3.one * SIZE;
        landingPS.transform.localScale = Vector3.one * SIZE;
        phase = 0;
        phaseT = 0;
        UpdatePosition(PlayerEntity.ActiveInstance.transform.position);
    }

    private void FixedUpdate()
    {
        switch (phase)
        {
            case 0: // Spawning with warning
                {
                    if (phaseT < WARNING_PHASE_FOLLOW_PLAYER_PERCENT)
                    {
                        UpdatePosition(PlayerEntity.ActiveInstance.transform.position);
                    }
                    phaseT += Time.deltaTime / WARNING_DURATION;
                    if (phaseT > 1)
                    {
                        phaseT = 0;
                        phase = 1;
                        attackParent.gameObject.SetActive(true);
                        visualParent.gameObject.SetActive(true);
                        User.Teleport(transform.position);
                        landingPS.Stop();
                        landingPS.Play();
                        UpdateAnimation(0);
                    }
                    break;
                }
            case 1: // Damage linger
                {
                    phaseT += Time.deltaTime / LINGER_DURATION;
                    UpdateAnimation(phaseT);
                    if (phaseT > 1)
                    {
                        phaseT = 0;
                        phase = 2;
                        attackParent.gameObject.SetActive(false);
                        visualParent.gameObject.SetActive(false);
                        poolParent.gameObject.SetActive(true);
                        poolPS.gameObject.SetActive(true);
                    }
                    break;
                }
            case 2: // Pool growing
                {
                    if (Time.time > nextPoolDamageTick)
                    {
                        poolCollider.gameObject.SetActive(false);
                        poolCollider.gameObject.SetActive(true);
                        nextPoolDamageTick = Time.time + POOL_DAMAGE_TICKRATE;
                    }
                    phaseT += Time.deltaTime / POOL_GROWING_DURATION;
                    UpdatePoolScale(phaseT);
                    if (phaseT > 1)
                    {
                        nextPoolDamageTick = Time.time + POOL_DAMAGE_TICKRATE;
                        phase = 3; phaseT = 0;
                        UpdatePoolScale(1);
                    }
                    break;
                }
            case 3: // Pool lingering
                {
                    phaseT += Time.deltaTime / POOL_LINGER_TIME;
                    if (phaseT > 1)
                    {
                        phase = 4; phaseT = 0;
                        poolPS.Stop();
                    }
                    break;
                }
            case 4: // Pool fading
                {
                    phaseT += Time.deltaTime / POOL_FADING_DURATION;
                    UpdatePoolScale(1 - phaseT);
                    if (phaseT > 1)
                    {
                        gameObject.SetActive(false);
                    }
                    break;
                }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerEntity pe = collision.gameObject.GetComponentInParent<PlayerEntity>();
            if (pe == null) { return; }

            DamagePlayer(pe, phase == 1 ? WARP_DAMAGE : POOL_DAMAGE);
        }
    }
    private void UpdatePoolScale(float t)
    {
        float radius = Mathf.Lerp(POOL_GROWING_SIZE_MIN, POOL_GROWING_SIZE_MAX, t);
        poolParent.transform.localScale = new Vector3(radius, randomizedPoolYScale, radius);
        poolPS.transform.localScale = Vector3.one * radius;
    }
    private void UpdatePosition(Vector3 p)
    {
        transform.position = new Vector3(p.x, SPAWN_HEIGHT, p.z);
    }
    private void UpdateAnimation(float t)
    {
        mr0.material.SetFloat("_Alpha", 1 - t);
        mr1.material.SetFloat("_Alpha", 1 - t);
        mr2.material.SetFloat("_Alpha", 1 - t);

        float vheight = Mathf.Lerp(VISUAL_HEIGHT_START, VISUAL_HEIGHT_END, t);
        float vwidth = Mathf.Lerp(VISUAL_WIDTH_START, VISUAL_WIDTH_END, t);

        visualParent.transform.localScale = new Vector3(vwidth, vheight, vwidth) * SIZE;
    }

    public override void OnUserDefeated()
    {
        gameObject.SetActive(false);
    }

    public override void OnUserStunned()
    {
        gameObject.SetActive(false);
    }

    public override void OnStageStateEnded()
    {
        gameObject.SetActive(false);
    }
}
