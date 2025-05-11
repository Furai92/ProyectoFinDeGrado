using UnityEngine;

public class TechCombatEffectBulletstorm : TechCombatEffect
{
    private float shootDir;
    private float direction;
    private float nextShootTime;
    private float shootInterval;
    private int shootsFired;
    private const int SHOOTS = 5;

    protected override void Initialize(Vector3 pos, float dir)
    {
        gameObject.SetActive(true);
        transform.position = pos;
        direction = dir;
        shootDir = 0;
        transform.rotation = Quaternion.Euler(0, direction, 0);
        shootInterval = PlayerEntity.DASH_DURATION / SHOOTS;
        nextShootTime = Time.time + shootInterval;
        shootsFired = 0;
    }
    private void FixedUpdate()
    {
        transform.position = PlayerEntity.ActiveInstance.transform.position;
        if (Time.time > nextShootTime) 
        {
            PlayerEntity.ActiveInstance.Attack(WeaponSO.WeaponSlot.Ranged, direction + shootDir, false, true);
            shootDir += 360 / SHOOTS;
            nextShootTime = Time.time + shootInterval;
            shootsFired++;
            if (shootsFired >= SHOOTS) { gameObject.SetActive(false); }
        }
    }
}
