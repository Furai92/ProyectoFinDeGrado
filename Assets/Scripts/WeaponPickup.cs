using UnityEngine;

public class WeaponPickup : Interactable
{
    [SerializeField] private Transform visualParent;
    private InteractableInfo info;

    private const float VISUAL_ROTATION_SPEED = 200f;

    public void SetUp(WeaponData wd, Vector3 wpos) 
    {
        transform.position = wpos;
        info = new InteractableInfo()
        {
            color = Color.white,
            desc = "Equip",
            gotransform = transform,
            name = "Weapon",
            weapon = wd
        };
        gameObject.SetActive(true);
    }
    private void OnEnable()
    {
        EventManager.StageStateStartedEvent += OnStageStateStarted;
    }
    protected override void OnDisable()
    {
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
        base.OnDisable();
    }
    private void OnStageStateStarted(StageStateBase s) 
    {
        if (s.GetStateType() == StageStateBase.StateType.Combat) { gameObject.SetActive(false); }
    }
    public override InteractableInfo GetInfo()
    {
        return info;
    }
    private void Update()
    {
        visualParent.Rotate(0, VISUAL_ROTATION_SPEED * Time.deltaTime, 0);
    }

    public override void OnInteract(int playerIndex)
    {
        StageManagerBase.GetPlayerReference(playerIndex).EquipWeapon(info.weapon);
        gameObject.SetActive(false);
    }
}
