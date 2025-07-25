using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class HudShop : MonoBehaviour, IGameMenu
{
    [SerializeField] private GameDatabaseSO dbso;
    [SerializeField] private ColorDatabaseSO cdb;
    [SerializeField] private List<Transform> cardParents;
    [SerializeField] private List<HudTechCard> cards;
    [SerializeField] private List<Transform> soldOutPanels;
    [SerializeField] private List<Transform> purchaseButtons;
    [SerializeField] private List<TextMeshProUGUI> prices;
    [SerializeField] private TextMeshProUGUI refreshCostText;
    [SerializeField] private Transform menuParent;
    [SerializeField] private TextMeshProUGUI currencyCount;

    private TechPool tp;
    private int refreshesDone;
    private List<TechSO> techInStock;

    private const float TECH_CHANCE_RARE = 35f;
    private const float TECH_CHANCE_EXOTIC = 10f;
    private const float TECH_CHANCE_PROTOTYPE = 5f;
    private const float TECH_PRICE_COMMON = 5f;
    private const float TECH_PRICE_RARE = 10f;
    private const float TECH_PRICE_EXOTIC = 20f;
    private const float TECH_PRICE_PROTOTYPE = 35f;
    private const float REFRESH_COST_BASE = 0f;
    private const float REFRESH_COST_SCALING = 1f;

    private void OnEnable()
    {
        EventManager.StageStateStartedEvent += OnStageStateStarted;
        EventManager.StageStateEndedEvent += OnStageStateEnded;
        tp = new TechPool(dbso, PersistentDataManager.GetTraitSelection());
        gameObject.SetActive(true);
        menuParent.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
        EventManager.StageStateEndedEvent -= OnStageStateEnded;
    }
    private void OnStageStateStarted(StageStateBase.GameState s)
    {
        if (s != StageStateBase.GameState.Rest) { return; }

        refreshesDone = 0;
        Refresh();
    }
    private void OnStageStateEnded(StageStateBase.GameState s)
    {
        if (IsOpen())
        {
            CloseMenu();
        }
    }
    private void Refresh() 
    {
        techInStock = new List<TechSO>();
        for (int i = 0; i < 5; i++) 
        {
            GameEnums.Rarity selectedRarity = GameEnums.Rarity.Common;
            int rarityRoll = Random.Range(0, 101);
            if (rarityRoll < TECH_CHANCE_RARE) 
            {
                if (rarityRoll < TECH_CHANCE_EXOTIC)
                {
                    if (rarityRoll < TECH_CHANCE_PROTOTYPE)
                    {
                        selectedRarity = GameEnums.Rarity.Prototype;
                    }
                    else 
                    {
                        selectedRarity = GameEnums.Rarity.Exotic;
                    }
                }
                else 
                {
                    selectedRarity = GameEnums.Rarity.Rare;
                }
            }
            techInStock.Add(tp.GetRandomTech(selectedRarity, PlayerEntity.ActiveInstance.ActiveTechDictionary));
        }
        UpdateShopVisuals();
    }
    private void UpdateShopVisuals() 
    {
        refreshCostText.text = string.Format("Reroll: {0}", GetRefreshPrice().ToString("F0")); 
        refreshCostText.color = GetRefreshPrice() <= PlayerEntity.ActiveInstance.Money ? cdb.GetColor("DETAIL_TYPE_POSITIVE") : cdb.GetColor("DETAIL_TYPE_NEGATIVE");
        currencyCount.text = string.Format("Currency: {0}", PlayerEntity.ActiveInstance.Money);
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
                    prices[i].color = GetTechPrice(techInStock[i]) <= PlayerEntity.ActiveInstance.Money ? cdb.GetColor("DETAIL_TYPE_POSITIVE") : cdb.GetColor("DETAIL_TYPE_NEGATIVE");
                    cards[i].gameObject.SetActive(true); cards[i].SetUp(techInStock[i], PlayerEntity.ActiveInstance.GetTechLevel(techInStock[i].ID)+1, true, true, true);
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
        switch (t.Rarity) 
        {
            case GameEnums.Rarity.Common: { return TECH_PRICE_COMMON * StageManagerBase.GetStageStat(StageStatGroup.StageStat.ShopPriceMult); }
            case GameEnums.Rarity.Rare: { return TECH_PRICE_RARE * StageManagerBase.GetStageStat(StageStatGroup.StageStat.ShopPriceMult); }
            case GameEnums.Rarity.Exotic: { return TECH_PRICE_EXOTIC * StageManagerBase.GetStageStat(StageStatGroup.StageStat.ShopPriceMult); }
            case GameEnums.Rarity.Prototype: { return TECH_PRICE_PROTOTYPE * StageManagerBase.GetStageStat(StageStatGroup.StageStat.ShopPriceMult); }
        }
        return 0;
    }
    private float GetRefreshPrice() 
    {
        return REFRESH_COST_BASE + StageManagerBase.GetStageStat(StageStatGroup.StageStat.ShopRerollBaseCost) + (refreshesDone * REFRESH_COST_SCALING);
    }
    public void OnRefreshClicked() 
    {
        if (!menuParent.gameObject.activeInHierarchy) { return; }
        if (StageManagerBase.GetCurrentStateType() != StageStateBase.GameState.Rest) { return; }
        if (PlayerEntity.ActiveInstance.Money < GetRefreshPrice()) { return; }

        PlayerEntity.ActiveInstance.SpendMoney(GetRefreshPrice());
        refreshesDone++;
        Refresh();
    }
    public void OnPurchaseClicked(int index) 
    {
        if (!menuParent.gameObject.activeInHierarchy) { return; }
        if (StageManagerBase.GetCurrentStateType() != StageStateBase.GameState.Rest) { return; }
        if (index >= techInStock.Count) { return; }
        if (techInStock[index] == null) { return; }
        if (PlayerEntity.ActiveInstance.Money < GetTechPrice(techInStock[index])) { return; }

        PlayerEntity.ActiveInstance.SpendMoney(GetTechPrice(techInStock[index]));
        PlayerEntity.ActiveInstance.EquipTech(techInStock[index]);

        // It is possible that the store generated more than one item that has a purchase count limit.
        // If after this purchase, the tech is maxed out, and any of the tech's in the shop is the same as this one, it should be removed from the shop

        TechSO techPurchased = techInStock[index];
        if (techPurchased.MaxLevel > 0 && PlayerEntity.ActiveInstance.GetTechLevel(techPurchased.ID) >= techPurchased.MaxLevel) 
        {
            for (int i = 0; i < techInStock.Count; i++) 
            {
                if (techInStock[i] == null) { continue; }
                if (techInStock[i].ID == techPurchased.ID) { techInStock[i] = null; }
            }
        }
        techInStock[index] = null;
        UpdateShopVisuals();
    }
    public void OpenMenu()
    {
        menuParent.gameObject.SetActive(true);
        UpdateShopVisuals();
    }

    public void CloseMenu()
    {
        menuParent.gameObject.SetActive(false);
        EventManager.OnUiMenuClosed(this);
    }

    public bool IsOpen()
    {
        return menuParent.gameObject.activeInHierarchy;
    }

    public bool CanBeOpened()
    {
        return StageManagerBase.GetCurrentStateType() == StageStateBase.GameState.Rest && PlayerEntity.ActiveInstance != null;
    }
}
