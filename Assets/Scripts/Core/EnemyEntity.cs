using UnityEngine;
using System.Collections.Generic;

public class EnemyEntity : MonoBehaviour
{

    // Stats ===================================================

    [field: SerializeField] public float MovementSpeed { get; private set; }
    [field: SerializeField] public float MaxHealth { get; private set; }

    // Rewards =================================================

    [field: SerializeField] public float ItemDropRate { get; private set; }
    [field: SerializeField] public float CurrencyDropRate { get; private set; }
    [field: SerializeField] public float HealthDroprate { get; private set; }

    // Status ==================================================

    private float currentHealth;
    private float[] statusBuildups;
    private float[] statusBuildupResistancesDivider;
    private float[] statusDurations;
    private float nextStatusUpdateTime;

    private const float FIRE_STATUS_HEALTH_PERCENT_DAMAGE = 0.25f;
    private const float STATUS_DURATION_STANDARD = 10f;
    private const float STATUS_UPDATE_INTERVAL = 1f;
    private const float STATUS_RESISTANCE_GROWTH_MULTIPLIER = 1.5f;
    private const float HEALTH_PERCENT_REQUIRED_TO_FULL_BUILDUP = 0.4f;
    private const int BUILDUP_ARRAY_LENGHT = 5;

    // AI Management ===========================================

    [SerializeField] private ScriptableObject AiFile;
    [field: SerializeField] public float MaxCombatDistance { get; private set; }
    [field: SerializeField] public bool RequiresLosForCombat { get; private set; }

    private AiStateBase pathfindingSearchState;
    private AiStateBase currentState;
    private AiStateBase combatRootState;
    private bool inCombat;

    // Movement and Physics ================================================

    [SerializeField] private Rigidbody rb;
    public Vector3 TargetMovementPosition { get; set; }
    public Vector3 TargetLookPosition { get; set; }
 

    private float currentLookRotation;

    private const float VISUAL_ROTATION_SPEED = 300f;
    private const float HEIGHT_LOCK = 0.5f;

    // Other/Cached variables

    public int EnemyInstanceID { get; private set; }
    private LayerMask LoSMask;

    // References

    [SerializeField] private Transform rotationParent;


    public void SetUp(Vector3 pos) 
    {
        transform.position = pos;
        gameObject.SetActive(true);
    }
    private void OnEnable()
    {
        LoSMask = LayerMask.GetMask("Walls");
        ResetEntity();
        SetupAi();
        EnemyInstanceID = StageManagerBase.RegisterEnemy(this);
    }
    private void ResetEntity() 
    {
        TargetMovementPosition = transform.position;
        currentLookRotation = 0;
        currentHealth = MaxHealth;
        statusBuildups = new float[BUILDUP_ARRAY_LENGHT];
        statusDurations = new float[BUILDUP_ARRAY_LENGHT];
        statusBuildupResistancesDivider = new float[BUILDUP_ARRAY_LENGHT];
        for (int i = 0; i < statusBuildupResistancesDivider.Length; i++) { statusBuildupResistancesDivider[i] = 1; }
        nextStatusUpdateTime = Time.time + STATUS_UPDATE_INTERVAL;
    }
    private void OnDisable()
    {
        EventManager.OnEnemyDisabled(this);
        StageManagerBase.UnregisterEnemy(this);
    }
    private void FixedUpdate()
    {
        UpdateAI();
        UpdateRotation();
        UpdateMovement();
        UpdateStatusEffects();
    }
    private void SetupAi() 
    {
        pathfindingSearchState = new AiStatePathfindingSearch(this);
        combatRootState = new AiStateChargeRangedAttack(this);
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
        currentState.StartState();
    }
    private void UpdateAI() 
    {
        if (statusDurations[(int)GameEnums.DamageElement.Frost] > 0) { return; } // Pause if frozen

        currentState.FixedUpdateState();
        if (currentState.IsFinished()) 
        {
            currentState.EndState();

            if (IsInCombatRange())
            {
                currentState = inCombat ? currentState.GetNextState() : combatRootState;
                inCombat = true;
            }
            else
            {
                currentState = pathfindingSearchState;
                inCombat = false;
            }
            currentState.StartState();
        }
    }
    private void UpdateStatusEffects() 
    {
        if (Time.time < nextStatusUpdateTime) { return; }

        nextStatusUpdateTime = Time.time + STATUS_UPDATE_INTERVAL;

        if (statusDurations[(int)GameEnums.DamageElement.Fire] > 0) 
        {
            DealStatusDamage(MaxHealth * FIRE_STATUS_HEALTH_PERCENT_DAMAGE / STATUS_DURATION_STANDARD / STATUS_UPDATE_INTERVAL, GameEnums.DamageElement.Fire); 
        } 

        for (int i = 0; i < statusDurations.Length; i++) 
        {
            statusDurations[i] = Mathf.MoveTowards(statusDurations[i], 0, STATUS_UPDATE_INTERVAL);
        }

    }
    private void UpdateMovement()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(TargetMovementPosition.x, transform.position.y, TargetMovementPosition.z), Time.fixedDeltaTime * MovementSpeed);
        transform.position = new Vector3(transform.position.x, HEIGHT_LOCK, transform.position.z);
    }
    private void UpdateRotation() 
    {
        float targetAngle = GameTools.AngleBetween(transform.position, TargetLookPosition); // TO DO: Negative???
        currentLookRotation = Mathf.MoveTowardsAngle(currentLookRotation, targetAngle, Time.fixedDeltaTime * VISUAL_ROTATION_SPEED);
        rotationParent.rotation = Quaternion.Euler(0, currentLookRotation, 0);
    }
    public bool IsInCombatRange() 
    {
        Vector3 closestPlayerPos = StageManagerBase.GetClosestPlayerPosition(transform.position);
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
        return currentHealth / MaxHealth;
    }
    public void Knockback(float magnitude, float direction)
    {
        Knockback(magnitude, GameTools.AngleToVector(direction));
    }
    public void Knockback(float magnitude, Vector3 direction)
    {
        if (direction == Vector3.zero) { direction = GameTools.AngleToVector(Random.Range(0, 361)); }

        rb.linearVelocity += direction.normalized * magnitude;
    }
    public void AddStatus(GameEnums.DamageElement e) 
    {
        EventManager.OnEnemyStatusApplied(e, this);

        statusDurations[(int)e] += STATUS_DURATION_STANDARD;
    }
    public void DealStatusDamage(float magnitude, GameEnums.DamageElement element) 
    {
        EventManager.OnEnemyStatusDamageTaken(magnitude, element, this);
        currentHealth -= magnitude;
        if (currentHealth <= 0)
        {
            EventManager.OnEnemyDefeated(this);
            gameObject.SetActive(false);
        }
    }
    public void AddStatusBuildup(float magnitude, GameEnums.DamageElement element) 
    {
        int statusIndex = (int)element;
        float buildupStrengtht = (magnitude / MaxHealth / HEALTH_PERCENT_REQUIRED_TO_FULL_BUILDUP) / statusBuildupResistancesDivider[statusIndex];
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
    public void DealDirectDamage(float magnitude, float critChance, float critDamage, float buildupMultiplier, GameEnums.DamageElement element) 
    {
        // Roll for critical hits
        int critLevel = 0;
        while (Random.Range(0, 101) < critChance) 
        {
            critChance -= 100;
            critLevel++;
            magnitude *= critDamage;
        }

        // Reduce health

        currentHealth -= magnitude;
        EventManager.OnEnemyDirectDamageTaken(magnitude, 0, element, this);

        // Kill if necessary

        if (currentHealth <= 0)
        {
            EventManager.OnEnemyDefeated(this);
            gameObject.SetActive(false);
        }
        else 
        {
            // If the enemy survives the hit, add status buildup based on the damage element.
            AddStatusBuildup(magnitude * buildupMultiplier, element);
        }
    }

}
