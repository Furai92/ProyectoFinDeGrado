using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HudBossHealthBarElement : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private Image filltrail;
    [SerializeField] private List<GameObject> extraBarIcons;

    public EnemyEntity TrackedEnemy { get; private set; }

    private const float FILLTRAIL_SPEED = 0.075f;
    private const float FILLTRAIL_SPEED_UPWARDS = 100f;

    public void SetUp(EnemyEntity e)
    {
        TrackedEnemy = e;
        gameObject.SetActive(true);
        UpdateFill();
        filltrail.fillAmount = fill.fillAmount;

        EventManager.EnemyDirectDamageTakenEvent += OnEnemyDirectDamageTaken;
        EventManager.EnemyStatusDamageTakenEvent += OnEnemyStatusDamageTaken;
        EventManager.EnemyDisabledEvent += OnEnemyDisabled;
    }
    private void OnDisable()
    {
        EventManager.EnemyDirectDamageTakenEvent -= OnEnemyDirectDamageTaken;
        EventManager.EnemyStatusDamageTakenEvent -= OnEnemyStatusDamageTaken;
        EventManager.EnemyDisabledEvent -= OnEnemyDisabled;
    }
    private void OnEnemyStatusDamageTaken(float magnitude, GameEnums.DamageElement elem, EnemyEntity e)
    {
        if (e == TrackedEnemy) { UpdateFill(); }
    }
    private void OnEnemyDirectDamageTaken(float magnitude, int critlevel, GameEnums.DamageElement elem, GameEnums.DamageType dtype, EnemyEntity e)
    {
        if (e == TrackedEnemy) { UpdateFill(); }
    }
    private void OnEnemyDisabled(EnemyEntity e, float overkill, GameEnums.EnemyRank rank, bool killcredit)
    {
        if (e == TrackedEnemy) { gameObject.SetActive(false); }
    }
    private void Update()
    {
        for (int i = 0; i < extraBarIcons.Count; i++)
        {
            extraBarIcons[i].gameObject.SetActive(TrackedEnemy.RemainingExtraBars > i);
        }
        float fillMoveSpeed = filltrail.fillAmount > fill.fillAmount ? FILLTRAIL_SPEED : FILLTRAIL_SPEED_UPWARDS;
        filltrail.fillAmount = Mathf.MoveTowards(filltrail.fillAmount, fill.fillAmount, Time.deltaTime * fillMoveSpeed);
    }
    private void UpdateFill()
    {
        fill.fillAmount = TrackedEnemy.GetHealthPercent();
    }
}
