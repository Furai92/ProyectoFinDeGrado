using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected virtual void OnDisable()
    {
        EventManager.OnInteractableDisabled(this);
    }

    public abstract InteractableInfo GetInfo();
    public abstract void OnInteract(int playerIndex);


    public struct InteractableInfo 
    {
        public Color color;
        public WeaponData weapon;
        public string name;
        public string desc;
        public Transform gotransform;
    }
}



