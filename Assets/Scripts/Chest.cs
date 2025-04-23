using UnityEngine;

public class Chest : Interactable
{
    private InteractableInfo info;

    public void SetUp(Vector3 pos) 
    {
        gameObject.SetActive(true);
        transform.position = pos;
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
        ObjectPoolManager.GetCurrencyPickupFromPool().SetUp(transform.position);
        ObjectPoolManager.GetCurrencyPickupFromPool().SetUp(transform.position);
        ObjectPoolManager.GetCurrencyPickupFromPool().SetUp(transform.position);
        ObjectPoolManager.GetCurrencyPickupFromPool().SetUp(transform.position);
    }
}
