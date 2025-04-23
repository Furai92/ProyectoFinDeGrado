using UnityEngine;

public class EnemyAttackFireBomb : EnemyAttackBase
{
    [SerializeField] private Transform orbParent;
    [SerializeField] private Transform poolParent;
    [SerializeField] private Collider poolCollider;

    private float nextDamageTick;
    private Vector3 targetPos;
    private Vector3 startPos;
    private int phase;
    private float phaseT;

    private const float POOL_DAMAGE = 20f;
    private const float POOL_DAMAGE_TICKRATE = 0.5f;
    private const float ORB_FLIGHT_HEIGHT = 2f;
    private const float ORB_FLIGHT_DURATION = 2f;
    private const float POOL_GROWING_SIZE_MIN = 0.1f;
    private const float POOL_GROWING_SIZE_MAX = 3f;
    private const float POOL_GROWING_DURATION = 5f;
    private const float POOL_LINGER_TIME = 15f;
    private const float POOL_FADING_DURATION = 1f;

    public override void Initialize(Vector3 pos, float dir)
    {
        startPos = pos;
        Vector3 closestPlayerPos = PlayerEntity.ActiveInstance.transform.position;
        targetPos = new Vector3(closestPlayerPos.x, 0, closestPlayerPos.z);
        LayerMask m = LayerMask.GetMask("Walls");

        RaycastHit rh;
        Physics.Raycast(startPos, targetPos - startPos, out rh, Vector3.Distance(startPos, targetPos), m);
        if (rh.collider != null) { targetPos = new Vector3(rh.point.x, 0, rh.point.z); }

        phase = 0;
        phaseT = 0;
        gameObject.SetActive(true);
        orbParent.gameObject.SetActive(true);
        poolParent.gameObject.SetActive(false);
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
                        poolCollider.gameObject.SetActive(true);
                        poolParent.transform.localScale = Vector3.one * POOL_GROWING_SIZE_MIN;
                        EventManager.OnCombatWarningDisplayed(HudCombatWarningElement.WarningType.Normal, transform.position);
                    }
                    break;
                }
            case 1: // Pool growing
                {
                    phaseT += Time.deltaTime / POOL_GROWING_DURATION;
                    poolParent.transform.localScale = Vector3.one * Mathf.Lerp(POOL_GROWING_SIZE_MIN, POOL_GROWING_SIZE_MAX, phaseT);
                    if (phaseT > 1)
                    {
                        nextDamageTick = Time.time + POOL_DAMAGE_TICKRATE;
                        phase = 2; phaseT = 0;
                        poolParent.transform.localScale = Vector3.one * POOL_GROWING_SIZE_MAX;
                    }
                    break;
                }
            case 2: // Pool lingering
                {
                    phaseT += Time.deltaTime / POOL_LINGER_TIME;
                    if (phaseT > 1)
                    {
                        phase = 3; phaseT = 0;
                    }
                    break;
                }
            case 3: // Pool fading
                {
                    phaseT += Time.deltaTime / POOL_FADING_DURATION;
                    poolParent.transform.localScale = Vector3.one * Mathf.Lerp(POOL_GROWING_SIZE_MAX, POOL_GROWING_SIZE_MIN, phaseT);
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
            if (!pe.IsEvading())
            {
                pe.DealDamage(POOL_DAMAGE);
            }
            else
            {
                EventManager.OnPlayerEvasion(transform.position);
            }
        }
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
