using UnityEngine;

public class AutoPickup : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private BouncySpawnController bsc;

    private float currencyValue;
    private int phase;
    private float phaseT;
    private float pickupDragStrenght;
    private Transform autoPickupColliderTransform;

    
    private const float PICKUP_DRAG_STRENGHT_GROWTH = 15f;
    private const float PICKUP_DRAG_STRENGHT_BASE = 5f;
    private const float LIFETIME = 20f;
    private const float REMOVE_DELAY = 1f;


    public void SetUp(Vector3 pos, float value) 
    {
        transform.position = pos;
        gameObject.SetActive(true);
        currencyValue = value;
        rb.WakeUp();
        autoPickupColliderTransform = null;
        pickupDragStrenght = 0;
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
                    pickupDragStrenght += Time.fixedDeltaTime * PICKUP_DRAG_STRENGHT_GROWTH;
                    transform.position = Vector3.MoveTowards(transform.position, autoPickupColliderTransform.position, Time.fixedDeltaTime * (pickupDragStrenght + PICKUP_DRAG_STRENGHT_BASE));
                    transform.localScale = Vector3.one / (1 + pickupDragStrenght * 0.1f);
                    if (transform.position == autoPickupColliderTransform.position)
                    {
                        gameObject.SetActive(false);
                        StageManagerBase.AddCurrency(currencyValue);
                    }
                    break;
                }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) { return; }

        autoPickupColliderTransform = other.transform;
        rb.Sleep();
    }
}
