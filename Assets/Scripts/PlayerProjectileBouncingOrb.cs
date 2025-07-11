using UnityEngine;

public class PlayerProjectileBouncingOrb : PlayerProjectileBase
{
    [SerializeField] private Transform bounceVisualParent;
    [SerializeField] private MeshRenderer mr;

    private float bounceT;

    private const float INITIAL_BOUNCE_T = 0.25f;
    private const float BOUNCE_VISUAL_HEIGHT_MIN = -0.8f;
    private const float BOUNCE_VISUAL_HEIGHT_MAX = 2f;
    private const float BOUNCES_PER_SECOND = 1f;
    private const float ORB_ALPHA = 0.8f;

    private void UpdateBounceVisual()
    {
        bounceVisualParent.localPosition = new Vector3(0, BOUNCE_VISUAL_HEIGHT_MIN + Mathf.Sin(bounceT * 180 * Mathf.Deg2Rad) * BOUNCE_VISUAL_HEIGHT_MAX, 0);
    }

    public override void OnBulletEnd()
    {
        
    }

    public override void OnBulletSpawn()
    {
        mr.material.SetColor("_Color", cdb.ElementToColor(setupData.Element));
        mr.material.SetFloat("_Alpha", ORB_ALPHA);
        bounceT = INITIAL_BOUNCE_T;
        UpdateBounceVisual();
        tr.Clear();
        tr.AddPosition(bounceVisualParent.position);
        mr.gameObject.SetActive(true);
    }

    public override void OnFixedUpdate()
    {
        bounceT = Mathf.MoveTowards(bounceT, 1, BOUNCES_PER_SECOND * setupData.Timescale * Time.fixedDeltaTime);
        UpdateBounceVisual();
        if (bounceT == 1)
        {
            PlayImpactEffects(transform.position, CurrentDirection);
            Explode(transform.position);
            setupData.Bounces--;
            bounceT = 0;
            if (setupData.Bounces < 0) 
            {
                SetRemoveable();
                mr.gameObject.SetActive(false);
            }
        }
    }

    public override void OnEntityCollision()
    {
        
    }

    public override void OnTerrainCollision()
    {
        
    }

}
