using UnityEngine;

public class CurrencyPickup : MonoBehaviour
{
    [SerializeField] private Transform visualParent;
    [SerializeField] private Rigidbody rb;

    private float currencyValue;
    private float randomizedHeight;
    private int phase;
    private float phaseT;
    private int bouncesDone;
    private float pickupDragStrenght;
    private Transform autoPickupColliderTransform;

    private const float PICKUP_DRAG_STRENGHT_GROWTH = 15f;
    private const float HEIGHT_MAX = 2.5f;
    private const float HEIGHT_MIN = 0.8f;
    private const float LIFETIME = 20f;
    private const float REMOVE_DELAY = 1f;
    private const float ANIM_SPEED = 4f;
    private const int BOUNCES_MAX = 3;

    public void SetUp(Vector3 pos, float value) 
    {
        transform.position = pos;
        gameObject.SetActive(true);
        currencyValue = value;
        randomizedHeight = Random.Range(HEIGHT_MIN, HEIGHT_MAX);
        visualParent.rotation = Quaternion.Euler(0, Random.Range(0, 361), 0);
        rb.WakeUp();
        autoPickupColliderTransform = null;
        pickupDragStrenght = 0;
        phase = 0;
        phaseT = 0;
        bouncesDone = 0;
        transform.localScale = Vector3.one;
    }
    private void FixedUpdate()
    {
        switch (phase) 
        {
            case 0: // Spawning
                {
                    rb.MovePosition(transform.position + Time.fixedDeltaTime / randomizedHeight * visualParent.forward);
                    phaseT += Time.fixedDeltaTime / randomizedHeight * ANIM_SPEED;
                    UpdateBounceAnimation(phaseT);
                    if (phaseT > 1)
                    {
                        bouncesDone++;
                        phaseT = 0;
                        UpdateBounceAnimation(0);
                        if (bouncesDone >= BOUNCES_MAX) { phase = 1; }
                    }
                    break;
                }
            case 1: // Waiting for pickup
                {
                    phaseT += Time.fixedDeltaTime / LIFETIME;
                    if (autoPickupColliderTransform != null) { phaseT = 0; phase = 3; }
                    if (phaseT > 1) { phase = 2; phaseT = 0; }
                    break;
                }
            case 2: // Remove delay
                {
                    phaseT += Time.fixedDeltaTime / REMOVE_DELAY;
                    transform.localScale = Vector3.one * (1 - phaseT);
                    if (phaseT > 1) { gameObject.SetActive(false); }
                    break;
                }
            case 3: // Being picked up
                {
                    pickupDragStrenght += Time.fixedDeltaTime * PICKUP_DRAG_STRENGHT_GROWTH;
                    transform.position = Vector3.MoveTowards(transform.position, autoPickupColliderTransform.position, Time.fixedDeltaTime * pickupDragStrenght);
                    transform.localScale = Vector3.one / (1 + pickupDragStrenght*0.1f);
                    if (transform.position == autoPickupColliderTransform.position) 
                    {
                        gameObject.SetActive(false);
                        StageManagerBase.AddCurrency(currencyValue);
                    }
                    break;
                }
        }
    }
    private void UpdateBounceAnimation(float t) 
    {
        visualParent.localPosition = new Vector3(0, Mathf.Sin(t * 180 * Mathf.Deg2Rad) * randomizedHeight / (bouncesDone+1), 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) { return; }

        autoPickupColliderTransform = other.transform;
        rb.Sleep();
    }
}
