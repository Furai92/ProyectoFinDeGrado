using UnityEngine;

public class WeaponPickup : Interactable
{
    [SerializeField] private Transform visualParent;
    private InteractableInfo info;

    private const float VISUAL_ROTATION_SPEED = 200f;
    private const float SPAWN_HEIGHT = 0.5f;

    public void SetUp(WeaponData wd, Vector3 wpos) 
    {
        transform.position = new Vector3(wpos.x, SPAWN_HEIGHT, wpos.z);
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
        EventManager.StageStateEndedEvent += OnStageStateEnded;
    }
    protected override void OnDisable()
    {
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
        base.OnDisable();
    }
    private void OnStageStateEnded(StageStateBase.GameState s) 
    {
        if (s == StageStateBase.GameState.Rest) 
        {
            gameObject.SetActive(false); 
        }
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
        PlayerEntity.ActiveInstance.EquipWeapon(info.weapon);
        gameObject.SetActive(false);
    }
}
