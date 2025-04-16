using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HudDashBar : MonoBehaviour
{
    [SerializeField] private List<Image> barParents;
    [SerializeField] private List<Image> barFills;
    [SerializeField] private Image fillingDashBar;


    private PlayerEntity playerReference;

    public void SetUp(PlayerEntity p)
    {
        playerReference = p;
        gameObject.SetActive(true);
        EventManager.PlayerStatsUpdatedEvent += OnPlayerStatsUpdated;
        UpdateActiveBars((int)p.GetStat(PlayerStatGroup.Stat.DashCount));
    }
    private void OnDisable()
    {
        EventManager.PlayerStatsUpdatedEvent -= OnPlayerStatsUpdated;
    }
    private void OnPlayerStatsUpdated(PlayerEntity p) 
    {
        UpdateActiveBars((int)p.GetStat(PlayerStatGroup.Stat.DashCount));
    }
    private void UpdateActiveBars(int maxbars) 
    {
        for (int i = 0; i < barParents.Count; i++)
        {
            barParents[i].gameObject.SetActive(i < maxbars);
        }
    }
    private void Update()
    {
        int currentDashes = playerReference.CurrentDashes;

        for (int i = 0; i < barFills.Count; i++) 
        {
            barFills[i].gameObject.SetActive(i < currentDashes);
        }
        fillingDashBar.fillAmount = playerReference.DashRechargePercent;

        if (currentDashes < barParents.Count)
        {
            fillingDashBar.transform.position = barParents[currentDashes].transform.position;
            fillingDashBar.gameObject.SetActive(true);
        }
        else 
        {
            fillingDashBar.gameObject.SetActive(false);
        }

    }
}
