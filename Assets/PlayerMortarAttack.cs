using UnityEngine;

public class PlayerMortarAttack : PlayerAttackBase
{
    [SerializeField] private Transform bounceVisualParent;

    private WeaponAttackSetupData setupData;
    private float bounceT;
    protected int bouncesRemaining;
    protected int piercesRemaining;
    protected float currentDirection;
    private LayerMask normalCheckRaycastMask;

    private const float BOUNCE_INITIAL_T = 0.25f;
    private const float BOUNCE_VISUAL_HEIGHT_MIN = -0.8f;
    private const float BOUNCE_VISUAL_HEIGHT_MAX = 2f;
    private const float BOUNCES_PER_SECOND = 1.25f;
    private const float NORMAL_CHECK_RAYCAST_LENGHT = 2f;
    private const float BASE_PROJECTILE_SPEED = 20f;


    public override void SetUp(Vector3 pos, float dir, WeaponAttackSetupData sd, PlayerAttackBase parentAttack)
    {
        setupData = sd;
        transform.position = pos;
        bouncesRemaining = sd.bounces;
        piercesRemaining = sd.pierces;
        SetNewDirection(dir);
        normalCheckRaycastMask = LayerMask.GetMask("Walls");
        bounceT = BOUNCE_INITIAL_T;
        UpdateBounceVisual();
        gameObject.gameObject.SetActive(true);
    }
    private void SetNewDirection(float newdir)
    {
        currentDirection = newdir;
        transform.rotation = Quaternion.Euler(0, currentDirection, 0);
    }
    private void FixedUpdate()
    {
        bounceT = Mathf.MoveTowards(bounceT, 1, BOUNCES_PER_SECOND * setupData.timescale * Time.fixedDeltaTime);
        UpdateBounceVisual();
        if (bounceT == 1) 
        {
            Explode();
            bouncesRemaining--;
            if (bouncesRemaining < 0) { gameObject.SetActive(false); }
            bounceT = 0;
        }
        transform.Translate(BASE_PROJECTILE_SPEED * Time.fixedDeltaTime * setupData.timescale * Vector3.forward);
    }
    private void UpdateBounceVisual() 
    {
        bounceVisualParent.transform.localPosition = new Vector3(0, BOUNCE_VISUAL_HEIGHT_MIN + Mathf.Sin(bounceT*180*Mathf.Deg2Rad) * BOUNCE_VISUAL_HEIGHT_MAX, 0);
    }
    private void Explode()
    {
        WeaponAttackSetupData sd = new WeaponAttackSetupData()
        {
            element = setupData.element,
            magnitude = setupData.magnitude,
            critchance = setupData.critchance,
            critdamage = setupData.critdamage,
            sizemult = setupData.splash,
            builduprate = setupData.builduprate,
        };
        StageManagerBase.GetObjectPool().GetPlayerAttackFromPool("EXPLOSION").SetUp(bounceVisualParent.position, 0, sd, this);
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            RaycastHit h;
            Physics.Raycast(transform.position, transform.forward, out h, NORMAL_CHECK_RAYCAST_LENGHT, normalCheckRaycastMask);

            if (h.collider != null)
            {
                SetNewDirection(GameTools.AngleReflection(currentDirection, GameTools.NormalToEuler(h.normal) + 90));
            }

        }

    }
}
