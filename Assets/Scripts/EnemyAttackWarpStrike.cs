using UnityEngine;

public class EnemyAttackWarpStrike : EnemyAttackBase
{
    [SerializeField] private CombatWarningCircle warning;
    [SerializeField] private Transform attackParent;
    [SerializeField] private Transform visualParent;
    [SerializeField] private MeshRenderer mr0;
    [SerializeField] private MeshRenderer mr1;
    [SerializeField] private MeshRenderer mr2;

    private float phaseT;
    private int phase;


    private const float VISUAL_HEIGHT_START = 8f;
    private const float VISUAL_HEIGHT_END = 0.1f;
    private const float VISUAL_WIDTH_START = 0.1f;
    private const float VISUAL_WIDTH_END = 1.2f;
    public const float WARNING_DURATION = 1.75f;
    public const float LINGER_DURATION = 0.5f;
    private const float WARNING_PHASE_FOLLOW_PLAYER_PERCENT = 0.4f;
    private const float REMOVE_DELAY = 1f;
    private const float BASE_DAMAGE = 75f;
    private const float SIZE = 12f;
    private const float SPAWN_HEIGHT = 0.1f;

    public override void Initialize(Vector3 pos, float dir)
    {
        attackParent.gameObject.SetActive(false);
        visualParent.gameObject.SetActive(false);
        gameObject.SetActive(true);
        warning.SetUp(transform.position, SIZE, WARNING_DURATION);

        attackParent.localScale = Vector3.one * SIZE;
        visualParent.localScale = Vector3.one * SIZE;
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
                        phase = 1;
                        attackParent.gameObject.SetActive(false);
                        visualParent.gameObject.SetActive(false);
                    }
                    break;
                }
            case 2: // RemoveDelay
                {
                    phaseT += Time.deltaTime / REMOVE_DELAY;
                    if (phaseT > 1) { gameObject.SetActive(false); }
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

            DamagePlayer(pe, BASE_DAMAGE);
        }
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
