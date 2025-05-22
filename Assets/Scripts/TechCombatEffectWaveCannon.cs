using UnityEngine;

public class TechCombatEffectWaveCannon : TechCombatEffect
{
    [SerializeField] private ParticleSystem PS0;
    [SerializeField] private ParticleSystem PS1;
    [SerializeField] private ParticleSystem PS2;
    [SerializeField] private ColorDatabaseSO cdb;
    [SerializeField] private Transform beamParent;

    private float phaseT;
    private int phase;

    private const float BEAM_MIN_SCALE = 0.1f;
    private const float SPAWN_DURATION = 0.25f;
    private const float DAMAGE_LINGER = 0.5f;
    private const float FADE_DURATION = 0.25f;
    private const float REMOVE_DELAY = 0.5f;
    private const float BEAM_MAX_LENGHT = 60f;
    private const float SPAWN_HEIGHT = 1f;

    protected override void Initialize(Vector3 pos, float dir)
    {
        gameObject.SetActive(true);
        transform.SetPositionAndRotation(new Vector3(pos.x, SPAWN_HEIGHT, pos.z), Quaternion.Euler(0, dir, 0));

        ParticleSystem.MainModule m0 = PS0.main; m0.startColor = cdb.ElementToColor(SetupData.element);
        ParticleSystem.MainModule m1 = PS1.main; m1.startColor = cdb.ElementToColor(SetupData.element);
        ParticleSystem.MainModule m2 = PS2.main; m2.startColor = cdb.ElementToColor(SetupData.element);

        PS0.transform.localScale = PS1.transform.localScale = PS2.transform.localScale = Vector3.one * SetupData.sizeMult;

        PS0.Stop(); PS0.Play();
        PS1.Stop(); PS1.Play();
        PS2.Stop(); PS2.Play();

        phase = 0;
        phaseT = 0;

        CastBeam();
        beamParent.gameObject.SetActive(true);
        UpdateBeamGrowthAnim(0);
    }
    private void FixedUpdate()
    {
        switch (phase) 
        {
            case 0: // Spawning
                {
                    phaseT += Time.fixedDeltaTime / SPAWN_DURATION;
                    UpdateBeamGrowthAnim(phaseT);
                    if (phaseT > 1) { phaseT = 0; phase = 1; }
                    break;
                }
            case 1: // Linger
                {
                    phaseT += Time.fixedDeltaTime / DAMAGE_LINGER;
                    if (phaseT > 1) { phaseT = 0; phase = 2; }
                    break;
                }
            case 2: // Fading
                {
                    phaseT += Time.fixedDeltaTime / FADE_DURATION;
                    UpdateBeamGrowthAnim(1-phaseT);
                    if (phaseT > 1) { phaseT = 0; phase = 3; beamParent.gameObject.SetActive(false); }
                    break;
                }
            case 3: // Remove Delay
                {
                    phaseT += Time.fixedDeltaTime / REMOVE_DELAY;
                    if (phaseT > 1) { gameObject.SetActive(false); }
                    break;
                }
        }
    }
    private void UpdateBeamGrowthAnim(float t) 
    {
        float width = Mathf.Lerp(BEAM_MIN_SCALE, 1, t) * SetupData.sizeMult;
        beamParent.transform.localScale = new Vector3(width, width, beamParent.transform.localScale.z);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("EnemyHitbox"))
        {
            EnemyEntity e = collision.gameObject.GetComponentInParent<EnemyEntity>();
            if (e != null)
            {
                DamageEnemy(e);
            }
        }
    }
    private void CastBeam()
    {
        LayerMask m = LayerMask.GetMask("Walls");
        RaycastHit rh;

        Physics.Raycast(transform.position, transform.forward, out rh, BEAM_MAX_LENGHT, m);
        beamParent.localScale = new Vector3(SetupData.sizeMult, SetupData.sizeMult, rh.collider == null ? BEAM_MAX_LENGHT : Vector3.Distance(transform.position, rh.point));
    }

}
