using UnityEngine;

public class TechCombatEffectStatusTransfer : TechCombatEffect
{
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private ParticleSystem impactPS;
    [SerializeField] private ColorDatabaseSO cdb;
    [SerializeField] private Transform visualParent;

    private Vector3 spawnPos;
    private Vector3 targetPos;
    private EnemyEntity target;
    private int phase;
    private float phaseT;

    private const float SPAWN_HEIGHT = 0.5f;
    private const float FLY_HEIGHT = 3f;
    private const float SPAWN_DURATION = 0.5f;
    private const float CHASE_SPEED_GAIN_RATE = 4f;
    private const float REMOVE_DELAY = 1.5f;

    protected override void Initialize(Vector3 pos, float dir)
    {
        gameObject.SetActive(true);
        visualParent.gameObject.SetActive(true);
        ps.Play();
        phase = 0;
        phaseT = 0;
        spawnPos = new Vector3(pos.x, SPAWN_HEIGHT, pos.z);
        transform.position = spawnPos;
        targetPos = spawnPos + new Vector3(0, FLY_HEIGHT, 0);
        ParticleSystem.MainModule m = ps.main; m.startColor = cdb.ElementToColor(SetupData.element);
        ParticleSystem.MainModule m2 = impactPS.main; m2.startColor = m.startColor;
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
                    phaseT += Time.deltaTime / SPAWN_DURATION;
                    transform.position = Vector3.Lerp(spawnPos, targetPos, phaseT);
                    if (phaseT > 1) { phase = 1; phaseT = 0; FindNewTarget(); }
                    break;
                }
            case 1: 
                {
                    phaseT += Time.deltaTime * CHASE_SPEED_GAIN_RATE;
                    if (target != null) 
                    {
                        Vector3 targetPos = target.transform.position + new Vector3(0, FLY_HEIGHT / phaseT, 0);
                        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * phaseT);
                        if (transform.position == targetPos)
                        {
                            phase = 2;
                            phaseT = 0;
                            target.AddStatus(SetupData.element);
                            visualParent.gameObject.SetActive(false);
                            ps.Stop();
                            PlayExplosionPs();
                        }
                    }
                    
                    break;
                }
            case 2: 
                {
                    phaseT += Time.deltaTime / REMOVE_DELAY;
                    if (phaseT > 1) { gameObject.SetActive(false); }
                    break;
                }
        }
    }
    private void PlayExplosionPs() 
    {
        impactPS.Stop();
        impactPS.Play();
    }
    private void OnEnemyDisabled(EnemyEntity e, bool killcredit) 
    {
        if (phase != 1) { return; }
        if (e != target) { return; }

        FindNewTarget();
    }
    private void FindNewTarget() 
    {
        target = StageManagerBase.GetRandomEnemyInRange(SetupData.enemyIgnored, transform.position, SetupData.sizeMult);
        if (target == null) 
        {
            phase = 2; 
            phaseT = 0; 
            PlayExplosionPs();
            ps.Stop();
            visualParent.gameObject.SetActive(false);
        }
        else 
        {
            phase = 1; 
            phaseT = 0; 
        }
    }
}
