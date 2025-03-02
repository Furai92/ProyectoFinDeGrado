using UnityEngine;

public class Explosion : PlayerAttackBase
{
    private WeaponAttackSetupData setupData;

    private float removeTime;
    private const float LIFETIME = 1f;

    public override void SetUp(Vector3 pos, float dir, WeaponAttackSetupData sd) {
        setupData = sd;
        gameObject.SetActive(true);
        transform.position = pos;
        transform.localScale = Vector3.one * sd.sizemult;
        removeTime = Time.time + LIFETIME;
    }

    void Update()
    {
        if (Time.time > removeTime) { gameObject.SetActive(false); }   
    }
}
