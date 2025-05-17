using UnityEngine;

public class TechCombatEffectLandmine : TechCombatEffect
{
    [SerializeField] private ParticleSystem chargingPS;
    [SerializeField] private ParticleSystem activePS;
    [SerializeField] private ColorDatabaseSO cdb;

    private int phase;
    private float phaseT;


    private const float CHARGE_DURATION = 3f;
    private const float LIFETIME = 30f;
    private const float REMOVE_DELAY = 2f;
    private const float SPAWN_HEIGHT = 0.2f;

    protected override void Initialize(Vector3 pos, float dir)
    {
        transform.position = new Vector3(pos.x, SPAWN_HEIGHT, pos.z);
        gameObject.SetActive(true);
        phase = 0;
        phaseT = 0;
        ParticleSystem.MainModule m0 = activePS.main; m0.startColor = cdb.ElementToColor(SetupData.element);
        ParticleSystem.MainModule m1 = chargingPS.main; m1.startColor = cdb.ElementToColor(SetupData.element);
        chargingPS.Play();
        activePS.Stop();
        EventManager.StageStateEndedEvent += OnStageStateEnded;
    }
    private void OnDisable()
    {
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
    }
    private void OnStageStateEnded(StageStateBase.GameState s) 
    {
        gameObject.SetActive(false);
    }
    private void Update()
    {

        switch (phase) 
        {
            case 0:
                {
                    phaseT += Time.deltaTime / CHARGE_DURATION;
                    if (phaseT >= 1)
                    {
                        phaseT = 0;
                        phase = 1; 
                        chargingPS.Stop();
                        activePS.Play();
                    }
                    break;
                }
            case 1: 
                {
                    phaseT += Time.deltaTime / LIFETIME;
                    if (phaseT >= 1)
                    {
                        phaseT = 0; 
                        phase = 2;
                        activePS.Stop(); 
                    }
                    break;
                }
            case 2: 
                {
                    phaseT += Time.deltaTime / REMOVE_DELAY;
                    if (phaseT >= 1) { gameObject.SetActive(false); }
                    break;
                }
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (phase != 1) { return; }

        if (collision.gameObject.CompareTag("EnemyHitbox"))
        {
            activePS.Stop();
            phase = 2;
            phaseT = 0;

            TechCombatEffectSetupData sd = new TechCombatEffectSetupData() 
            {
                sizeMult = SetupData.sizeMult,
                element = SetupData.element,
                enemyIgnored = -1,
                magnitude = SetupData.magnitude
            };

            ObjectPoolManager.GetTechCombatEffectFromPool("EXPLOSION").SetUp(transform.position, 0, sd);
        }
    }
}
