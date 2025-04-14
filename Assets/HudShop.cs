using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class HudShop : IngameMenuBase
{
    [SerializeField] private GameDatabaseSO dbso;
    [SerializeField] private List<Transform> cardParents;
    [SerializeField] private List<HudTechCard> cards;
    [SerializeField] private List<Transform> soldOutPanels;
    [SerializeField] private List<Transform> purchaseButtons;
    [SerializeField] private List<TextMeshProUGUI> prices;
    [SerializeField] private TextMeshProUGUI refreshCostText;
    [SerializeField] private Transform menuParent;

    private PlayerEntity playerRef;
    private int refreshesDone;
    private List<TechSO> techInStock;

    private const float REFRESH_COST_BASE = 5f;
    private const float REFRESH_COST_SCALING = 2f;

    public void SetUp(PlayerEntity p) 
    {
        gameObject.SetActive(true);
        menuParent.gameObject.SetActive(false);
        playerRef = p;
        EventManager.StageStateStartedEvent += OnStageStateStarted;
        EventManager.StageStateEndedEvent += OnStageStateEnded;
    }
    private void OnDisable()
    {
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
    }
    private void OnStageStateStarted(StageStateBase s) 
    {
        refreshesDone = 0;
        Refresh();
    }
    private void Refresh() 
    {
        techInStock = new List<TechSO>();
        for (int i = 0; i < 5; i++) 
        {
            techInStock.Add(GetCompatibleTech());
        }
        UpdateShopVisuals();
    }
    private void UpdateShopVisuals() 
    {
        refreshCostText.text = GetRefreshPrice().ToString("F0");
        for (int i = 0; i < cards.Count; i++) 
        {
            if (i < techInStock.Count)
            {
                //[SerializeField] private List<Transform> purchaseButtons;

                // The slot is used
                if (techInStock[i] == null)
                {
                    // Out of stock (slot is filled with a null tech)
                    cardParents[i].gameObject.SetActive(true);
                    soldOutPanels[i].gameObject.SetActive(true);
                    purchaseButtons[i].gameObject.SetActive(true);
                    prices[i].gameObject.SetActive(false);
                    cards[i].gameObject.SetActive(false);
                    purchaseButtons[i].gameObject.SetActive(false);
                }
                else 
                {
                    // Valid purchase
                    soldOutPanels[i].gameObject.SetActive(false);
                    purchaseButtons[i].gameObject.SetActive(true);
                    prices[i].gameObject.SetActive(true); prices[i].text = GetTechPrice(techInStock[i]).ToString("F0");
                    cards[i].gameObject.SetActive(true); cards[i].SetUp(techInStock[i]);
                    purchaseButtons[i].gameObject.SetActive(true);
                }
            }
            else 
            {
                // The slot is unused
                cardParents[i].gameObject.SetActive(false);
                soldOutPanels[i].gameObject.SetActive(false);
                purchaseButtons[i].gameObject.SetActive(false);
                prices[i].gameObject.SetActive(false); 
                cards[i].gameObject.SetActive(false);
            }
        }
    }
    private float GetTechPrice(TechSO t) 
    {
        return 5; // TEMP
    }
    private float GetRefreshPrice() 
    {
        return  REFRESH_COST_BASE + refreshesDone * REFRESH_COST_SCALING;
    }
    private TechSO GetCompatibleTech() 
    {
        List<TechSO> compatibleTech = new List<TechSO>();
        for (int i = 0; i < dbso.Techs.Count; i++) 
        {
            compatibleTech.Add(dbso.Techs[i]);
        }

        if (compatibleTech.Count > 0) 
        {
            return compatibleTech[Random.Range(0, compatibleTech.Count)];
        }
        return null;
    }
    private void OnStageStateEnded(StageStateBase s) 
    {
        if (IsOpen()) 
        {
            CloseMenu();
        }
    }
    public void OnRefreshClicked() 
    {
        if (!menuParent.gameObject.activeInHierarchy) { return; }
        if (StageManagerBase.GetCurrentStateType() != StageStateBase.StateType.Rest) { return; }
        if (StageManagerBase.GetPlayerCurrency(0) < GetRefreshPrice()) { return; }

        StageManagerBase.ChangeCurrency(-GetRefreshPrice());
        refreshesDone++;
        Refresh();
    }
    public void OnPurchaseClicked(int index) 
    {
        if (!menuParent.gameObject.activeInHierarchy) { return; }
        if (StageManagerBase.GetCurrentStateType() != StageStateBase.StateType.Rest) { return; }
        if (index >= techInStock.Count) { return; }
        if (techInStock[index] == null) { return; }
        if (StageManagerBase.GetPlayerCurrency(0) < GetTechPrice(techInStock[index])) { return; }

        StageManagerBase.ChangeCurrency(-GetTechPrice(techInStock[index]));
        playerRef.EquipTech(techInStock[index]);
        techInStock[index] = null;
        UpdateShopVisuals();
    }
    public override void OpenMenu()
    {
        menuParent.gameObject.SetActive(true);
    }

    public override void CloseMenu()
    {
        menuParent.gameObject.SetActive(false);
    }

    public override bool IsOpen()
    {
        return menuParent.gameObject.activeInHierarchy;
    }

    public override bool CanBeOpened()
    {
        return StageManagerBase.GetCurrentStateType() == StageStateBase.StateType.Rest;
    }
}
