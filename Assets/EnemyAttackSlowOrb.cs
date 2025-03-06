using UnityEngine;

public class EnemyAttackSlowOrb : EnemyAttackBase
{
    private float removeTime;

    private const float ORB_SPEED = 7.5f;
    private const float LIFETIME = 10f;

    protected override void OnSetup(Vector3 pos, float dir)
    {
        gameObject.SetActive(true);
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, dir, 0);
        removeTime = Time.time + LIFETIME;
    }
    private void FixedUpdate()
    {
        transform.Translate(ORB_SPEED * Time.fixedDeltaTime * Vector3.forward);
        if (Time.time > removeTime) { gameObject.SetActive(false); }
    }
    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag) 
        {
            case "Wall": 
                {
                    gameObject.SetActive(false);
                    break;
                }
            case "Player": 
                {
                    gameObject.SetActive(false);
                    break;
                }
        }
    }
}
