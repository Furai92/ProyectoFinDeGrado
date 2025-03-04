using UnityEngine;
using System.Collections.Generic;

public class PlayerBeamAttack : PlayerAttackBase
{
    [SerializeField] private Transform visual;

    private int phase;
    private float phaseT;
    private float direction;

    private const float BOUNCE_END_SEPARATION = 0.1f; // Helps improve bounces with walls
    private const float CAST_LENGHT = 50f;
    private const float CAST_RADIUS = 0.25f;
    private const float INHERITED_LAUNCH_DELAY = 0.05f;
    private const float LIFETIME = 0.3f;

    public override void SetUp(Vector3 pos, float dir, WeaponAttackSetupData sd, PlayerAttackBase parentAttack)
    {
        setupData = sd;
        direction = dir;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, direction, 0);
        phase = 0;
        phaseT = parentAttack == null ? 1 : 0;
        visual.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
    private void FixedUpdate()
    {
        switch (phase) 
        {
            case 0: // Pre launch delay (bounces only)
                {
                    phaseT += Time.fixedDeltaTime / INHERITED_LAUNCH_DELAY;
                    if (phaseT > 1) { phaseT = 0; phase++; CastBeam(); }
                    break;
                }
            case 1: // Active time 
                {
                    phaseT += Time.fixedDeltaTime / LIFETIME;
                    if (phaseT > 1) { gameObject.SetActive(false); }
                    break;
                }
        }
    }
    private void CastBeam()
    {
        // Setup masks and hit buffers
        LayerMask terrainMask = LayerMask.GetMask("Walls");
        LayerMask hitboxMask = LayerMask.GetMask("Hitboxes");
        RaycastHit terrainHits;
        RaycastHit entityHits;
        // Raycast ONLY WITH TERRAIN to create a path for the beam
        Physics.Raycast(transform.position, transform.forward, out terrainHits, CAST_LENGHT, terrainMask);

        // Spherecast ONLY WITH ENTITIES to check for damage
        bool beamInterrupted = false;
        Vector3 beamEnd = terrainHits.collider == null ? transform.position + transform.forward * CAST_LENGHT : terrainHits.point;
        Vector3 currentBeamPos = transform.position;
        float beamDiameter = CAST_RADIUS * setupData.sizemult;
        List<Collider> disabledColliders = new List<Collider>();
        while (!beamInterrupted && currentBeamPos != beamEnd) 
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
                if (enemyHit.EnemyInstanceID != setupData.enemyIgnored) 
                {
                    if (setupData.splash > 0) 
                    {
                        Explode(enemyHit.transform.position); 
                    }
                    else 
                    {
                        enemyHit.DealDamage(setupData.magnitude, setupData.critchance, setupData.critdamage, setupData.builduprate, setupData.element);
                    }
                    
                    // Spend pierces to continue
                    if (setupData.pierces > 0)
                    {
                        setupData.pierces--;
                    }
                    // If there's no pierces left, use bounces AND stop the beam
                    else
                    {
                        beamInterrupted = true;
                        beamEnd = currentBeamPos;
                        if (setupData.bounces > 0)
                        {
                            setupData.bounces--;
                            EnemyEntity closestEnemy = StageManagerBase.GetBounceTarget(beamEnd, enemyHit.EnemyInstanceID);
                            CreateBounce(currentBeamPos, closestEnemy == null ? Random.Range(0, 361) : GameTools.AngleBetween(beamEnd, closestEnemy.transform.position), enemyHit.EnemyInstanceID);
                        }
                    }
                }
                
            }
            else
            {
                currentBeamPos = beamEnd;
            }

        }
        // If nothing stopped the entity spherecast and the terrain raycast found something...
        if (!beamInterrupted && terrainHits.collider != null)
        {
            if (setupData.bounces > 0)
            {
                if (setupData.splash > 0) { Explode(beamEnd); }
                setupData.bounces--;
                CreateBounce(beamEnd, GameTools.AngleReflection(direction, GameTools.NormalToEuler(terrainHits.normal) + 90), -1);
            }
        }
        // Visually stretch the beam to the corresponding shape
        transform.localScale = new Vector3(beamDiameter, beamDiameter, Vector3.Distance(transform.position, beamEnd));
        visual.gameObject.SetActive(true);
        // Return the colliders to their layer
        for (int i = 0; i < disabledColliders.Count; i++) 
        {
            disabledColliders[i].gameObject.layer = LayerMask.NameToLayer("Hitboxes");
        }
    }
    private void CreateBounce(Vector3 startPos, float bdir, int lastHitEnemyID) 
    {
        WeaponAttackSetupData bounceSd = new WeaponAttackSetupData()
        {
            magnitude = setupData.magnitude,
            critchance = setupData.critchance,
            critdamage = setupData.critdamage,
            sizemult = setupData.sizemult,
            splash = setupData.splash,
            timescale = setupData.timescale,
            bounces = setupData.bounces,
            builduprate = setupData.builduprate,
            pierces = setupData.pierces,
            element = setupData.element,
            enemyIgnored = lastHitEnemyID
        };
        StageManagerBase.GetObjectPool().GetPlayerAttackFromPool("BEAM").SetUp(Vector3.MoveTowards(startPos, transform.position, BOUNCE_END_SEPARATION), bdir, bounceSd, this);
    }
}
