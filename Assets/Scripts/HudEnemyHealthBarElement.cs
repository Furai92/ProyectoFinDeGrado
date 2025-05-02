using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HudEnemyHealthBarElement : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private Image filltrail;
    [SerializeField] private List<GameObject> extraBarIcons;

    public EnemyEntity TrackedEnemy { get; private set; }

    private float removeTime;
    private Camera mcam;

    private const float MAXIMUM_SIZE = 1f;
    private const float DISTANCE_TO_SIZE_REDUCTION = 0.2f;
    private const float LIFETIME = 4f;
    private const float FILLTRAIL_SPEED = 0.5f;
    private const float BAR_WPOS_VERTICAL_OFFSET = 2f;

    public void SetUp(EnemyEntity e, Camera c) 
    {
        mcam = c;
        TrackedEnemy = e;
        gameObject.SetActive(true);
        UpdateFill();
        filltrail.fillAmount = fill.fillAmount;

        EventManager.EnemyDirectDamageTakenEvent += OnEnemyDirectDamageTaken;
        EventManager.EnemyStatusDamageTakenEvent += OnEnemyStatusDamageTaken;
        EventManager.EnemyDefeatedEvent += OnEnemyDefeated;
        EventManager.EnemyDisabledEvent += OnEnemyDisabled;
    }
    private void OnDisable()
    {
        EventManager.EnemyDirectDamageTakenEvent -= OnEnemyDirectDamageTaken;
        EventManager.EnemyStatusDamageTakenEvent -= OnEnemyStatusDamageTaken;
        EventManager.EnemyDefeatedEvent -= OnEnemyDefeated;
        EventManager.EnemyDisabledEvent -= OnEnemyDisabled;
        EventManager.OnUiEnemyHealthBarDisabled(this);
    }
    private void OnEnemyStatusDamageTaken(float magnitude, GameEnums.DamageElement elem, EnemyEntity e)
    {
        if (e == TrackedEnemy) { UpdateFill(); }
    }
    private void OnEnemyDirectDamageTaken(float magnitude, int critlevel, GameEnums.DamageElement elem, GameEnums.DamageType dtype, EnemyEntity e) 
    {
        if (e == TrackedEnemy) { UpdateFill(); }
    }
    private void OnEnemyDefeated(EnemyEntity e)
    {
        if (e == TrackedEnemy) { gameObject.SetActive(false); }
    }
    private void OnEnemyDisabled(EnemyEntity e)
    {
        if (e == TrackedEnemy) { gameObject.SetActive(false); }
    }
    private void Update()
    {
        for (int i = 0; i < extraBarIcons.Count; i++) 
        {
            extraBarIcons[i].gameObject.SetActive(TrackedEnemy.RemainingExtraBars > i);
        }
        filltrail.fillAmount = Mathf.MoveTowards(filltrail.fillAmount, fill.fillAmount, Time.deltaTime * FILLTRAIL_SPEED);
        transform.position = mcam.WorldToScreenPoint(TrackedEnemy.transform.position + Vector3.up * BAR_WPOS_VERTICAL_OFFSET);
        if (transform.position.z <= 0) { gameObject.SetActive(false); }
        transform.localScale = Mathf.Min(1/(transform.position.z * DISTANCE_TO_SIZE_REDUCTION), MAXIMUM_SIZE) * Vector3.one;
        if (Time.time > removeTime) { gameObject.SetActive(false); }
    }
    private void UpdateFill() 
    {
        fill.fillAmount = TrackedEnemy.GetHealthPercent();
        removeTime = Time.time + LIFETIME;
    }
}
