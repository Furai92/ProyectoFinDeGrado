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
    [SerializeField] private List<HudPreviousRunRecapDamageElement> damageRecapElements;

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

        switch (record.result) 
        {
            case StageStatsTracker.StageRecord.Result.Victory:
                {
                    runResultText.text = sdb.GetString("RUN_RESULT_VICTORY");
                    runResultText.color = Color.green;
                    break;
                }
            case StageStatsTracker.StageRecord.Result.Defeat:
                {
                    runResultText.text = sdb.GetString("RUN_RESULT_DEFEAT");
                    runResultText.color = Color.red;
                    break;
                }
            case StageStatsTracker.StageRecord.Result.Abandoned:
                {
                    runResultText.text = sdb.GetString("RUN_RESULT_ABANDON");
                    runResultText.color = Color.white;
                    break;
                }
        }


        for (int i = 0; i < damageRecapElements.Count; i++) 
        {
            GameEnums.DamageElement e = (GameEnums.DamageElement)i;
            damageRecapElements[i].SetUp(e, record.directDamageRecords[i], record.statusDamageRecords[i]);
        }
    }
    public string GetMenuNameID()
    {
        return "MENU_NAME_RECAP";
    }
}
