using UnityEngine;

public class BouncySpawnController : MonoBehaviour
{

    [SerializeField] private Transform visualParent;
    [SerializeField] private Rigidbody rb;

    private float randomizedHeight;

    private int bouncesDone;
    private float bounceT;
    public bool BounceAnimationFinished { get; private set; }

    private const float SPAWN_MOVEMENT_SPEED = 10f;
    private const float HEIGHT_MAX = 2.5f;
    private const float HEIGHT_MIN = 0.8f;
    private const int BOUNCES_MAX = 3;
    private const float BOUNCE_SPEED = 4f;

    void OnEnable()
    {
        bouncesDone = 0;
        randomizedHeight = Random.Range(HEIGHT_MIN, HEIGHT_MAX);
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 361), 0);
        bounceT = 0;
        BounceAnimationFinished = false;
    }

    private void FixedUpdate()
    {
        if (BounceAnimationFinished) { return; }

        rb.MovePosition(transform.position + Time.fixedDeltaTime / randomizedHeight * transform.forward * SPAWN_MOVEMENT_SPEED);
        bounceT += Time.fixedDeltaTime / randomizedHeight * BOUNCE_SPEED;
        UpdateBounceAnimation(bounceT);
        if (bounceT > 1)
        {
            bouncesDone++;
            BounceAnimationFinished = bouncesDone >= BOUNCES_MAX;
            bounceT = 0;
            UpdateBounceAnimation(0);
        }
    }
    private void UpdateBounceAnimation(float t)
    {
        visualParent.localPosition = new Vector3(0, Mathf.Sin(t * 180 * Mathf.Deg2Rad) * randomizedHeight / (bouncesDone + 1), 0);
    }
}
