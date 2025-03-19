using UnityEngine;

public abstract class PlayerAttackBase : MonoBehaviour
{
    protected WeaponAttackSetupData setupData;
    public abstract void SetUp(Vector3 pos, float direction, WeaponAttackSetupData sd, PlayerAttackBase parentAttack);

    private const float BOUNCE_TARGET_SEARCH_RADIUS = 20f;

    protected void Explode(Vector3 center)
    {
        WeaponAttackSetupData explosionsd = new WeaponAttackSetupData(setupData);
        explosionsd.SizeMultiplier = setupData.Splash;

        ObjectPoolManager.GetPlayerAttackFromPool("EXPLOSION").SetUp(center, 0, explosionsd, this);
    }
    public float GetBounceDirection(Vector3 pos, Collider hitboxIgnored = null)
    {
        LayerMask mask = LayerMask.GetMask("EnemyHitboxes");
        Collider[] hitboxesFound = Physics.OverlapSphere(pos, BOUNCE_TARGET_SEARCH_RADIUS, mask);

        Collider hitboxSelected;

        if (hitboxesFound.Length == 0)
        {
            // Nothing found, return a random direction
            return Random.Range(0, 361);
        }
        else 
        {
            int randomIndexSelected = Random.Range(0, hitboxesFound.Length);
            hitboxSelected = hitboxesFound[randomIndexSelected];
            if (hitboxSelected == hitboxIgnored) 
            {
                randomIndexSelected++;
                if (randomIndexSelected >= hitboxesFound.Length) { randomIndexSelected = 0; }
                hitboxSelected = hitboxesFound[randomIndexSelected];
                if (hitboxSelected == hitboxIgnored) 
                {
                    // Only one hitbox found, and it's the one that should be ignored
                    return Random.Range(0, 361);
                }
            }
        }
        return GameTools.AngleBetween(pos, hitboxSelected.transform.position);
    }
}
