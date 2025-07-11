using UnityEngine;

public class EnemyAttackFireBomb : EnemyAttackBase
{
    [SerializeField] private Transform orbParent;
    [SerializeField] private Transform poolParent;
    [SerializeField] private Collider poolCollider;
    [SerializeField] private ParticleSystem ps;

    private float nextDamageTick;
    private Vector3 targetPos;
    private Vector3 startPos;
    private int phase;
    private float phaseT;
    private float randomizedYScale;

    private const float POOL_DAMAGE = 20f;
    private const float POOL_DAMAGE_TICKRATE = 0.5f;
    private const float ORB_FLIGHT_HEIGHT = 2f;
    private const float ORB_FLIGHT_DURATION = 2f;
    private const float POOL_GROWING_SIZE_MIN = 0.1f;
    private const float POOL_GROWING_SIZE_MAX = 15f;
    private const float POOL_GROWING_DURATION = 5f;
    private const float POOL_LINGER_TIME = 25f;
    private const float POOL_FADING_DURATION = 1f;
    private const float MIN_Y_SCALE = 0.95f;
    private const float MAX_Y_SCALE = 1.1f;

    public override void Initialize(Vector3 pos, float dir)
    {
        startPos = pos;
        Vector3 closestPlayerPos = PlayerEntity.ActiveInstance.transform.position;
        targetPos = new Vector3(closestPlayerPos.x, 0, closestPlayerPos.z);
        LayerMask m = LayerMask.GetMask("Walls");

        RaycastHit rh;
        Physics.Raycast(startPos, targetPos - startPos, out rh, Vector3.Distance(startPos, targetPos), m);
        if (rh.collider != null) { targetPos = new Vector3(rh.point.x, 0, rh.point.z); }

        randomizedYScale = Random.Range(MIN_Y_SCALE, MAX_Y_SCALE);
        phase = 0;
        phaseT = 0;
        ps.Stop();
        gameObject.SetActive(true);
        orbParent.gameObject.SetActive(true);
        poolParent.gameObject.SetActive(false);
        ObjectPoolManager.GetWarningCircleFromPool().SetUp(targetPos, POOL_GROWING_SIZE_MAX, ORB_FLIGHT_DURATION);
        orbParent.transform.localPosition = Vector3.zero;
    }
    private void FixedUpdate()
    {
        switch (phase) 
        {
            case 1:
            case 2:
            case 3: 
                {
                    if (Time.time > nextDamageTick) 
                    {
                        poolCollider.gameObject.SetActive(false);
                        poolCollider.gameObject.SetActive(true);
                        nextDamageTick = Time.time + POOL_DAMAGE_TICKRATE;
                    }
                    break;
                }
        }
    }
    private void Update()
    {
        switch (phase) 
        {
            case 0: // Orb flying to target pos
                {
                    phaseT += Time.deltaTime / ORB_FLIGHT_DURATION;
                    transform.position = Vector3.Lerp(startPos, targetPos, phaseT);
                    orbParent.transform.localPosition = Mathf.Sin(phaseT * 180 * Mathf.Deg2Rad) * ORB_FLIGHT_HEIGHT * Vector3.up;
                    if (phaseT > 1) 
                    {
                        phase = 1; phaseT = 0;
                        orbParent.gameObject.SetActive(false);
                        poolParent.gameObject.SetActive(true);
                        ps.Play();
                        poolCollider.gameObject.SetActive(true);
                        UpdatePoolScale(0);
                        EventManager.OnCombatWarningDisplayed(HudCombatWarningElement.WarningType.Normal, transform.position);
                    }
                    break;
                }
            case 1: // Pool growing
                {
                    phaseT += Time.deltaTime / POOL_GROWING_DURATION;
                    UpdatePoolScale(phaseT);
                    if (phaseT > 1)
                    {
                        nextDamageTick = Time.time + POOL_DAMAGE_TICKRATE;
                        phase = 2; phaseT = 0;
                        UpdatePoolScale(1);
                    }
                    break;
                }
            case 2: // Pool lingering
                {
                    phaseT += Time.deltaTime / POOL_LINGER_TIME;
                    if (phaseT > 1)
                    {
                        phase = 3; phaseT = 0;
                        ps.Stop();
                    }
                    break;
                }
            case 3: // Pool fading
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

            DamagePlayer(pe, POOL_DAMAGE);
        }
    }

    private void UpdatePoolScale(float t) 
    {
        float radius = Mathf.Lerp(POOL_GROWING_SIZE_MIN, POOL_GROWING_SIZE_MAX, t);
        poolParent.transform.localScale = new Vector3(radius, randomizedYScale, radius);
        ps.transform.localScale = Vector3.one * radius;
    }

    public override void OnUserDefeated()
    {

    }

    public override void OnUserStunned()
    {

    }

    public override void OnStageStateEnded()
    {
        gameObject.SetActive(false);
    }

}
