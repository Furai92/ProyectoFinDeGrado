using UnityEngine;
using System.Collections.Generic;

public class PlayerAttackFastProjectile : PlayerAttackBase
{
    [SerializeField] private Transform visual;

    private List<int> enemyHits;
    private int phase;
    private float phaseT;
    private float direction;

    private const float BASE_STEP_DISTANCE = 10f;
    private const float BOUNCE_END_SEPARATION = 0.1f; // Helps improve bounces with walls
    private const float CAST_RADIUS = 0.25f;
    private const float REMOVE_DELAY = 0.3f;
    private const float TRAVEL_DISTANCE = 50f;

    public override void SetUp(Vector3 pos, float dir, WeaponAttackSetupData sd, PlayerAttackBase parentAttack)
    {
        enemyHits = new List<int>();
        setupData = sd;
        direction = dir;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, direction, 0);
        phase = 0;
        phaseT = 0;
        visual.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }
    private void FixedUpdate()
    {
        switch (phase) 
        {
            case 0: 
                {
                    CastBeam();
                    phaseT += TRAVEL_DISTANCE * setupData.Timescale / BASE_STEP_DISTANCE / 200;
                    if (phaseT > 1) { phaseT = 0; phase = 1; }
                    break;
                }
            case 1: // Remove delay
                {
                    phaseT += Time.fixedDeltaTime / REMOVE_DELAY;
                    if (phaseT > 1) { gameObject.SetActive(false); }
                    break;
                }
        }
    }
    private void CastBeam()
    {
        float currentBeamStep = BASE_STEP_DISTANCE * setupData.Timescale;
        // Setup masks and hit buffers
        LayerMask terrainMask = LayerMask.GetMask("Walls");
        LayerMask hitboxMask = LayerMask.GetMask("EnemyHitboxes");
        RaycastHit terrainHits;
        RaycastHit entityHits;
        // Raycast ONLY WITH TERRAIN to create a path for the beam
        Physics.Raycast(transform.position, transform.forward, out terrainHits, currentBeamStep, terrainMask);

        // Spherecast ONLY WITH ENTITIES to check for damage
        bool beamInterruptedByEntity = false;
        Vector3 beamEnd = terrainHits.collider == null ? transform.position + transform.forward * currentBeamStep : terrainHits.point;
        Vector3 currentBeamPos = transform.position;
        float beamDiameter = CAST_RADIUS * setupData.SizeMultiplier;
        List<Collider> disabledColliders = new List<Collider>();
        while (!beamInterruptedByEntity && currentBeamPos != beamEnd)
        {
            Physics.SphereCast(currentBeamPos - (beamDiameter * 2 * transform.forward), beamDiameter * 0.5f, transform.forward, out entityHits, Vector3.Distance(currentBeamPos, beamEnd), hitboxMask);
            if (entityHits.collider != null)
            {
                // The spherecast found a hitbox on the way
                // Damage the entity hit by the beam
                EnemyEntity enemyHit = entityHits.collider.GetComponentInParent<EnemyEntity>();
                // Change the layer of the collider to ignore it on the next cast
                entityHits.collider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                disabledColliders.Add(entityHits.collider);
                currentBeamPos = transform.position + transform.forward * entityHits.distance;

                // If the enemy hit is supposed to be ignored, skip this part
                if (enemyHit.EnemyInstanceID != setupData.EnemyIgnored && !enemyHits.Contains(enemyHit.EnemyInstanceID))
                {
                    setupData.EnemyIgnored = enemyHit.EnemyInstanceID;
                    enemyHits.Add(enemyHit.EnemyInstanceID);
                    if (setupData.Splash > 0)
                    {
                        Explode(entityHits.point);
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
                            direction = GetBounceDirection(beamEnd, entityHits.collider);
                            transform.rotation = Quaternion.Euler(0, direction, 0);
                            enemyHits.Clear();
                        }
                    }
                    else
                    {
                        beamInterruptedByEntity = true;
                        beamEnd = currentBeamPos;
                        // No bounces/pierces left
                        phase = 1;
                        phaseT = 0;
                    }
                }

            }
            else
            {
                currentBeamPos = beamEnd;
            }

        }
        // If nothing stopped the entity spherecast and the terrain raycast found something... (the bullet hit a wall)
        if (!beamInterruptedByEntity && terrainHits.collider != null)
        {
            if (setupData.Splash > 0) { Explode(beamEnd); }
            if (setupData.Bounces > 0)
            {
                setupData.Bounces--;
                direction = GameTools.AngleReflection(direction, GameTools.NormalToEuler(terrainHits.normal) + 90);
                transform.rotation = Quaternion.Euler(0, direction, 0);
                enemyHits.Clear();
            }
            else 
            {
                phase = 1;
                phaseT = 0;
            }
        }
        beamEnd = Vector3.MoveTowards(beamEnd, transform.position, BOUNCE_END_SEPARATION);
        transform.position = beamEnd;

        // Return the colliders to their layer
        for (int i = 0; i < disabledColliders.Count; i++)
        {
            disabledColliders[i].gameObject.layer = LayerMask.NameToLayer("EnemyHitboxes");
        }
    }
}
