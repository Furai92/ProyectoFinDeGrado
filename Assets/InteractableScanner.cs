using UnityEngine;
using System.Collections.Generic;

public class InteractableScanner : MonoBehaviour
{
    private Interactable closestInteractable;
    private List<Interactable> interactablesInRange;
    private float nextInteractableUpdate;

    private const float INTERACTABLE_UPDATE_RATE = 0.5f;

    private void OnEnable()
    {
        interactablesInRange = new List<Interactable>();
        nextInteractableUpdate = Time.time + INTERACTABLE_UPDATE_RATE;
    }
    private void Update()
    {
        if (Time.time > nextInteractableUpdate) 
        {
            UpdateClosestInteractable();
            nextInteractableUpdate = Time.time + INTERACTABLE_UPDATE_RATE;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Interactable inter = other.gameObject.GetComponentInParent<Interactable>();
        if (inter != null)
        {
            interactablesInRange.Add(inter);
            UpdateClosestInteractable();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Interactable inter = other.gameObject.GetComponentInParent<Interactable>();
        if (inter != null) 
        {
            interactablesInRange.Remove(inter);
            UpdateClosestInteractable();
        }
    }

    private void UpdateClosestInteractable() 
    {
        Interactable closest = null;
        float smallestMag = Mathf.Infinity;
        for (int i = 0; i < interactablesInRange.Count; i++) 
        {
            float mag = (transform.position - interactablesInRange[i].transform.position).sqrMagnitude;
            if (mag < smallestMag) 
            {
                smallestMag = mag;
                closest = interactablesInRange[i];
            }
        }
        HudInteractionManager.UpdateTargetInteractable(closest);
    }
}
