using UnityEngine;

public class HudCombatNotifications : MonoBehaviour
{
    [SerializeField] private GameObject directDamageNotificationPrefab;
    [SerializeField] private GameObject statusDamageNotificationPrefab;
    [SerializeField] private GameObject statusNotificationPrefab;

    private MonoBehaviourPool<HudDirectDamageNotificationElement> directDamageNotificationPool;
    private MonoBehaviourPool<HudStatusDamageNotificationElement> statusDamageNotificationPool;
    private MonoBehaviourPool<HudStatusNotificationElement> statusNotificationPool;
    private Camera mcam;

    public void SetUp(Camera c) 
    {
        gameObject.SetActive(true);
        mcam = c;
    }
    private void OnEnable()
    {
        directDamageNotificationPool = new MonoBehaviourPool<HudDirectDamageNotificationElement>(directDamageNotificationPrefab, transform);
        statusDamageNotificationPool = new MonoBehaviourPool<HudStatusDamageNotificationElement>(statusDamageNotificationPrefab, transform);
        statusNotificationPool = new MonoBehaviourPool<HudStatusNotificationElement>(statusNotificationPrefab, transform);

        EventManager.EnemyDirectDamageTakenEvent += OnEnemyDirectDamageTaken;
        EventManager.EnemyStatusEffectAppliedEvent += OnEnemyStatusApplied;
        EventManager.EnemyStatusDamageTakenEvent += OnEnemyStatusDamageTaken;
    }
    private void OnDisable()
    {
        EventManager.EnemyDirectDamageTakenEvent -= OnEnemyDirectDamageTaken;
        EventManager.EnemyStatusEffectAppliedEvent -= OnEnemyStatusApplied;
        EventManager.EnemyStatusDamageTakenEvent -= OnEnemyStatusDamageTaken;
    }
    private void OnEnemyStatusDamageTaken(float magnitude, GameEnums.DamageElement elem, EnemyEntity target) 
    {
        statusDamageNotificationPool.GetCopyFromPool().SetUp(mcam, magnitude, elem, target);
    }
    private void OnEnemyDirectDamageTaken(float magnitude, int critlevel, GameEnums.DamageElement elem, EnemyEntity target) 
    {
        directDamageNotificationPool.GetCopyFromPool().SetUp(mcam, magnitude, critlevel, elem, target);
    }
    private void OnEnemyStatusApplied(GameEnums.DamageElement elem, EnemyEntity target) 
    {
        statusNotificationPool.GetCopyFromPool().SetUp(mcam, elem, target);
    }
}
