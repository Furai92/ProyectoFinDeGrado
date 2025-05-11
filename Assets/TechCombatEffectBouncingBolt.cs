using UnityEngine;

public class TechCombatEffectBouncingBolt : TechCombatEffect
{
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private ColorDatabaseSO cdb;

    private EnemyEntity target;
    private int phase;
    private float phaseT;
    private int bounces;

    private const float BOUNCES_MAX = 3;
    private const float SPAWN_HEIGHT = 0.5f;
    private const float CHASE_SPEED = 40f;
    private const float REMOVE_DELAY = 1.5f;

    protected override void Initialize(Vector3 pos, float dir)
    {
        tr.Clear();
        gameObject.SetActive(true);
        ps.Play();
        phase = 0;
        phaseT = 0;
        bounces = 0;
        transform.position = new Vector3(pos.x, SPAWN_HEIGHT, pos.z);
        ParticleSystem.MainModule m = ps.main; m.startColor = tr.startColor = tr.endColor = cdb.ElementToColor(SetupData.element);
        FindNewTarget();
    }
    private void OnEnable()
    {
        EventManager.EnemyDisabledEvent += OnEnemyDisabled;
    }
    private void OnDisable()
    {
        EventManager.EnemyDisabledEvent -= OnEnemyDisabled;
    }
    private void Update()
    {
        switch (phase)
        {
            case 0:
                {
                    if (target != null)
                    {
                        Vector3 currentPos = new Vector3(transform.position.x, SPAWN_HEIGHT, transform.position.z);
                        Vector3 targetPos = new Vector3(target.transform.position.x, SPAWN_HEIGHT, target.transform.position.z);
                        transform.position = Vector3.MoveTowards(currentPos, targetPos, Time.deltaTime * CHASE_SPEED);
                        if (transform.position == targetPos)
                        {
                            phase = 2;
                            phaseT = 0;
                            DamageEnemy(target);
                            bounces++;
                            if (bounces >= BOUNCES_MAX)
                            {
                                phase = 1; phaseT = 0;
                            }
                            else 
                            {
                                SetupData.enemyIgnored = target.EnemyInstanceID;
                                FindNewTarget();
                            }
                        }
                    }

                    break;
                }
            case 1:
                {
                    phaseT += Time.deltaTime / REMOVE_DELAY;
                    if (phaseT > 1) { gameObject.SetActive(false); }
                    break;
                }
        }
    }
    private void OnEnemyDisabled(EnemyEntity e, bool killcredit)
    {
        if (phase != 0) { return; }
        if (e != target) { return; }

        FindNewTarget();
    }
    private void FindNewTarget()
    {
        target = StageManagerBase.GetRandomEnemyInRange(SetupData.enemyIgnored, transform.position, SetupData.sizeMult);
        if (target == null)
        {
            phase = 1;
            phaseT = 0;
            ps.Stop();
        }
        else
        {
            phase = 0;
            phaseT = 0;
        }
    }
}
