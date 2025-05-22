using UnityEngine;

public class TechCombatEffectDashDamage : TechCombatEffect
{
    [SerializeField] private ParticleSystem PS0;
    [SerializeField] private ParticleSystem PS1;
    [SerializeField] private ColorDatabaseSO cdb;
    [SerializeField] private Transform visualRotationParent;

    private float phaseT;
    private int phase;
    private float direction;

    private const float VISUAL_ROTATION_SPEED = 500f;
    private const float REMOVE_DELAY = 1f;

    protected override void Initialize(Vector3 pos, float dir)
    {
        transform.position = pos;

        transform.localScale = Vector3.one * SetupData.sizeMult;
        gameObject.SetActive(true);
        ParticleSystem.MainModule m0 = PS0.main; m0.startColor = cdb.ElementToColor(SetupData.element);
        ParticleSystem.MainModule m1 = PS1.main; m1.startColor = cdb.ElementToColor(SetupData.element);
        phase = 0;
        phaseT = 0;
        UpdateVisualRotation();
        PS0.Play();
        PS1.Play();
    }
    private void FixedUpdate()
    {
        switch (phase) 
        {
            case 0: 
                {
                    phaseT += Time.fixedDeltaTime / PlayerEntity.DASH_DURATION;
                    UpdateVisualRotation();
                    if (phaseT > 1) { phase = 1; phaseT = 0; PS0.Stop(); PS1.Stop(); }
                    break;
                }
            case 1:
                {
                    phaseT += Time.fixedDeltaTime / REMOVE_DELAY;
                    if (phaseT > 1) { gameObject.SetActive(false); }
                    break;
                }
        }
        transform.position = PlayerEntity.ActiveInstance.transform.position;
    }
    private void UpdateVisualRotation() 
    {
        visualRotationParent.transform.localRotation = Quaternion.Euler(0, direction + phaseT * VISUAL_ROTATION_SPEED, 0);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (phase != 0) { return; }

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
