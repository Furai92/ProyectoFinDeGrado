using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract InteractableInfo GetInfo();


    public struct InteractableInfo 
    {
        public Color color;
        public WeaponData weapon;
        public string name;
        public string desc;
        public Transform gotransform;
    }
}



