using UnityEngine;

public class DeflectedAttack : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform visualParent;

    private float phaseT;
    private int phase;

    private float magnitude;
    private LayerMask terrainCollisionMask;

    private const float TRACKING_RANGE = 40f;
    private const float TERRAIN_COLLISION_RAYCAST_LENGHT = 1f;
    private const float SPEED = 45f;
    private const float LIFETIME = 10f;
    private const float REMOVE_DELAY = 2f;

    public void SetUp(float mag, Vector3 pos)
    {
        transform.position = pos;
        magnitude = mag;
        EnemyEntity closestEnemy = StageManagerBase.GetClosestEnemyInRange(null, transform.position, TRACKING_RANGE);
        float direction = closestEnemy == null ? Random.Range(0, 361) : GameTools.AngleBetween(transform.position, closestEnemy.transform.position);
        transform.rotation = Quaternion.Euler(0, direction, 0);
        terrainCollisionMask = LayerMask.GetMask("Walls");
        phase = 0;
        phaseT = 0;

        tr.Clear();
        gameObject.SetActive(true);
        visualParent.gameObject.SetActive(true);
        rb.WakeUp();
        ps.Stop();
        ps.Play();
    }
    private void FixedUpdate()
    {
        switch (phase) 
        {
            case 0: 
                {
                    transform.Translate(SPEED * Time.fixedDeltaTime * Vector3.forward);
                    phaseT += Time.fixedDeltaTime / LIFETIME;
                    if (phaseT > 1) { gameObject.SetActive(false); } // Timeout
                    break;
                }
            case 1: 
                {
                    phaseT += Time.fixedDeltaTime / REMOVE_DELAY;
                    if (phaseT > 1) { gameObject.SetActive(false); } // Timeout
                    break;
                }
        }
        
    }
    private void SetAsRemoveable() 
    {
        phase = 1;
        phaseT = 0;
        rb.Sleep();
        visualParent.gameObject.SetActive(false);
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (Physics.Raycast(transform.position, transform.forward, TERRAIN_COLLISION_RAYCAST_LENGHT, terrainCollisionMask))
            {
                ps.Stop();
                ps.Play();
                SetAsRemoveable();
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "EnemyHitbox":
                {
                    EnemyEntity e = collision.gameObject.GetComponentInParent<EnemyEntity>();
                    if (e != null) { e.DealDirectDamage(magnitude, 0, 2, 0, GameEnums.DamageElement.Physical); }

                    ps.Stop();
                    ps.Play();
                    SetAsRemoveable();
                    break;
                }
        }
    }
}
