using UnityEngine;
using System.Collections.Generic;

public class PlayerAttackFastProjectile : PlayerAttackBase
{
    [SerializeField] private Transform visual;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private ColorDatabaseSO cdb;

    private List<int> enemyHits;
    private int phase;
    private float distanceCovered;
    private float removeTime;
    private float direction;

    private const float MINIMUM_STEP_LENGHT = 0.2f; // This prevents an error where decimal rounding gets the bullet stuck with near zero distance steps 
    private const float BASE_SPEED = 10f;
    private const float BOUNCE_END_SEPARATION = 0.1f; // Helps improve bounces with walls
    private const float CAST_RADIUS = 0.25f;
    private const float REMOVE_DELAY = 0.3f;
    private const float MAX_TRAVEL_DISTANCE = 50f;

    public override void SetUp(Vector3 pos, float dir, WeaponAttackSetupData sd, PlayerAttackBase parentAttack)
    {

        enemyHits = new List<int>();
        setupData = sd;
        direction = dir;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, direction, 0);
        phase = 0;
        distanceCovered = 0;
        tr.Clear();
        tr.startColor = tr.endColor = cdb.ElementToColor(setupData.Element);
        tr.AddPosition(transform.position);
        visual.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }
    private void FixedUpdate()
    {
        switch (phase) 
        {
            case 0: 
                {
                    if (MAX_TRAVEL_DISTANCE - distanceCovered > MINIMUM_STEP_LENGHT) 
                    {
                        CastBeam();
                    }
                    else
                    {
                        phase = 1; removeTime = Time.time + REMOVE_DELAY;
                    }
                    break;
                }
            case 1: // Remove delay
                {
                    if (Time.time > removeTime) { gameObject.SetActive(false); }
                    break;
                }
        }
    }
    private void CastBeam()
    {
        float stepMaximumLenght = Mathf.Min(BASE_SPEED * setupData.Timescale, MAX_TRAVEL_DISTANCE - distanceCovered);
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
        float beamDiameter = CAST_RADIUS * setupData.SizeMultiplier;
        List<Collider> disabledColliders = new List<Collider>();
        while (!beamInterruptedByEntity && currentBeamPos != beamEnd)
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
                    if (setupData.Splash > 0)
                    {
                        Explode(entityCastResults.point);
                    }
                    else
                    {
                        enemyHit.DealDirectDamage(setupData.Magnitude, setupData.CritChance, setupData.CritDamage, setupData.BuildupRate, setupData.Element);
                        enemyHit.Knockback(setupData.Knockback, direction);
                    }

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
                        if (setupData.Bounces > 0)
                        {
                            setupData.Bounces--;
                            tr.AddPosition(currentBeamPos);
                            direction = GetBounceDirection(beamEnd, true, entityCastResults.collider);
                            transform.rotation = Quaternion.Euler(0, direction, 0);
                            enemyHits.Clear();
                        }
                    }
                    else
                    {
                        beamInterruptedByEntity = true;
                        beamEnd = currentBeamPos;
                        // No bounces and pierces left
                        distanceCovered = MAX_TRAVEL_DISTANCE; // This marks the attack as finished
                    }
                }

            }
            else
            {
                currentBeamPos = beamEnd;
            }

        }
        // If nothing stopped the entity spherecast and the terrain raycast found something... (the bullet hit a wall and nothing else)
        if (!beamInterruptedByEntity && terrainCastResults.collider != null)
        {
            if (setupData.Splash > 0) { Explode(beamEnd); }
            if (setupData.Bounces > 0)
            {
                setupData.Bounces--;
                tr.AddPosition(currentBeamPos);
                direction = GameTools.AngleReflection(direction, GameTools.NormalToEuler(terrainCastResults.normal) + 90);
                transform.rotation = Quaternion.Euler(0, direction, 0);
                enemyHits.Clear();
            }
            else 
            {
                distanceCovered = MAX_TRAVEL_DISTANCE; // This marks the attack as finished
            }
        }
        
        distanceCovered += Vector3.Distance(transform.position, beamEnd);
        beamEnd = Vector3.MoveTowards(beamEnd, transform.position, BOUNCE_END_SEPARATION); // Move the end of the beam a small amount towards the start, this improves the bounces against straight walls
        transform.position = beamEnd;

        // Return the colliders to their layer
        for (int i = 0; i < disabledColliders.Count; i++)
        {
            disabledColliders[i].gameObject.layer = LayerMask.NameToLayer("EnemyHitboxes");
        }
    }
}
