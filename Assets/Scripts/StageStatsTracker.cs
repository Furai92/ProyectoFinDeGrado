using UnityEngine;

public class StageStatsTracker : MonoBehaviour
{
    private StageRecord record;

    private void OnEnable()
    {
        record = new StageRecord();

        EventManager.EnemyDirectDamageTakenEvent += OnEnemyDirectDamageTaken;
        EventManager.EnemyStatusDamageTakenEvent += OnEnemyStatusDamageTaken;
        EventManager.StageStateEndedEvent += OnStageStateStarted;
        EventManager.PlayerDefeatedEvent += OnPlayerDefeated;
        EventManager.CurrencyUpdateEvent += OnCurrencyUpdated;
        EventManager.EnemyDisabledEvent += OnEnemyDefeated;
        EventManager.ChestOpenedEvent += OnChestOpened;
    }
    private void OnDisable()
    {
        EventManager.EnemyDirectDamageTakenEvent -= OnEnemyDirectDamageTaken;
        EventManager.EnemyStatusDamageTakenEvent -= OnEnemyStatusDamageTaken;
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
        EventManager.PlayerDefeatedEvent -= OnPlayerDefeated;
        EventManager.CurrencyUpdateEvent -= OnCurrencyUpdated;
        EventManager.EnemyDisabledEvent -= OnEnemyDefeated;
        EventManager.ChestOpenedEvent -= OnChestOpened;
    }
    private void OnEnemyDirectDamageTaken(float mag, int clevel, GameEnums.DamageElement elem, GameEnums.DamageType dtype, EnemyEntity target) 
    {
        record.directDamageRecords[(int)elem] += mag;
    }
    private void OnEnemyStatusDamageTaken(float mag, GameEnums.DamageElement elem, EnemyEntity target) 
    {
        record.statusDamageRecords[(int)elem] += mag;
    }
    private void OnStageStateStarted(StageStateBase.GameState s) 
    {
        if (s == StageStateBase.GameState.Victory) 
        {
            record.victory = true;
            PersistentDataManager.AddStageRecord(record);
            string difficultyProgressID = string.Format("WINS_CHAOS_{0}", PersistentDataManager.GetCurrentDifficulty().DifficultyIndex);
            PersistentDataManager.AddGameProgress(difficultyProgressID, 1);
        }
    }
    private void OnCurrencyUpdated(int change) 
    {
        if (change > 0) { PersistentDataManager.AddGameProgress("CREDITS_COLLECTED", change); }
    }
    private void OnEnemyDefeated(EnemyEntity target, float overkill, GameEnums.EnemyRank r, bool killcredit) 
    {
        if (killcredit) { PersistentDataManager.AddGameProgress("ENEMY_KILLS", 1); }
    }
    private void OnChestOpened() 
    {
        PersistentDataManager.AddGameProgress("CHESTS_OPEN", 1);
    }
    private void OnPlayerDefeated() 
    {
        record.victory = false;
        PersistentDataManager.AddStageRecord(record);
    }
    public class StageRecord
    {
        public bool victory;
        public float[] directDamageRecords;
        public float[] statusDamageRecords;

        public StageRecord() 
        {
            directDamageRecords = new float[GameEnums.DAMAGE_ELEMENTS];
            statusDamageRecords = new float[GameEnums.DAMAGE_ELEMENTS];
            victory = false;
        }
    }
}
