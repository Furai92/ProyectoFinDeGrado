using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class HudNewRunMenu : MonoBehaviour, IGameMenu
{
    [SerializeField] private StringDatabaseSO sdb;
    [SerializeField] private Transform menuParent;
    [SerializeField] private HudTraitCard traitCardPage0;
    [SerializeField] private HudTraitCard traitCardPage1;
    [SerializeField] private HudDifficultyLevelCard difficultyCard;
    [SerializeField] private TextMeshProUGUI elementSelectedDesc;

    [SerializeField] private TextMeshProUGUI headerMain;
    [SerializeField] private TextMeshProUGUI headerSub;

    [SerializeField] private List<PlayerTraitSO> page0Traits;
    [SerializeField] private List<PlayerTraitSO> page1Traits;
    [SerializeField] private List<DifficultyLevelSO> difficultyLevels;

    private int currentPage;
    private int trait0SelectIndex;
    private int trait1SelectIndex;
    private int difficultySelectIndex;

    private void UpdatePage() 
    {
        headerMain.text = sdb.GetString("NEWRUN_MENU_HEADER_MAIN");
        traitCardPage0.SetUp(page0Traits[trait0SelectIndex]);
        traitCardPage1.SetUp(page1Traits[trait1SelectIndex]);
        difficultyCard.SetUp(difficultyLevels[difficultySelectIndex]);
        traitCardPage0.gameObject.SetActive(currentPage == 0);
        traitCardPage1.gameObject.SetActive(currentPage == 1);
        difficultyCard.gameObject.SetActive(currentPage == 2);
        switch (currentPage) 
        {
            case 0: 
                {
                    headerSub.text = sdb.GetString("NEWRUN_MENU_HEADER_0");
                    elementSelectedDesc.text = sdb.GetString(page0Traits[trait0SelectIndex].DescID);
                    break;
                }
            case 1:
                {
                    headerSub.text = sdb.GetString("NEWRUN_MENU_HEADER_1");
                    elementSelectedDesc.text = sdb.GetString(page1Traits[trait1SelectIndex].DescID);
                    break;
                }
            case 2: 
                {
                    headerSub.text = sdb.GetString("NEWRUN_MENU_HEADER_2");
                    elementSelectedDesc.text = sdb.GetString(difficultyLevels[difficultySelectIndex].DescID);
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
                    break;
                }
            case 1:
                {
                    currentPage = 2;
                    UpdatePage();
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
                    break;
                }
            case 2:
                {
                    currentPage = 1;
                    UpdatePage();
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
                    if (difficultySelectIndex >= difficultyLevels.Count) { difficultySelectIndex = 0; }
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
                    if (difficultySelectIndex < 0) { difficultySelectIndex = difficultyLevels.Count - 1; }
                    break;
                }
        }
        UpdatePage();
    }

    private void StartRun() 
    {
        List<PlayerTraitSO> traitsSelected = new List<PlayerTraitSO>();
        traitsSelected.Add(page0Traits[trait0SelectIndex]);
        traitsSelected.Add(page1Traits[trait1SelectIndex]);

        PersistentDataManager.SetGameDifficulty(difficultyLevels[difficultySelectIndex]);
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
    }
    public string GetMenuNameID()
    {
        return "MENU_NAME_NEW_RUN";
    }
}
