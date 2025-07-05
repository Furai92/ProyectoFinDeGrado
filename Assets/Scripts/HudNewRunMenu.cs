using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class HudNewRunMenu : MonoBehaviour, IGameMenu
{
    [SerializeField] private GameDatabaseSO database;
    [SerializeField] private StringDatabaseSO sdb;
    [SerializeField] private Transform menuParent;
    [SerializeField] private HudTechCard techCard;
    [SerializeField] private HudDifficultyLevelCard difficultyCard;
    [SerializeField] private TextMeshProUGUI techBoostedChanceDesc;
    [SerializeField] private HudPageDisplay pageDisplay;

    [SerializeField] private TextMeshProUGUI headerMain;
    [SerializeField] private TextMeshProUGUI headerSub;

    [SerializeField] private List<PlayerTraitSO> page0Traits;
    [SerializeField] private List<PlayerTraitSO> page1Traits;
    [SerializeField] private TextMeshProUGUI difficultyDesc;
    [SerializeField] private TextMeshProUGUI difficultyDescAccumulative;
    [SerializeField] private CanvasGroup pageAnimCG;

    private int currentPage;
    private int trait0SelectIndex;
    private int trait1SelectIndex;
    private int difficultySelectIndex;

    private const float PAGE_ANIM_DISPLACEMENT = 150f;
    private const float PAGE_ANIM_DURATION = 0.25f;

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    private void UpdatePage() 
    {
        headerMain.text = sdb.GetString("NEWRUN_MENU_HEADER_MAIN");
        difficultyCard.SetUp(database.DifficultyLevels[difficultySelectIndex]);
        difficultyCard.gameObject.SetActive(currentPage == 2);
        difficultyDesc.gameObject.SetActive(currentPage == 2);
        difficultyDescAccumulative.gameObject.SetActive(currentPage == 2 && difficultySelectIndex != 0);
        techCard.gameObject.SetActive(currentPage != 2);
        
        switch (currentPage) 
        {
            case 0: 
                {
                    techCard.SetUp(page0Traits[trait0SelectIndex].Tech, 1, false, false, false);
                    techBoostedChanceDesc.gameObject.SetActive(true);
                    techBoostedChanceDesc.text = string.Format(sdb.GetString("NEWRUN_MENU_BOOSTED_CHANCE"), sdb.GetString(page0Traits[trait0SelectIndex].NameID));
                    pageDisplay.UpdateDisplay(page0Traits.Count, trait0SelectIndex);
                    headerSub.text = sdb.GetString("NEWRUN_MENU_HEADER_0");
                    break;
                }
            case 1:
                {
                    techCard.SetUp(page1Traits[trait1SelectIndex].Tech, 1, false, false, false);
                    techBoostedChanceDesc.gameObject.SetActive(true);
                    techBoostedChanceDesc.text = string.Format(sdb.GetString("NEWRUN_MENU_BOOSTED_CHANCE"), sdb.GetString(page1Traits[trait1SelectIndex].NameID));
                    pageDisplay.UpdateDisplay(page1Traits.Count, trait1SelectIndex);
                    headerSub.text = sdb.GetString("NEWRUN_MENU_HEADER_1");
                    break;
                }
            case 2: 
                {
                    techBoostedChanceDesc.gameObject.SetActive(false);
                    pageDisplay.UpdateDisplay(database.DifficultyLevels.Count, difficultySelectIndex);
                    difficultyDesc.text = sdb.GetString(database.DifficultyLevels[difficultySelectIndex].DescID);
                    headerSub.text = sdb.GetString("NEWRUN_MENU_HEADER_2");
                    break;
                }
        }
    }

    public void OnNextPageClicked() 
    {
        switch (currentPage)
        {
            case 0: 
                {
                    currentPage = 1;
                    UpdatePage();
                    PlayPageAnim(true);
                    break;
                }
            case 1:
                {
                    currentPage = 2;
                    UpdatePage();
                    PlayPageAnim(true);
                    break;
                }
            case 2:
                {
                    StartRun();
                    break;
                }
        }
    }
    public void OnPrevPageClicked() 
    {
        switch (currentPage)
        {
            case 0:
                {
                    CloseMenu();
                    break;
                }
            case 1:
                {
                    currentPage = 0;
                    UpdatePage();
                    PlayPageAnim(false);
                    break;
                }
            case 2:
                {
                    currentPage = 1;
                    UpdatePage();
                    PlayPageAnim(false);
                    break;
                }
        }

    }

    public void OnNextElementClicked() 
    {
        switch (currentPage) 
        {
            case 0:
                {
                    trait0SelectIndex++;
                    if (trait0SelectIndex >= page0Traits.Count) { trait0SelectIndex = 0; }
                    break;
                }
            case 1:
                {
                    trait1SelectIndex++;
                    if (trait1SelectIndex >= page1Traits.Count) { trait1SelectIndex = 0; }
                    break;
                }
            case 2:
                {
                    difficultySelectIndex++;
                    if (difficultySelectIndex >= database.DifficultyLevels.Count) { difficultySelectIndex = 0; }
                    break;
                }
        }
        UpdatePage();
    }
    public void OnPrevElementClicked() 
    {
        switch (currentPage)
        {
            case 0:
                {
                    trait0SelectIndex--;
                    if (trait0SelectIndex < 0) { trait0SelectIndex = page0Traits.Count-1; }
                    break;
                }
            case 1:
                {
                    trait1SelectIndex--;
                    if (trait1SelectIndex < 0) { trait1SelectIndex = page1Traits.Count-1; }
                    break;
                }
            case 2:
                {
                    difficultySelectIndex--;
                    if (difficultySelectIndex < 0) { difficultySelectIndex = database.DifficultyLevels.Count - 1; }
                    break;
                }
        }
        UpdatePage();
    }
    private void PlayPageAnim(bool next) 
    {
        StopAllCoroutines();
        StartCoroutine(PageAnim(next));
    }
    private void UpdatePageAnim(float t, bool next) 
    {
        pageAnimCG.transform.localPosition = (1 - t) * (next ? PAGE_ANIM_DISPLACEMENT : -PAGE_ANIM_DISPLACEMENT) * Vector3.right;
        pageAnimCG.alpha = t;
    }
    private void StartRun() 
    {
        List<PlayerTraitSO> traitsSelected = new List<PlayerTraitSO>();
        traitsSelected.Add(page0Traits[trait0SelectIndex]);
        traitsSelected.Add(page1Traits[trait1SelectIndex]);

        PersistentDataManager.SetGameDifficulty(database.DifficultyLevels[difficultySelectIndex]);
        PersistentDataManager.SetTraitSelection(traitsSelected);
        SceneManager.LoadScene("Ingame");
    }

    public bool CanBeOpened()
    {
        return true;
    }

    public void CloseMenu()
    {
        EventManager.OnUiMenuClosed(this);
        menuParent.gameObject.SetActive(false);
    }

    public bool IsOpen()
    {
        return menuParent.gameObject.activeInHierarchy;
    }

    public void OpenMenu()
    {
        menuParent.gameObject.SetActive(true);
        currentPage = 0;
        UpdatePage();
        UpdatePageAnim(1, true);
    }
    public string GetMenuNameID()
    {
        return "MENU_NAME_NEW_RUN";
    }
    IEnumerator PageAnim(bool next) 
    {
        float t = 0;
        while (t < 1) 
        {
            t = Mathf.MoveTowards(t, 1, Time.unscaledDeltaTime / PAGE_ANIM_DURATION);
            UpdatePageAnim(t, next);
            yield return null;
        }
    }
}
