using UnityEngine;

public class WeaponPickup : Interactable
{
    [SerializeField] private Transform visualParent;
    private InteractableInfo info;

    private const float VISUAL_ROTATION_SPEED = 200f;

    private void OnEnable()
    {
        info = new InteractableInfo()
        {
            color = Color.white,
            desc = "Equip",
            gotransform = transform,
            name = "Weapon",
            weapon = null
        };
    }
    public override InteractableInfo GetInfo()
    {
        return info;
    }
    private void Update()
    {
        visualParent.Rotate(0, VISUAL_ROTATION_SPEED * Time.deltaTime, 0);
    }
}
