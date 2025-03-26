using UnityEngine;

public class HudCombatNotifications : MonoBehaviour
{
    [SerializeField] private GameObject damageNotificationPrefab;
    [SerializeField] private GameObject statusNotificationPrefab;

    private MonoBehaviourPool<HudDamageNotificationsElement> damageNotificationPool;
    private MonoBehaviourPool<HudStatusNotificationElement> statusNotificationPool;
    private Camera mcam;

    public void SetUp(Camera c) 
    {
        gameObject.SetActive(true);
        mcam = c;
    }
    private void OnEnable()
    {
        damageNotificationPool = new MonoBehaviourPool<HudDamageNotificationsElement>(damageNotificationPrefab, transform);
        statusNotificationPool = new MonoBehaviourPool<HudStatusNotificationElement>(statusNotificationPrefab, transform);
        EventManager.EnemyDirectDamageTakenEvent += OnEnemyDamageTaken;
        EventManager.EnemyStatusEffectAppliedEvent += OnEnemyStatusApplied;
    }
    private void OnDisable()
    {
        EventManager.EnemyDirectDamageTakenEvent -= OnEnemyDamageTaken;
        EventManager.EnemyStatusEffectAppliedEvent -= OnEnemyStatusApplied;
    }
    private void OnEnemyDamageTaken(float magnitude, int critlevel, GameEnums.DamageElement elem, EnemyEntity target) 
    {
        damageNotificationPool.GetCopyFromPool().SetUp(mcam, magnitude, critlevel, elem, target);
    }
    private void OnEnemyStatusApplied(GameEnums.DamageElement elem, EnemyEntity target) 
    {
        statusNotificationPool.GetCopyFromPool().SetUp(mcam, elem, target);
    }
}
