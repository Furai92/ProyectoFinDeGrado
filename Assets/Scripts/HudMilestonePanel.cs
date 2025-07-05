using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudMilestonePanel : MonoBehaviour
{
    [SerializeField] private Transform progressBackground;
    [SerializeField] private TextMeshProUGUI milestoneName;
    [SerializeField] private TextMeshProUGUI milestoneDesc;
    [SerializeField] private TextMeshProUGUI milestoneProgress;
    [SerializeField] private StringDatabaseSO sdb;


    public void SetUp(MilestoneSO m) 
    {
        milestoneName.text = sdb.GetString(m.NameID);
        milestoneDesc.text = sdb.GetString(m.DescID);
        int mprog = Mathf.Min(PersistentDataManager.GetGameProgress(m.GameProgressCheckID), m.GameProgressCheckReq);
        milestoneProgress.text = string.Format("{0}/{1}", mprog, m.GameProgressCheckReq);

        float progressPrecent = Mathf.Clamp01((float)mprog / m.GameProgressCheckReq);
        progressBackground.transform.localScale = new Vector3(progressPrecent, 1, 1);
    }
}
