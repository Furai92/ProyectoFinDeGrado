using UnityEngine;

public class HudCombatNotifications : MonoBehaviour
{
    [SerializeField] private GameObject coloredTextNotificationPrefab;
    [SerializeField] private GameObject combatWarningPrefab;
    [SerializeField] private ColorDatabaseSO cdb;
    [SerializeField] private StringDatabaseSO sdb;
    [SerializeField] private Camera mCamRef;

    private MonoBehaviourPool<HudColoredTextNotification> coloredTextNotificationPool;
    private MonoBehaviourPool<HudCombatWarningElement> combatWarningPool;
    private float sizeMult;

    private const float SETTINGS_TO_SIZE_MULT_MIN = 0.3f;
    private const float SETTINGS_TO_SIZE_MULT_MAX = 1f;

    private const float STATUS_DAMAGE_SIZE = 1f;
    private const float DIRECT_DAMAGE_SIZE = 1.5f;
    private const float STATUS_EFFECT_SIZE = 2f;
    private const float STATUS_DAMAGE_LIFETIME = 0.75f;
    private const float DIRECT_DAMAGE_LIFETIME = 1.0f;
    private const float STATUS_EFFECT_LIFETIME = 1.25f;
    private const float EVADED_SIZE = 1.5f;
    private const float EVADED_LIFETIME = 1f;

    private void OnEnable()
    {
        coloredTextNotificationPool = new MonoBehaviourPool<HudColoredTextNotification>(coloredTextNotificationPrefab, transform);
        combatWarningPool = new MonoBehaviourPool<HudCombatWarningElement>(combatWarningPrefab, transform);
        UpdateSizeMultiplier();

        EventManager.EnemyDirectDamageTakenEvent += OnEnemyDirectDamageTaken;
        EventManager.EnemyStatusEffectAppliedEvent += OnEnemyStatusApplied;
        EventManager.EnemyStatusDamageTakenEvent += OnEnemyStatusDamageTaken;
        EventManager.CombatWarningDisplayedEvent += OnCombatWarningDisplayed;
        EventManager.PlayerEvasionEvent += OnEnemyAttackEvaded;
        EventManager.GameSettingsChangedEvent += UpdateSizeMultiplier;
    }
    private void OnDisable()
    {
        EventManager.EnemyDirectDamageTakenEvent -= OnEnemyDirectDamageTaken;
        EventManager.EnemyStatusEffectAppliedEvent -= OnEnemyStatusApplied;
        EventManager.EnemyStatusDamageTakenEvent -= OnEnemyStatusDamageTaken;
        EventManager.CombatWarningDisplayedEvent -= OnCombatWarningDisplayed;
        EventManager.PlayerEvasionEvent -= OnEnemyAttackEvaded;
        EventManager.GameSettingsChangedEvent -= UpdateSizeMultiplier;
    }
    private void UpdateSizeMultiplier() 
    {
        sizeMult = Mathf.Lerp(SETTINGS_TO_SIZE_MULT_MIN, SETTINGS_TO_SIZE_MULT_MAX, GameSettingsManager.GetSetting(GameSettingsManager.GameSetting.DamageNotificationSize));
    }
    private void OnEnemyStatusDamageTaken(float magnitude, GameEnums.DamageElement elem, EnemyEntity target) 
    {
        coloredTextNotificationPool.GetCopyFromPool().SetUp(mCamRef, magnitude.ToString("F0"), cdb.ElementToColor(elem), STATUS_DAMAGE_SIZE * sizeMult, STATUS_DAMAGE_LIFETIME, true, target.transform.position);
    }
    private void OnEnemyDirectDamageTaken(float magnitude, int critlevel, GameEnums.DamageElement elem, EnemyEntity target) 
    {
        string damageText = magnitude.ToString("F0");
        for (int i = 0; i < critlevel; i++) { damageText += "!"; }
        coloredTextNotificationPool.GetCopyFromPool().SetUp(mCamRef, damageText, cdb.ElementToColor(elem), DIRECT_DAMAGE_SIZE * sizeMult, DIRECT_DAMAGE_LIFETIME, true, target.transform.position);
    }
    private void OnEnemyStatusApplied(GameEnums.DamageElement elem, EnemyEntity target) 
    {
        coloredTextNotificationPool.GetCopyFromPool().SetUp(mCamRef, sdb.ElementToStatusName(elem), cdb.ElementToColor(elem), STATUS_EFFECT_SIZE * sizeMult, STATUS_EFFECT_LIFETIME, true, target.transform.position);
    }
    private void OnEnemyAttackEvaded(Vector3 wpos)
    {
        coloredTextNotificationPool.GetCopyFromPool().SetUp(mCamRef, "Evaded!", Color.white, EVADED_SIZE * sizeMult, EVADED_LIFETIME, false, wpos);
    }
    private void OnCombatWarningDisplayed(HudCombatWarningElement.WarningType wt, Vector3 pos) 
    {
        combatWarningPool.GetCopyFromPool().SetUp(mCamRef, pos, wt);
    }
}
