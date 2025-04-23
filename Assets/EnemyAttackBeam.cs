using UnityEngine;

public class EnemyAttackBeam : EnemyAttackBase
{
    [SerializeField] private Transform warningParent;
    [SerializeField] private Transform damagingParent;

    private float phaseT;
    private int phase;
    private int warningTicks;

    private const float DAMAGE = 100f;
    private const float WARNING_WIDTH_MIN = 0.1f;
    private const float WARNING_WIDTH_MAX = 1.5f;
    private const float WARNING_SPAWN_DURATION = 0.15f;
    private const int WARNING_TICKS_MAX = 6;
    private const float WARNING_TICK_INTERVAL = 0.25f;
    private const float DAMAGING_COMPONENT_GROWTH_DURATION = 0.25f;
    private const float DAMAGING_COMPONENT_LINGER = 0.5f;
    private const float DAMAGING_COMPONENT_FADE_DURATION = 0.25f;
    private const float DAMAGING_COMPONENT_WIDTH_MIN = 0.1f;
    private const float DAMAGING_COMPONENT_WIDTH_MAX = 8f;
    private const float BEAM_MAX_LENGHT = 50f;

    public override void Initialize(Vector3 pos, float dir)
    {
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, dir, 0);
        warningTicks = 0;
        phaseT = 0;
        phase = 0;
        gameObject.SetActive(true);
        warningParent.gameObject.SetActive(true);
        damagingParent.gameObject.SetActive(false);
        warningParent.transform.localScale = new Vector3(WARNING_WIDTH_MIN, WARNING_WIDTH_MIN, 1);
        CastBeam();
    }
    private void CastBeam() 
    {
        LayerMask m = LayerMask.GetMask("Walls");
        RaycastHit rh;

        Physics.Raycast(transform.position, transform.forward, out rh, BEAM_MAX_LENGHT, m);
        transform.localScale = new Vector3(1, 1, rh.collider == null ? BEAM_MAX_LENGHT : Vector3.Distance(transform.position, rh.point));
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerEntity pe = collision.gameObject.GetComponentInParent<PlayerEntity>();
            if (pe == null) { return; }
            if (!pe.IsEvading())
            {
                pe.DealDamage(DAMAGE);
            }
            else
            {
                EventManager.OnPlayerEvasion(transform.position);
            }
        }
    }
    private void FixedUpdate()
    {
        switch (phase) 
        {
            case 0: // Warning Spawning
                {
                    phaseT += Time.fixedDeltaTime / WARNING_SPAWN_DURATION;
                    transform.position = User.transform.position;
                    float width = Mathf.Lerp(WARNING_WIDTH_MIN, WARNING_WIDTH_MAX, phaseT);
                    warningParent.transform.localScale = new Vector3(width, width, 1);
                    if (phaseT > 1) { phase = 1; phaseT = 0; }
                    break;
                }
            case 1: // Warning Linger
                {
                    transform.position = User.transform.position;
                    phaseT += Time.fixedDeltaTime / WARNING_TICK_INTERVAL;
                    if (phaseT > 1) 
                    {
                        warningTicks++;
                        phaseT = 0;
                        CastBeam();
                        if (warningTicks >= WARNING_TICKS_MAX) 
                        {
                            phase = 2;
                            damagingParent.gameObject.SetActive(true);
                            damagingParent.transform.localScale = new Vector3(DAMAGING_COMPONENT_WIDTH_MIN, DAMAGING_COMPONENT_WIDTH_MIN, 1);
                        }
                    }
                    break;
                }
            case 2: // Attack Spawning
                {
                    phaseT += Time.fixedDeltaTime / DAMAGING_COMPONENT_GROWTH_DURATION;
                    float warning_w = Mathf.Lerp(WARNING_WIDTH_MIN, WARNING_WIDTH_MAX, 1-phaseT);
                    float attack_w = Mathf.Lerp(DAMAGING_COMPONENT_WIDTH_MIN, DAMAGING_COMPONENT_WIDTH_MAX, phaseT);
                    warningParent.transform.localScale = new Vector3(warning_w, warning_w, 1);
                    damagingParent.transform.localScale = new Vector3(attack_w, attack_w, 1);
                    if (phaseT > 1) 
                    {
                        phase = 3; phaseT = 0;
                        warningParent.gameObject.SetActive(false);
                    }
                    break;
                }
            case 3: // Attack Linger
                {
                    phaseT += Time.fixedDeltaTime / DAMAGING_COMPONENT_LINGER;
                    if (phaseT > 1) { phase = 4; phaseT = 0; }
                    break;
                }
            case 4: // Attack Fading
                {
                    phaseT += Time.fixedDeltaTime / DAMAGING_COMPONENT_FADE_DURATION;
                    float attack_w = Mathf.Lerp(DAMAGING_COMPONENT_WIDTH_MIN, DAMAGING_COMPONENT_WIDTH_MAX, 1-phaseT);
                    damagingParent.transform.localScale = new Vector3(attack_w, attack_w, 1);
                    if (phaseT > 1) { gameObject.SetActive(false); }
                    break;
                }
        }
    }

    public override void OnUserDefeated()
    {
        if (phase < 2) { gameObject.SetActive(false); }
    }

    public override void OnUserStunned()
    {
        if (phase < 2) { gameObject.SetActive(false); }
    }

    public override void OnStageStateEnded()
    {
        gameObject.SetActive(false);
    }
}
