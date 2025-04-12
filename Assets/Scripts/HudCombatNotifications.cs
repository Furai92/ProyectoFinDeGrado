using UnityEngine;

public class HudCombatNotifications : MonoBehaviour
{
    [SerializeField] private GameObject coloredTextNotificationPrefab;
    [SerializeField] private GameObject combatWarningPrefab;
    [SerializeField] private ColorDatabaseSO cdb;
    [SerializeField] private StringDatabaseSO sdb;

    private MonoBehaviourPool<HudColoredTextNotification> coloredTextNotificationPool;
    private MonoBehaviourPool<HudCombatWarningElement> combatWarningPool;
    private Camera mcam;

    private const float STATUS_DAMAGE_SIZE = 1f;
    private const float DIRECT_DAMAGE_SIZE = 1.5f;
    private const float STATUS_EFFECT_SIZE = 2f;
    private const float STATUS_DAMAGE_LIFETIME = 0.75f;
    private const float DIRECT_DAMAGE_LIFETIME = 1.0f;
    private const float STATUS_EFFECT_LIFETIME = 1.25f;
    private const float EVADED_SIZE = 1.5f;
    private const float EVADED_LIFETIME = 1f;

    public void SetUp(Camera c) 
    {
        gameObject.SetActive(true);
        mcam = c;
    }
    private void OnEnable()
    {
        coloredTextNotificationPool = new MonoBehaviourPool<HudColoredTextNotification>(coloredTextNotificationPrefab, transform);
        combatWarningPool = new MonoBehaviourPool<HudCombatWarningElement>(combatWarningPrefab, transform);

        EventManager.EnemyDirectDamageTakenEvent += OnEnemyDirectDamageTaken;
        EventManager.EnemyStatusEffectAppliedEvent += OnEnemyStatusApplied;
        EventManager.EnemyStatusDamageTakenEvent += OnEnemyStatusDamageTaken;
        EventManager.CombatWarningDisplayedEvent += OnCombatWarningDisplayed;
        EventManager.PlayerEvasionEvent += OnEnemyAttackEvaded;
    }
    private void OnDisable()
    {
        EventManager.EnemyDirectDamageTakenEvent -= OnEnemyDirectDamageTaken;
        EventManager.EnemyStatusEffectAppliedEvent -= OnEnemyStatusApplied;
        EventManager.EnemyStatusDamageTakenEvent -= OnEnemyStatusDamageTaken;
        EventManager.CombatWarningDisplayedEvent -= OnCombatWarningDisplayed;
        EventManager.PlayerEvasionEvent -= OnEnemyAttackEvaded;
    }
    private void OnEnemyStatusDamageTaken(float magnitude, GameEnums.DamageElement elem, EnemyEntity target) 
    {
        coloredTextNotificationPool.GetCopyFromPool().SetUp(mcam, magnitude.ToString("F0"), cdb.ElementToColor(elem), STATUS_DAMAGE_SIZE, STATUS_DAMAGE_LIFETIME, true, target.transform.position);
    }
    private void OnEnemyDirectDamageTaken(float magnitude, int critlevel, GameEnums.DamageElement elem, EnemyEntity target) 
    {
        coloredTextNotificationPool.GetCopyFromPool().SetUp(mcam, magnitude.ToString("F0"), cdb.ElementToColor(elem), DIRECT_DAMAGE_SIZE, DIRECT_DAMAGE_LIFETIME, true, target.transform.position);
    }
    private void OnEnemyStatusApplied(GameEnums.DamageElement elem, EnemyEntity target) 
    {
        coloredTextNotificationPool.GetCopyFromPool().SetUp(mcam, sdb.StatusToString(elem), cdb.ElementToColor(elem), STATUS_EFFECT_SIZE, STATUS_EFFECT_LIFETIME, true, target.transform.position);
    }
    private void OnEnemyAttackEvaded(Vector3 wpos)
    {
        coloredTextNotificationPool.GetCopyFromPool().SetUp(mcam, "Evaded!", Color.white, EVADED_SIZE, EVADED_LIFETIME, false, wpos);
    }
    private void OnCombatWarningDisplayed(HudCombatWarningElement.WarningType wt, Vector3 pos) 
    {
        combatWarningPool.GetCopyFromPool().SetUp(mcam, pos, wt);
    }
}
