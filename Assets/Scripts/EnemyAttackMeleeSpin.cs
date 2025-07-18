using UnityEngine;

public class EnemyAttackMeleeSpin : EnemyAttackBase
{
    [SerializeField] private Collider col;
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private ParticleSystem ps2;
    [SerializeField] private CombatWarningCircle warning;
    [SerializeField] private Transform attackParent;
    [SerializeField] private bool linkToUser;

    private float phaseT;
    private int phase;

    private const float WARNING_DURATION = 1.5f;
    private const float LINGER_DURATION = 0.5f;
    private const float REMOVE_DELAY = 1f;
    private const float BASE_DAMAGE = 75f;
    private const float SIZE = 10f;
    private const float SPAWN_HEIGHT = 0.5f;

    public override void Initialize(Vector3 pos, float dir)
    {
        gameObject.SetActive(true);
        col.enabled = false;
        attackParent.localScale = Vector3.one * SIZE;
        ps.Stop();
        ps2.Stop();
        phase = 0;
        phaseT = 0;
        UpdatePosition(pos);
        warning.SetUp(transform.position, SIZE, WARNING_DURATION);
    }

    private void FixedUpdate()
    {
        switch (phase) 
        {
            case 0: // Spawning with warning
                {
                    if (linkToUser) { UpdatePosition(User.transform.position); }
                    phaseT += Time.deltaTime / WARNING_DURATION;
                    warning.UpdatePosition(transform.position);
                    if (phaseT > 1) 
                    {
                        phaseT = 0; 
                        phase = 1;
                        col.enabled = true;
                        ps.Play();
                        ps2.Play();
                    }
                    break;
                }
            case 1: // Damage linger
                {
                    if (linkToUser) { UpdatePosition(User.transform.position); }
                    phaseT += Time.deltaTime / LINGER_DURATION;
                    if (phaseT > 1) 
                    {
                        phaseT = 0;
                        phase = 1;
                        col.enabled = false;
                        ps.Stop();
                        ps2.Stop();
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
