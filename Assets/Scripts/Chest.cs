using UnityEngine;

public class Chest : Interactable
{
    private int chestID;
    private InteractableInfo info;

    private const float SPAWN_HEIGHT = 0.5f;
    private const int BASE_CURRENCY_DROP_RATE = 10;

    public void SetUp(Vector3 pos, int id) 
    {
        gameObject.SetActive(true);
        chestID = id;
        transform.position = new Vector3(pos.x, SPAWN_HEIGHT, pos.z);
    }
    private void OnEnable()
    {

        EventManager.StageStateEndedEvent += OnStageStateEnded;
        info = new InteractableInfo()
        {
            color = Color.white,
            desc = "Open",
            gotransform = transform,
            name = "Chest",
            weapon = null
        };
    }
    protected override void OnDisable()
    {
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
        EventManager.OnChestDisabled(chestID);
        base.OnDisable();
    }

    private void OnStageStateEnded(StageStateBase.GameState s) 
    {
        gameObject.SetActive(false);
    }

    public override InteractableInfo GetInfo()
    {
        return info;
    }

    public override void OnInteract(int playerIndex)
    {
        gameObject.SetActive(false);

        int drops = (int)(BASE_CURRENCY_DROP_RATE * StageManagerBase.GetStageStat(StageStatGroup.StageStat.ChestDropRate));

        for (int i = 0; i < drops; i++) 
        {
            ObjectPoolManager.GetCurrencyPickupFromPool().SetUp(transform.position);
        }
    }
}
