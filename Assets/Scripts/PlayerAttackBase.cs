using UnityEngine;
using System.Collections.Generic;

public abstract class PlayerAttackBase : MonoBehaviour
{
    protected WeaponAttackSetupData setupData;
    public abstract void SetUp(Vector3 pos, float direction, WeaponAttackSetupData sd, PlayerAttackBase parentAttack);

    private const float BOUNCE_TARGET_SEARCH_RADIUS = 30f;

    protected void Explode(Vector3 center)
    {
        WeaponAttackSetupData explosionsd = new WeaponAttackSetupData(setupData);
        explosionsd.SizeMultiplier = setupData.Splash;

        ObjectPoolManager.GetPlayerAttackFromPool("EXPLOSION").SetUp(center, 0, explosionsd, this);
    }
    public float GetBounceDirection(Vector3 pos, bool losCheck, Collider hitboxIgnored = null)
    {
        LayerMask hitboxMask = LayerMask.GetMask("EnemyHitboxes");
        Collider[] hitboxesFound = Physics.OverlapSphere(pos, BOUNCE_TARGET_SEARCH_RADIUS, hitboxMask);


        List<Vector3> validBounceTargetPositions = new List<Vector3>();
        for (int i = 0; i < hitboxesFound.Length; i++)
        {
            if (hitboxesFound[i] == hitboxIgnored) { continue; }
            if (losCheck)
            {
                LayerMask wallMask = LayerMask.GetMask("Walls");
                if (Physics.Raycast(pos, pos - hitboxesFound[i].transform.position, Vector3.Distance(pos, hitboxesFound[i].transform.position), wallMask)) { continue; }
            }
            validBounceTargetPositions.Add(hitboxesFound[i].transform.position);
        }
        if (validBounceTargetPositions.Count == 0)
        {
            return Random.Range(0, 361); // Nothing found, return a random direction
        } 
        else 
        {
            return GameTools.AngleBetween(pos, validBounceTargetPositions[Random.Range(0, validBounceTargetPositions.Count)]);
        }
    }
}
