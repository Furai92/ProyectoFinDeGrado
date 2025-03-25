using UnityEngine;
using UnityEngine.UI;

public class HudEnemyHealthBarElement : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private Image filltrail;

    public EnemyEntity TrackedEnemy { get; private set; }

    private float removeTime;
    private Camera mcam;

    private const float LIFETIME = 4f;
    private const float FILLTRAIL_SPEED = 1f;
    private const float BAR_WPOS_VERTICAL_OFFSET = 2f;

    public void SetUp(EnemyEntity e, Camera c) 
    {
        mcam = c;
        TrackedEnemy = e;
        gameObject.SetActive(true);
        UpdateFill();
        filltrail.fillAmount = fill.fillAmount;

        EventManager.EnemyDirectDamageTakenEvent += OnEnemyDirectDamageTaken;
        EventManager.EnemyDefeatedEvent += OnEnemyDefeated;
        EventManager.EnemyDisabledEvent += OnEnemyDisabled;
    }
    private void OnDisable()
    {
        EventManager.EnemyDirectDamageTakenEvent -= OnEnemyDirectDamageTaken;
        EventManager.EnemyDefeatedEvent -= OnEnemyDefeated;
        EventManager.EnemyDisabledEvent -= OnEnemyDisabled;
        EventManager.OnUiEnemyHealthBarDisabled(this);
    }
    private void OnEnemyDirectDamageTaken(float magnitude, int critlevel, GameEnums.DamageElement elem, EnemyEntity e) 
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
        filltrail.fillAmount = Mathf.MoveTowards(filltrail.fillAmount, fill.fillAmount, Time.deltaTime * FILLTRAIL_SPEED);
        transform.position = mcam.WorldToScreenPoint(TrackedEnemy.transform.position + Vector3.up * BAR_WPOS_VERTICAL_OFFSET);
        if (transform.position.z < 0) { gameObject.SetActive(false); }
        if (Time.time > removeTime) { gameObject.SetActive(false); }
    }
    private void UpdateFill() 
    {
        fill.fillAmount = TrackedEnemy.GetHealthPercent();
        removeTime = Time.time + LIFETIME;
    }
}
