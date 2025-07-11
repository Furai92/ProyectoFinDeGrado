using UnityEngine;
using System.Collections.Generic;

public abstract class PlayerProjectileBase : PlayerAttackBase
{
    [Header("Visual references")]
    [SerializeField] private Transform visual;
    [SerializeField] protected TrailRenderer tr;
    [SerializeField] protected ColorDatabaseSO cdb;
    [SerializeField] private ParticleSystem ps;

    [Space]
    [Header("Params")]
    [SerializeField] private bool EntityCollisions;
    [SerializeField] private bool UnlimitedWallBounces;
    [SerializeField] private bool ExplodeOnWallCollision;
    [SerializeField] private float MaxTravelDistance;

    private List<int> enemyHits;
    private int phase;
    private float distanceCovered;
    private float removeTime;
    public float CurrentDirection { get; protected set; }

    private const float MINIMUM_STEP_LENGHT = 0.2f; // This prevents an error where decimal rounding gets the bullet stuck with near zero distance steps 
    private const float BASE_SPEED = 20f;
    private const float BOUNCE_END_SEPARATION = 0.1f; // Helps improve bounces with walls
    private const float BASE_COLLISION_RADIUS = 0.5f;
    private const float REMOVE_DELAY = 0.5f;
    private const float BASE_TR_WIDTH = 0.15f;

    public abstract void OnBulletSpawn();
    public abstract void OnFixedUpdate();
    public abstract void OnBulletEnd();
    public abstract void OnTerrainCollision();
    public abstract void OnEntityCollision();

    public override void SetUp(Vector3 pos, float dir, WeaponAttackSetupData sd, PlayerAttackBase parentAttack)
    {

        enemyHits = new List<int>();
        setupData = sd;
        CurrentDirection = dir;
        transform.SetPositionAndRotation(pos, Quaternion.Euler(0, CurrentDirection, 0));
        phase = 0;
        distanceCovered = 0;
        tr.Clear();
        tr.startWidth = BASE_TR_WIDTH * setupData.SizeMultiplier;
        tr.endWidth = 0;
        tr.startColor = tr.endColor = cdb.ElementToColor(setupData.Element);
        tr.AddPosition(visual.position);
        if (ps != null) { ParticleSystem.MainModule m = ps.main; m.startColor = tr.startColor; }
        visual.gameObject.SetActive(true);
        gameObject.SetActive(true);
        OnBulletSpawn();
    }
    protected void SetRemoveable() 
    {
        distanceCovered = MaxTravelDistance;
    }
    private void FixedUpdate()
    {
        switch (phase) 
        {
            case 0: 
                {
                    if (MaxTravelDistance - distanceCovered > MINIMUM_STEP_LENGHT) 
                    {
                        CastBeam();
                        OnFixedUpdate();
                    }
                    else
                    {
                        phase = 1; removeTime = Time.time + REMOVE_DELAY;
                    }
                    break;
                }
            case 1: // Remove delay
                {
                    if (Time.time > removeTime) { OnBulletEnd(); gameObject.SetActive(false); }
                    break;
                }
        }
    }

    private void CastBeam()
    {
        float stepMaximumLenght = Mathf.Min(BASE_SPEED * setupData.Timescale * Time.fixedDeltaTime, MaxTravelDistance - distanceCovered);
        // Setup masks and hit buffers
        LayerMask terrainMask = LayerMask.GetMask("Walls");
        LayerMask hitboxMask = LayerMask.GetMask("EnemyHitboxes");
        RaycastHit terrainCastResults;
        RaycastHit entityCastResults;
        // Raycast ONLY WITH TERRAIN to create a path for the beam
        Physics.Raycast(transform.position, transform.forward, out terrainCastResults, stepMaximumLenght, terrainMask);

        // Spherecast ONLY WITH ENTITIES to check for damage
        bool beamInterruptedByEntity = false;
        Vector3 beamEnd = terrainCastResults.collider == null ? transform.position + transform.forward * stepMaximumLenght : terrainCastResults.point;
        Vector3 currentBeamPos = transform.position;
        float beamDiameter = BASE_COLLISION_RADIUS * setupData.SizeMultiplier;
        List<Collider> disabledColliders = new List<Collider>();
        while (!beamInterruptedByEntity && currentBeamPos != beamEnd && EntityCollisions)
        {
            Physics.SphereCast(currentBeamPos - (beamDiameter * 2 * transform.forward), beamDiameter * 0.5f, transform.forward, out entityCastResults, Vector3.Distance(currentBeamPos, beamEnd), hitboxMask);
            if (entityCastResults.collider != null)
            {
                // The spherecast found a hitbox on the way
                // Damage the entity hit by the beam
                EnemyEntity enemyHit = entityCastResults.collider.GetComponentInParent<EnemyEntity>();
                // Change the layer of the collider to ignore it on the next cast
                entityCastResults.collider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                disabledColliders.Add(entityCastResults.collider);
                currentBeamPos = transform.position + transform.forward * entityCastResults.distance;

                // If the enemy hit is supposed to be ignored, skip this part
                if (enemyHit.EnemyInstanceID != setupData.EnemyIgnored && !enemyHits.Contains(enemyHit.EnemyInstanceID))
                {
                    setupData.EnemyIgnored = enemyHit.EnemyInstanceID;
                    enemyHits.Add(enemyHit.EnemyInstanceID);
                    OnEntityCollision();
                    if (setupData.Splash > 0)
                    {
                        Explode(entityCastResults.point);
                    }
                    else
                    {
                        enemyHit.DealDirectDamage(setupData.Magnitude, setupData.CritChance, setupData.CritDamage, setupData.BuildupRate, setupData.Element, setupData.DamageType);
                        enemyHit.Knockback(setupData.Knockback, CurrentDirection);
                    }
                    PlayImpactEffects(entityCastResults.point, CurrentDirection);

                    // Spend pierces to continue
                    if (setupData.Pierces > 0)
                    {
                        setupData.Pierces--;
                    }
                    // If there's no pierces left, use bounces AND stop the beam
                    else if (setupData.Bounces > 0)
                    {
                        beamInterruptedByEntity = true;
                        beamEnd = currentBeamPos;
                        EventManager.OnProjectileBounce(this, true);
                        setupData.Bounces--;
                        CurrentDirection = GetBounceDirection(beamEnd, true, entityCastResults.collider);
                        transform.rotation = Quaternion.Euler(0, CurrentDirection, 0);
                        enemyHits.Clear();
                    }
                    else
                    {
                        beamInterruptedByEntity = true;
                        beamEnd = currentBeamPos;
                        // No bounces and pierces left
                        SetRemoveable();
                    }
                }
            }
            else
            {
                currentBeamPos = beamEnd;
            }

        }
        // If no entity collisions are found and terrain raycast found something... (the bullet hit a wall and nothing else)
        if (!beamInterruptedByEntity && terrainCastResults.collider != null)
        {
            // Move the end of the beam a small amount towards the start, this improves the bounces against straight walls
            beamEnd = Vector3.MoveTowards(beamEnd, transform.position, BOUNCE_END_SEPARATION); 

            tr.AddPosition(currentBeamPos);
            OnTerrainCollision();
            PlayImpactEffects(beamEnd, CurrentDirection);

            if (setupData.Splash > 0 && ExplodeOnWallCollision) { Explode(beamEnd); }
            if (setupData.Bounces > 0 || UnlimitedWallBounces)
            {
                if (!UnlimitedWallBounces) { setupData.Bounces--; }

                EventManager.OnProjectileBounce(this, false);
                CurrentDirection = GameTools.AngleReflection(CurrentDirection, GameTools.NormalToEuler(terrainCastResults.normal) + 90);
                transform.rotation = Quaternion.Euler(0, CurrentDirection, 0);
                enemyHits.Clear();
            }
            else 
            {
                distanceCovered = MaxTravelDistance; // This marks the attack as finished
            }
        }
        
        distanceCovered += Vector3.Distance(transform.position, beamEnd);
        transform.position = beamEnd;

        // Return the colliders to their layer
        for (int i = 0; i < disabledColliders.Count; i++)
        {
            disabledColliders[i].gameObject.layer = LayerMask.NameToLayer("EnemyHitboxes");
        }
    }
}
