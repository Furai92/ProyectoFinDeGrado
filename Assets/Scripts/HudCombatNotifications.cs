using UnityEngine;

public class HudCombatNotifications : MonoBehaviour
{
    [SerializeField] private GameObject directDamageNotificationPrefab;
    [SerializeField] private GameObject statusDamageNotificationPrefab;
    [SerializeField] private GameObject statusNotificationPrefab;
    [SerializeField] private GameObject combatWarningPrefab;

    private MonoBehaviourPool<HudDirectDamageNotificationElement> directDamageNotificationPool;
    private MonoBehaviourPool<HudStatusDamageNotificationElement> statusDamageNotificationPool;
    private MonoBehaviourPool<HudStatusNotificationElement> statusNotificationPool;
    private MonoBehaviourPool<HudCombatWarningElement> combatWarningPool;
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
        combatWarningPool = new MonoBehaviourPool<HudCombatWarningElement>(combatWarningPrefab, transform);

        EventManager.EnemyDirectDamageTakenEvent += OnEnemyDirectDamageTaken;
        EventManager.EnemyStatusEffectAppliedEvent += OnEnemyStatusApplied;
        EventManager.EnemyStatusDamageTakenEvent += OnEnemyStatusDamageTaken;
        EventManager.CombatWarningDisplayedEvent += OnCombatWarningDisplayed;
    }
    private void OnDisable()
    {
        EventManager.EnemyDirectDamageTakenEvent -= OnEnemyDirectDamageTaken;
        EventManager.EnemyStatusEffectAppliedEvent -= OnEnemyStatusApplied;
        EventManager.EnemyStatusDamageTakenEvent -= OnEnemyStatusDamageTaken;
        EventManager.CombatWarningDisplayedEvent += OnCombatWarningDisplayed;
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
    private void OnCombatWarningDisplayed(HudCombatWarningElement.WarningType wt, Vector3 pos) 
    {
        combatWarningPool.GetCopyFromPool().SetUp(mcam, pos, wt);
    }
}
