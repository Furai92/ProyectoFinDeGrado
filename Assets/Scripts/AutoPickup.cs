using UnityEngine;

public abstract class AutoPickup : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private BouncySpawnController bsc;

    private float currencyValue;
    private int phase;
    private float phaseT;

    private Vector3 targetPosition;
    private PlayerEntity playerCollidedWith;
    private Transform autoPickupColliderTransform;

    private const float PICKUP_ORBIT_ROTATION_SPEED = 15f;
    private const float PICKUP_ORBIT_DISTANCE = 4f;
    private const float PICKUP_ORBIT_DURATION = 1f;
    private const float LIFETIME = 20f;
    private const float REMOVE_DELAY = 1f;


    public void SetUp(Vector3 pos, float value) 
    {
        transform.position = pos;
        gameObject.SetActive(true);
        currencyValue = value;
        rb.WakeUp();
        autoPickupColliderTransform = null;
        phase = 0;
        phaseT = 0;
        transform.localScale = Vector3.one;
    }

    private void FixedUpdate()
    {
        switch (phase)
        {
            case 0: // Waiting for pickup
                {
                    phaseT += Time.fixedDeltaTime / LIFETIME;
                    if (bsc.BounceAnimationFinished && autoPickupColliderTransform != null) { phase = 2; phaseT = 0; }
                    if (phaseT > 1) { phase = 1; phaseT = 0; }
                    break;
                }
            case 1: // Remove delay
                {
                    phaseT += Time.fixedDeltaTime / REMOVE_DELAY;
                    transform.localScale = Vector3.one * (1 - phaseT);
                    if (phaseT > 1) { gameObject.SetActive(false); }
                    break;
                }
            case 2: // Being picked up
                {
                    phaseT += Time.fixedDeltaTime / PICKUP_ORBIT_DURATION;
                    transform.localScale = Vector3.one * (1 - phaseT);
                    Vector3 orbitOffset = (1-phaseT) * PICKUP_ORBIT_DISTANCE * new Vector3(Mathf.Sin(phaseT * PICKUP_ORBIT_ROTATION_SPEED), 0, Mathf.Cos(phaseT * PICKUP_ORBIT_ROTATION_SPEED));
                    targetPosition = autoPickupColliderTransform.position + orbitOffset;
                    transform.position = Vector3.Lerp(transform.position, targetPosition, phaseT);
                    if (phaseT >= 1) 
                    {
                        gameObject.SetActive(false);
                        OnPickup(playerCollidedWith);
                    }
                    break;
                }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) { return; }
        PlayerEntity p = other.gameObject.GetComponentInParent<PlayerEntity>();
        if (p == null) { return; }

        playerCollidedWith = p;
        autoPickupColliderTransform = other.transform;
        rb.Sleep();
    }

    public abstract void OnPickup(PlayerEntity p);
}
