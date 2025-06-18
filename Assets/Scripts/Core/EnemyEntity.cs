using UnityEngine;
using System.Collections.Generic;

public class EnemyEntity : MonoBehaviour
{
    [field: SerializeField] public string ID { get; private set; }

    // Stats ===================================================

    [field: SerializeField] public float MovementSpeed { get; private set; }
    [field: SerializeField] public float BaseHealth { get; private set; }
    [field: SerializeField] public int ExtraBars { get; private set; }
    [field: SerializeField] public float KnockbackResistance { get; private set; }
    [field: SerializeField] public GameEnums.EnemyRank Rank { get; private set; }

    // Rewards =================================================

    [field: SerializeField] public float ItemDropRate { get; private set; }
    [field: SerializeField] public float CurrencyDropRate { get; private set; }
    [field: SerializeField] public float HealthDroprate { get; private set; }

    // Status ==================================================

    public int RemainingExtraBars { get; private set; }

    private float maxHealth;
    public float CurrentHealth { get; private set; }
    private float[] statusBuildups;
    private float[] statusBuildupResistancesDivider;
    private float[] statusDurations;
    private float nextStatusUpdateTime;

    private const float INCINERATE_STATUS_HEALTH_PERCENT_DAMAGE = 1.5f;
    private const float FIRE_STATUS_HEALTH_PERCENT_DAMAGE = 0.8f;
    private const float SHOCK_STATUS_HEALTH_PERCENT_DAMAGE = 0.5f;
    private const float SHOCK_STATUS_BONUS_CRIT_CHANCE = 50f;
    private const float FROST_STATUS_HEALTH_PERCENT_DAMAGE = 0.2f;
    private const float FROSTBITE_STATUS_HEALTH_PERCENT_DAMAGE = 0.2f;
    private const float FROSTBITE_STATUS_EFFECT_MULTIPLIER = 1.6f;
    private const float VOID_STATUS_HEALTH_PERCENT_DAMAGE = 0.5f;
    private const float VOID_STATUS_RADIUS_BASE = 5f;
    private const float VOID_STATUS_RADIUS_SCALING = 0.003f;
    public const float STATUS_DURATION_STANDARD = 10f;
    private const float STATUS_UPDATE_INTERVAL = 1f;
    private const float STATUS_RESISTANCE_GROWTH_MULTIPLIER = 1.5f;
    private const float HEALTH_PERCENT_REQUIRED_TO_FULL_BUILDUP = 0.4f;

    // AI Management ===========================================

    [SerializeField] private EnemyAiSO AiFile;
    [field: SerializeField] public float MaxCombatDistance { get; private set; }
    [field: SerializeField] public bool RequiresLosForCombat { get; private set; }

    
    private AiStateBase pathfindingSearchState;
    private AiStateBase currentState;
    private AiStateBase combatRootState;
    private float outOfCombatSpeedMult;
    private bool inCombat;

    // Movement and Physics ================================================

    [SerializeField] private Rigidbody rb;
    public Vector3 TargetMovementPosition { get; set; }
    public Vector3 TargetLookPosition { get; set; }

    private float difficultySpeedMult;
    private Vector3 currentKnockbackForce;
    private float currentLookRotation;
    private List<PullForce> activePullForces;

    private const float ELITE_HP_MULT = 2f;
    private const int ELITE_EXTRA_BARS = 3;

    private const float OUT_OF_COMBAT_SPEED_CAP = 2f;
    private const float OUT_OF_COMBAT_SPEED_RAMP_UP = 0.15f; 
    private const float MIN_MOVEMENT_DISTANCE = 0.15f;
    private const float KNOCKBACK_FORCE_DECAY_LINEAR = 0.2f;
    private const float KNOCKBACK_FORCE_DECAY_MULTIPLICATIVE = 0.98f;
    private const float KNOCKBACK_FORCE_MAG_CLAMP = 50f;
    private const float VISUAL_ROTATION_SPEED = 300f;

    // Other/Cached variables

    public int EnemyInstanceID { get; private set; }
    private LayerMask LoSMask;

    // References
    [SerializeField] private Transform rotationParent;


    public void SetUp(Vector3 pos) 
    {
        transform.position = pos;
        gameObject.SetActive(true);
        EventManager.OnEnemySpawned(this);
    }
    private void OnEnable()
    {
        LoSMask = LayerMask.GetMask("Walls");
        ResetEntity();
        SetupAi();
        EnemyInstanceID = StageManagerBase.RegisterEnemy(this);

        EventManager.StageStateEndedEvent += OnStageStateEnded;
    }
    private void ResetEntity() 
    {
        if (Rank == GameEnums.EnemyRank.Normal && Random.Range(0, 101) < StageManagerBase.GetStageStat(StageStatGroup.StageStat.EliteChance)) { Rank = GameEnums.EnemyRank.Elite; }

        TargetMovementPosition = transform.position;
        currentLookRotation = 0;
        currentKnockbackForce = Vector3.zero;
        CurrentHealth = maxHealth = BaseHealth * StageManagerBase.GetStageStat(StageStatGroup.StageStat.EnemyHealthMult) 
            * (Rank == GameEnums.EnemyRank.Elite ? ELITE_HP_MULT : 1) * (Rank == GameEnums.EnemyRank.Boss ? StageManagerBase.GetStageStat(StageStatGroup.StageStat.BossHealthMult) : 1);
        RemainingExtraBars = ExtraBars + (int)StageManagerBase.GetStageStat(StageStatGroup.StageStat.EnemyExtraHealthBarCount) + (Rank == GameEnums.EnemyRank.Elite ? ELITE_EXTRA_BARS : 0);
        statusBuildups = new float[GameEnums.DAMAGE_ELEMENTS];
        statusDurations = new float[GameEnums.DAMAGE_ELEMENTS];
        statusBuildupResistancesDivider = new float[GameEnums.DAMAGE_ELEMENTS];
        activePullForces = new List<PullForce>();
        for (int i = 0; i < statusBuildupResistancesDivider.Length; i++) { statusBuildupResistancesDivider[i] = 1; }
        nextStatusUpdateTime = Time.time + STATUS_UPDATE_INTERVAL;
        outOfCombatSpeedMult = 1;
        difficultySpeedMult = StageManagerBase.GetStageStat(StageStatGroup.StageStat.EnemySpeedMult);

    }
    private void OnDisable()
    {
        EventManager.OnEnemyDisabled(this, -CurrentHealth, Rank, CurrentHealth <= 0);
        StageManagerBase.UnregisterEnemy(this);

        EventManager.StageStateEndedEvent -= OnStageStateEnded;
    }
    private void FixedUpdate()
    {
        UpdateAI();
        UpdateRotation();
        UpdateMovement();
        UpdateStatusEffects();
    }
    private void OnStageStateEnded(StageStateBase.GameState s)
    {
        gameObject.SetActive(false);
    }
    private void SetupAi() 
    {
        pathfindingSearchState = new AiStatePathfindingSearch();
        combatRootState = AiFile.GenerateAiTree();
        if (IsInCombatRange()) 
        {
            currentState = combatRootState;
            inCombat = true;
        }
        else 
        {
            currentState = pathfindingSearchState;
            inCombat = false;
        }
        currentState.StartState(this);
    }
    private void OnEntityStunned()
    {
        currentState.EndState(this);
        currentState = combatRootState;
        currentState.StartState(this);
        outOfCombatSpeedMult = 1;
    }
    private void UpdateAI() 
    {
        if (statusDurations[(int)GameEnums.DamageElement.Frost] > 0) { return; } // Pause if frozen
        if (!inCombat) { outOfCombatSpeedMult = Mathf.MoveTowards(outOfCombatSpeedMult, OUT_OF_COMBAT_SPEED_CAP, OUT_OF_COMBAT_SPEED_RAMP_UP * Time.fixedDeltaTime);  }

        currentState.FixedUpdateState(this);
        if (currentState.IsFinished(this)) 
        {
            currentState.EndState(this);

            if (IsInCombatRange())
            {
                currentState = inCombat ? currentState.GetNextState(this) : combatRootState;
                inCombat = true;
            }
            else
            {
                if (inCombat) { outOfCombatSpeedMult = 1; }
                currentState = pathfindingSearchState;
                inCombat = false;
            }
            currentState.StartState(this);
        }
    }
    private void UpdateStatusEffects() 
    {
        if (Time.time < nextStatusUpdateTime) { return; }

        nextStatusUpdateTime = Time.time + STATUS_UPDATE_INTERVAL;

        if (statusDurations[(int)GameEnums.DamageElement.Fire] > 0)
        {
            DealStatusDamage(maxHealth * FIRE_STATUS_HEALTH_PERCENT_DAMAGE / STATUS_DURATION_STANDARD / STATUS_UPDATE_INTERVAL, GameEnums.DamageElement.Fire);
        }
        if (statusDurations[(int)GameEnums.DamageElement.Incineration] > 0)
        {
            DealStatusDamage(maxHealth * INCINERATE_STATUS_HEALTH_PERCENT_DAMAGE / STATUS_DURATION_STANDARD / STATUS_UPDATE_INTERVAL, GameEnums.DamageElement.Incineration);
        }
        if (statusDurations[(int)GameEnums.DamageElement.Frostbite] > 0)
        {
            DealStatusDamage(maxHealth * FROSTBITE_STATUS_HEALTH_PERCENT_DAMAGE / STATUS_DURATION_STANDARD / STATUS_UPDATE_INTERVAL, GameEnums.DamageElement.Frostbite);
        }
        if (statusDurations[(int)GameEnums.DamageElement.Frost] > 0)
        {
            DealStatusDamage(maxHealth * FROST_STATUS_HEALTH_PERCENT_DAMAGE / STATUS_DURATION_STANDARD / STATUS_UPDATE_INTERVAL, GameEnums.DamageElement.Frost);
        }
        if (statusDurations[(int)GameEnums.DamageElement.Thunder] > 0)
        {
            DealStatusDamage(maxHealth * SHOCK_STATUS_HEALTH_PERCENT_DAMAGE / STATUS_DURATION_STANDARD / STATUS_UPDATE_INTERVAL, GameEnums.DamageElement.Thunder);
        }
        if (statusDurations[(int)GameEnums.DamageElement.Void] > 0)
        {
            ObjectPoolManager.GetVoidNovaFromPool().SetUp(
                maxHealth * VOID_STATUS_HEALTH_PERCENT_DAMAGE / STATUS_DURATION_STANDARD / STATUS_UPDATE_INTERVAL,
                BaseHealth * VOID_STATUS_RADIUS_SCALING + VOID_STATUS_RADIUS_BASE,
                transform.position, this);
        }

        for (int i = 0; i < statusDurations.Length; i++) 
        {
            statusDurations[i] = Mathf.MoveTowards(statusDurations[i], 0, STATUS_UPDATE_INTERVAL);
        }

    }
    private void UpdateMovement()
    {
        rb.linearVelocity = currentKnockbackForce;
        currentKnockbackForce *= KNOCKBACK_FORCE_DECAY_MULTIPLICATIVE;
        currentKnockbackForce = Vector3.MoveTowards(currentKnockbackForce, Vector3.zero, Time.fixedDeltaTime * KNOCKBACK_FORCE_DECAY_LINEAR);

        for (int i = activePullForces.Count-1; i >= 0; i--) 
        {
            Vector3 pullDir = (new Vector3(activePullForces[i].center.x, 0, activePullForces[i].center.z) - new Vector3(transform.position.x, 0, transform.position.z));
            rb.linearVelocity += pullDir.normalized * activePullForces[i].magnitude;
            if (Time.time > activePullForces[i].endTime) { activePullForces.RemoveAt(i); }
        }

        if (statusDurations[(int)GameEnums.DamageElement.Frost] <= 0)
        {
            Vector3 movementDir = (new Vector3(TargetMovementPosition.x, 0, TargetMovementPosition.z) - new Vector3(transform.position.x, 0, transform.position.z));
            if (movementDir.sqrMagnitude > MIN_MOVEMENT_DISTANCE) { rb.linearVelocity += MovementSpeed * difficultySpeedMult * (inCombat ? 1 : outOfCombatSpeedMult) * movementDir.normalized; }
        }
    }
    private void UpdateRotation() 
    {
        float targetAngle = GameTools.AngleBetween(transform.position, TargetLookPosition);
        currentLookRotation = Mathf.MoveTowardsAngle(currentLookRotation, targetAngle, Time.fixedDeltaTime * VISUAL_ROTATION_SPEED);
        rotationParent.rotation = Quaternion.Euler(0, currentLookRotation, 0);
    }
    public bool IsInCombatRange() 
    {
        Vector3 closestPlayerPos = PlayerEntity.ActiveInstance.transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, closestPlayerPos);
        if (distanceToPlayer < MaxCombatDistance) 
        {
            if (!RequiresLosForCombat) { return true; }
            return !Physics.Raycast(transform.position, closestPlayerPos - transform.position, distanceToPlayer, LoSMask);
        }
        return false;
    }
    public float GetHealthPercent() 
    {
        return CurrentHealth / maxHealth;
    }
    public void Knockback(float magnitude, float direction)
    {
        Knockback(magnitude, GameTools.AngleToVector(direction));
    }
    public void Knockback(float magnitude, Vector3 direction)
    {
        if (direction == Vector3.zero) { direction = GameTools.AngleToVector(Random.Range(0, 361)); }

        currentKnockbackForce += magnitude * (1-KnockbackResistance) * direction.normalized;
        currentKnockbackForce = Vector3.ClampMagnitude(currentKnockbackForce, KNOCKBACK_FORCE_MAG_CLAMP);
    }
    public int GetStatusEffectCount() 
    {
        int count = 0;
        for (int i = 0; i < statusDurations.Length; i++) 
        {
            if (statusDurations[i] > 0) { count++; }
        }
        return count;
    }
    public float GetStatusDuration(GameEnums.DamageElement e) { return statusDurations[(int)e]; }
    public void AddStatus(GameEnums.DamageElement e) 
    {
        if (CurrentHealth <= 0) { return; }

        statusDurations[(int)e] += STATUS_DURATION_STANDARD;
        EventManager.OnEnemyStatusApplied(e, this);
        if (e == GameEnums.DamageElement.Frost) 
        {
            OnEntityStunned();
        }
    }
    public void RemoveStatus(GameEnums.DamageElement e) 
    {
        statusDurations[(int)e] = 0;
    }
    public void DealStatusDamage(float magnitude, GameEnums.DamageElement element) 
    {
        CurrentHealth -= magnitude;
        // Use extra health bars
        while (RemainingExtraBars > 0 && CurrentHealth <= 0)
        {
            RemainingExtraBars--;
            CurrentHealth += maxHealth;
        }
        EventManager.OnEnemyStatusDamageTaken(magnitude, element, this);
        if (CurrentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    public void AddPullForce(float _mag, Vector3 _center, float _duration) 
    {
        activePullForces.Add(new PullForce() { center = _center, endTime = Time.time + _duration, magnitude = _mag });
    }
    public void AddStatusBuildup(float magnitude, GameEnums.DamageElement element) 
    {
        if (element == GameEnums.DamageElement.NonElemental) { return; }
        if (CurrentHealth <= 0) { return; }

        int statusIndex = (int)element;
        float buildupStrengtht = (magnitude / BaseHealth / HEALTH_PERCENT_REQUIRED_TO_FULL_BUILDUP) / statusBuildupResistancesDivider[statusIndex];
        if (statusDurations[(int)GameEnums.DamageElement.Frostbite] > 0) { buildupStrengtht *= FROSTBITE_STATUS_EFFECT_MULTIPLIER; }
        while (buildupStrengtht > 0)
        {
            statusBuildups[statusIndex] += buildupStrengtht /= statusBuildupResistancesDivider[statusIndex];
            if (statusBuildups[statusIndex] >= 1)
            {
                buildupStrengtht = statusBuildups[statusIndex] - 1;
                statusBuildups[statusIndex] = 0;
                statusBuildupResistancesDivider[statusIndex] *= STATUS_RESISTANCE_GROWTH_MULTIPLIER;
                AddStatus(element);
            }
            else
            {
                buildupStrengtht = 0; // Or return
            }
        }
    }
    public void DealDirectDamage(float magnitude, float critChance, float critDamage, float buildupMultiplier, GameEnums.DamageElement element, GameEnums.DamageType dtype) 
    {
        if (CurrentHealth <= 0) { return; }

        int critLevel = 0;
        if (dtype != GameEnums.DamageType.Tech && statusDurations[(int)GameEnums.DamageElement.Frostbite] <= 0) 
        {
            // Roll for critical hits
            if (statusDurations[(int)GameEnums.DamageElement.Thunder] > 0) { critChance += SHOCK_STATUS_BONUS_CRIT_CHANCE; }
            while (Random.Range(0, 101) < critChance)
            {
                critChance -= 100;
                critLevel++;
                magnitude *= critDamage;
            }
        }
        

        // Reduce health
        CurrentHealth -= magnitude;
        // Use extra health bars
        while (RemainingExtraBars > 0 && CurrentHealth <= 0)
        {
            RemainingExtraBars--;
            CurrentHealth += maxHealth;
        }
        EventManager.OnEnemyDirectDamageTaken(magnitude, critLevel, element, dtype, this);

        if (CurrentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
        else 
        {
            // ONLY If the enemy survives the hit (prevents unnecesary overkill situations)
            // Add status buildup based on the damage element.
            AddStatusBuildup(magnitude * buildupMultiplier, element);
        }
    }
    private struct PullForce 
    {
        public float endTime;
        public float magnitude;
        public Vector3 center;
    }
}
