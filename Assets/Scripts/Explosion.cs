using UnityEngine;

public class Explosion : PlayerAttackBase
{
    private float magnitude;
    private float removeTime;
    private const float LIFETIME = 1f;

    public void SetUp(float size, float mag) { 
        gameObject.SetActive(true);
        transform.localScale = Vector3.one * size;
        removeTime = Time.time + LIFETIME;
        magnitude = mag;
    }

    void Update()
    {
        if (Time.time > removeTime) { gameObject.SetActive(false); }   
    }
}
