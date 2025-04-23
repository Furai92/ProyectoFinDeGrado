using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private ProceduralStagePropertiesSO backgroundStageProperties;
    [SerializeField] private Transform backgroundStageParent;
    [SerializeField] private MainMenuTravellingCamera travellingCam;
    [SerializeField] private HudSettingsMenu settingsMenu;
    [SerializeField] private Transform mainStateParent;

    private ProceduralStageData backgroundStage;

    private IGameMenu activeMenu;

    void OnEnable()
    {
        backgroundStage = new ProceduralStageData(Random.Range(0, 9999999), 30, backgroundStageProperties, backgroundStageParent);
        travellingCam.SetUp(backgroundStage);

        EventManager.UiMenuClosedEvent += OnMenuClosed;
    }
    private void OnDisable()
    {
        EventManager.UiMenuClosedEvent -= OnMenuClosed;
    }
    private void OnMenuClosed(IGameMenu m) 
    {
        if (activeMenu == m) { activeMenu = null; }

        UpdateFocus();
    }
    private void ToggleMenu(IGameMenu m) 
    {
        if (m == activeMenu)
        {
            activeMenu = null;
            activeMenu.CloseMenu();
        }
        else 
        {
            if (activeMenu != null) { activeMenu.CloseMenu(); }
            m.OpenMenu();
            activeMenu = m;
        }

        UpdateFocus();
    }
    private void UpdateFocus() 
    {
        mainStateParent.gameObject.SetActive(activeMenu == null);
    }


    public void OnStartRunClicked() 
    {
        // TEMP
        SceneManager.LoadScene("Ingame");
    }
    public void OnCollectionClicked() 
    {

    }
    public void OnSettingsClicked() 
    {
        ToggleMenu(settingsMenu);
    }
    public void OnQuitClicked() 
    {
        Application.Quit(0);
    }

}
