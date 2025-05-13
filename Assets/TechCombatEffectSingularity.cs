using UnityEngine;

public class TechCombatEffectSingularity : TechCombatEffect
{
    [SerializeField] private ParticleSystem PS0;
    [SerializeField] private ParticleSystem PS1;
    [SerializeField] private Transform visualParent;
    [SerializeField] private Collider col;

    private float phaseT;
    private int phase;
    private int pulses;

    private const float PULSE_INTERVAL = 0.5f;
    private const int PULSE_COUNT = 20;
    private const float SPAWN_DURATION = 0.25f;
    private const float FADE_DURATION = 0.25f;
    private const float REMOVE_DELAY = 0.5f;
    private const float ANIM_MIN_SCALE = 0.1f;
    private const float PULL_STRENGHT = 15f;
    private const float SPAWN_HEIGHT = 0.5f;

    protected override void Initialize(Vector3 pos, float dir)
    {
        transform.position = new Vector3(pos.x, SPAWN_HEIGHT, pos.z);
        transform.localScale = Vector3.one * SetupData.sizeMult;
        gameObject.SetActive(true);

        col.enabled = false;
        pulses = 0;
        phaseT = 0;
        phase = 0;
        PS0.Play();
        PS1.Play();
        UpdateScaleAnimation(0);
    }
    private void FixedUpdate()
    {
        switch (phase) 
        {
            case 0: // Spawning
                {
                    phaseT += Time.fixedDeltaTime / SPAWN_DURATION;
                    UpdateScaleAnimation(phaseT);
                    if (phaseT >= 1) { phase = 1; phaseT = 0; col.enabled = true; }
                    break;
                }
            case 1: // Pulsing
                {
                    phaseT += Time.fixedDeltaTime / PULSE_INTERVAL;
                    if (phaseT >= 1) 
                    {
                        col.enabled = false;
                        pulses++;
                        phaseT = 0;
                        if (pulses >= PULSE_COUNT)
                        {
                            phase = 2;
                            PS1.Stop();
                            PS0.Stop();
                        }
                        else 
                        {
                            col.enabled = true;
                        }

                    }
                    break;
                }
            case 2: // Fading
                {
                    phaseT += Time.fixedDeltaTime / FADE_DURATION;
                    if (phaseT >= 1) { phase = 3; phaseT = 0; }
                    break;
                }
            case 3: // Remove delay
                {
                    phaseT += Time.fixedDeltaTime / REMOVE_DELAY;
                    if (phaseT >= 1) { gameObject.SetActive(false); }
                    break;
                }
        }
    }
    private void UpdateScaleAnimation(float t) 
    {
        visualParent.transform.localScale = Vector3.one * Mathf.Lerp(ANIM_MIN_SCALE, 1, t);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (phase != 1) { return; }

        if (collision.gameObject.CompareTag("EnemyHitbox"))
        {
            EnemyEntity e = collision.gameObject.GetComponentInParent<EnemyEntity>();
            if (e != null)
            {
                e.DealDirectDamage(SetupData.magnitude / PULSE_COUNT, 0, 1, GameTools.IntellectToBuildupMultiplier(PlayerEntity.ActiveInstance.GetStat(PlayerStatGroup.Stat.Intellect)), 
                    GameEnums.DamageElement.Void, GameEnums.DamageType.Tech);
                e.AddPullForce(PULL_STRENGHT, transform.position, PULSE_INTERVAL);
            }
        }
    }
}

