using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class HudPreviousRunRecapMenu : MonoBehaviour, IGameMenu
{
    [SerializeField] private StringDatabaseSO sdb;
    [SerializeField] private ColorDatabaseSO cdb;
    [SerializeField] private Transform menuParent;
    [SerializeField] private TextMeshProUGUI runResultText;
    [SerializeField] private List<TextMeshProUGUI> typeLabels;
    [SerializeField] private List<TextMeshProUGUI> directNumbers;
    [SerializeField] private List<TextMeshProUGUI> statusNumbers;

    public bool CanBeOpened()
    {
        return true;
    }

    public void OnContinueClicked() 
    {
        CloseMenu();
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
        UpdateDisplayedInfo();
        PersistentDataManager.DeletePreviousStageRecord();
        menuParent.gameObject.SetActive(true);
    }
    private void UpdateDisplayedInfo()
    {
        StageStatsTracker.StageRecord record = PersistentDataManager.GetPreviousStageRecord();

        runResultText.text = record.victory ? sdb.GetString("RUN_RESULT_VICTORY") : sdb.GetString("RUN_RESULT_DEFEAT");
        runResultText.color = record.victory ? Color.green : Color.red;

        for (int i = 0; i < typeLabels.Count; i++) 
        {
            GameEnums.DamageElement e = (GameEnums.DamageElement)i;
            typeLabels[i].text = sdb.ElementToName(e);
            directNumbers[i].text = record.directDamageRecords[i].ToString("F0");
            statusNumbers[i].text = record.statusDamageRecords[i].ToString("F0");
            typeLabels[i].color = directNumbers[i].color = statusNumbers[i].color = cdb.ElementToColor(e);
        }
    }
}
