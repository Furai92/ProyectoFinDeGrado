using UnityEngine;

public class TechCombatEffectEnergyWave : TechCombatEffect
{
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private ColorDatabaseSO cdb;

    private LayerMask mask;
    private int phase;
    private float phaseT;

    private const float FLIGHT_DURATION = 10f;
    private const float SPAWN_GROWTH_DURATION = 0.5f;
    private const float REMOVE_DELAY = 2f;
    private const float RAYCAST_LEGHT_MULT = 2f;
    private const float MINIMUM_SCALE = 0.1f;
    private const float SPEED = 20f;
    private const float SPAWN_HEIGHT = 1f;

    protected override void Initialize(Vector3 pos, float dir)
    {
        gameObject.SetActive(true);
        transform.SetPositionAndRotation(new Vector3(pos.x, SPAWN_HEIGHT, pos.z), Quaternion.Euler(0, dir, 0));
        ParticleSystem.MainModule m = ps.main; m.startColor = cdb.ElementToColor(SetupData.element);
        mask = LayerMask.GetMask("Walls");
        phaseT = 0;
        phase = 0;
        ps.Play();
        UpdateScale(phaseT);
    }

    private void FixedUpdate()
    {
        switch (phase) 
        {
            case 0: 
                {
                    phaseT = Mathf.MoveTowards(phaseT, 1, Time.fixedDeltaTime / SPAWN_GROWTH_DURATION);
                    transform.Translate(SPEED * Time.fixedDeltaTime * Vector3.forward);
                    UpdateScale(phaseT);
                    CheckWalls();
                    if (phaseT >= 1) { phase = 1; phaseT = 0; }
                    break;
                }
            case 1:
                {
                    phaseT = Mathf.MoveTowards(phaseT, 1, Time.fixedDeltaTime / FLIGHT_DURATION);
                    transform.Translate(SPEED * Time.fixedDeltaTime * Vector3.forward);
                    CheckWalls();
                    if (phaseT >= 1) { phase = 2; phaseT = 0; ps.Stop(); }
                    break;
                }
            case 2: 
                {
                    phaseT = Mathf.MoveTowards(phaseT, 1, Time.fixedDeltaTime / REMOVE_DELAY);
                    UpdateScale(1-phaseT);
                    if (phaseT >= 1) { gameObject.SetActive(false); }
                    break;
                }
        }

    }
    private void CheckWalls() 
    {
        if (Physics.Raycast(transform.position, transform.forward, SPEED * RAYCAST_LEGHT_MULT * Time.fixedDeltaTime, mask))
        {
            phaseT = 0;
            phase = 2;
            ps.Stop();
        }
    }
    private void UpdateScale(float t) 
    {
        transform.localScale = Vector3.one * Mathf.Lerp(MINIMUM_SCALE, SetupData.sizeMult, t);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (phase > 1) { return; }

        if (collision.gameObject.CompareTag("EnemyHitbox"))
        {
            EnemyEntity e = collision.gameObject.GetComponentInParent<EnemyEntity>();
            if (e != null)
            {
                DamageEnemy(e);
            }
        }
    }
}
