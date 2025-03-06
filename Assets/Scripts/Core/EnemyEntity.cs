using UnityEngine;
using System.Collections.Generic;

public class EnemyEntity : MonoBehaviour
{

    // Stats ===================================================

    [field: SerializeField] public float MovementSpeed { get; private set; }
    [field: SerializeField] public float MaxHealth { get; private set; }

    // Status ==================================================

    private float currentHealth;
    private float[] statusBuildups;

    private const float HEALTH_PERCENT_REQUIRED_TO_FULL_BUILDUP = 0.2f;

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
        TargetMovementPosition = transform.position;
        currentLookRotation = 0;
        currentHealth = MaxHealth;
        LoSMask = LayerMask.GetMask("Walls");
        SetupAi();
        statusBuildups = new float[10];
        EnemyInstanceID = StageManagerBase.RegisterEnemy(this);
    }
    private void OnDisable()
    {
        StageManagerBase.UnregisterEnemy(this);
    }
    private void FixedUpdate()
    {
        UpdateAI();
        UpdateRotation();
        UpdateMovement();
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
    public void Knockback(float magnitude, float direction)
    {
        Knockback(magnitude, GameTools.AngleToVector(direction));
    }
    public void Knockback(float magnitude, Vector3 direction)
    {
        rb.linearVelocity += direction.normalized * magnitude;
    }
    public void DealDamage(float magnitude, float critChance, float critDamage, float buildupMultiplier, GameEnums.DamageElement element) 
    {
        // Roll for critical hits
        int critLevel = 0;
        while (Random.Range(0, 101) < critChance) { critChance -= 100; critLevel++; }
        magnitude *= Mathf.Pow(1 + critDamage, critLevel);

        // Increase stauts buildup

        statusBuildups[(int)element] += ((magnitude/currentHealth) / HEALTH_PERCENT_REQUIRED_TO_FULL_BUILDUP) * buildupMultiplier;

        // Reduce health

        EventManager.OnEnemyDirectDamageTaken(transform.position, magnitude, 0, element);
        currentHealth -= magnitude;

        // Kill if necessary

        if (currentHealth < 0) 
        {
            gameObject.SetActive(false);
            StageManagerBase.GetObjectPool().GetCurrencyPickupFromPool().SetUp(transform.position, 1); 
            StageManagerBase.GetObjectPool().GetCurrencyPickupFromPool().SetUp(transform.position, 1);
            StageManagerBase.GetObjectPool().GetCurrencyPickupFromPool().SetUp(transform.position, 1);
        }

    }

}
