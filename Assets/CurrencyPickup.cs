using UnityEngine;

public class CurrencyPickup : MonoBehaviour
{
    [SerializeField] private Transform visualParent;
    [SerializeField] private Rigidbody rb;

    private float currencyValue;
    private float randomizedSpeed;
    private float randomizedHeight;
    private float lifetimeRemaining;
    private int bouncesDone;

    private const float SPEED_BASE = 2f;
    private const float SPEED_MAX = 3f;
    private const float SPEED_MIN = 1f;
    private const float HEIGHT_MAX = 2f;
    private const float HEIGHT_MIN = 0.5f;
    private const float LIFETIME = 20f;
    private const int BOUNCES = 5;

    public void SetUp(Vector3 pos, float value) 
    {
        transform.position = pos;
        gameObject.SetActive(true);
        currencyValue = value;
        randomizedHeight = Random.Range(HEIGHT_MIN, HEIGHT_MAX);
        randomizedSpeed = Random.Range(SPEED_MIN, SPEED_MAX);
        lifetimeRemaining = LIFETIME;
        visualParent.rotation = Quaternion.Euler(0, Random.Range(0, 361), 0);
        bouncesDone = 0;
    }
    private void FixedUpdate()
    {
        if (bouncesDone < BOUNCES) 
        {
            rb.MovePosition(transform.position + Time.fixedDeltaTime * randomizedSpeed * visualParent.forward);
        }
        

        lifetimeRemaining -= lifetimeRemaining * Time.fixedDeltaTime;
        if (lifetimeRemaining < 0) { gameObject.SetActive(false); }
    }
}
