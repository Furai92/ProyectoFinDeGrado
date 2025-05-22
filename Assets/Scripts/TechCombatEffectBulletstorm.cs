using UnityEngine;

public class TechCombatEffectBulletstorm : TechCombatEffect
{
    [SerializeField] private ParticleSystem ps;

    private float shootDir;
    private float direction;
    private float nextShootTime;
    private float shootInterval;
    private int shootsFired;
    private const int SHOOTS = 6;
    private bool waitingToRemove;
    private float removeTime;

    private const float REMOVE_DELAY = 2f;

    protected override void Initialize(Vector3 pos, float dir)
    {
        gameObject.SetActive(true);
        transform.position = pos;
        direction = PlayerEntity.ActiveInstance.CurrentLookDirection;
        shootDir = 0;
        transform.rotation = Quaternion.Euler(0, direction, 0);
        shootInterval = PlayerEntity.DASH_DURATION / SHOOTS;
        nextShootTime = Time.time + shootInterval;
        shootsFired = 0;
        waitingToRemove = false;
        ps.Play();
    }
    private void FixedUpdate()
    {
        transform.position = PlayerEntity.ActiveInstance.transform.position;
        if (waitingToRemove)
        {
            if (Time.time > removeTime) { gameObject.SetActive(false); } 
        }
        else 
        {
            if (Time.time > nextShootTime)
            {
                PlayerEntity.ActiveInstance.Attack(WeaponSO.WeaponSlot.Ranged, direction + shootDir, false, true);
                shootDir += 360 / SHOOTS;
                nextShootTime = Time.time + shootInterval;
                shootsFired++;
                if (shootsFired >= SHOOTS) 
                {
                    removeTime = Time.time + REMOVE_DELAY;
                    waitingToRemove = true;
                    ps.Stop();
                }
            }
        }


    }
}
