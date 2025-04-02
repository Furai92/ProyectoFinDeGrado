using Unity.Netcode;
using UnityEngine;

public class PlayerEntity : NetworkBehaviour
{
    // Status ========================================================================================

    float currentHealth;

    // Weapon status =================================================================================

    [field: SerializeField] public float StatusHeatRanged { get; private set; }
    [field: SerializeField] public float StatusHeatMelee { get; private set; }
    [field: SerializeField] public bool StatusOverheatRanged { get; private set; }
    [field: SerializeField] public bool StatusOverheatMelee { get; private set; }

    [SerializeField] private WeaponSO debugRangedWeaponSO;
    [SerializeField] private WeaponSO debugMeleeWeaponSO;

    private float rangedAttackReady;
    private float meleeAttackReady;
    private float currentDirection;
    private float heatDecayMelee;
    private float heatDecayRanged;

    private const float HEAT_DECAY_GROWTH = 0.05f;

    // Stats and equipment =================================================================================

    private StatGroup stats;

    private WeaponData meleeWeapon;
    private WeaponData rangedWeapon;

    private WeaponData.WeaponStats rangedWeaponStats;
    private WeaponData.WeaponStats meleeWeaponStats;

    // Movement and camera =================================================================================

    [SerializeField] private Transform m_camVerticalRotationAxis;
    [SerializeField] private Transform m_rotationParent;
    [SerializeField] private Camera m_playerCamera;
    [SerializeField] private Rigidbody m_rb;

    private float movementInputH;
    private float movementInputV;
    private float rotationInputH;
    private float rotationInputV;
    private bool _isPlayerControlsEnabled = false;

    private const float LOCK_Y = 1.0f;
    private const float BASE_MOVEMENT_SPEED = 10f;
    private const float ROTATION_SPEED = 60f;
    private const float MIN_CAM_VERTICAL_ROTATION_X = 350f;
    private const float MAX_CAM_VERTICAL_ROTATION_X = 50f;
    private const float CAM_HORIZONTAL_DISPLACEMENT_WHEN_MOVING = 2f;

    // Buffs



    // Techs ========================================================================================



    // Debug ========================================================================================

    [SerializeField] private GameObject _cube;
    [SerializeField] private GameObject _capsule;


    public void SetUp(Vector3 pos)
    {
        gameObject.SetActive(true);
        transform.position = pos;
        StageManagerBase.UpdatePlayerPosition(transform.position);
        stats = new StatGroup();
        stats.ChangeStat(StatGroup.Stat.Speed, 1f);
        stats.ChangeStat(StatGroup.Stat.MaxHealth, 500f);
        currentHealth = stats.GetStat(StatGroup.Stat.MaxHealth);
        EquipWeapon(new WeaponData(debugRangedWeaponSO));
        EquipWeapon(new WeaponData(debugMeleeWeaponSO));

        StatusOverheatRanged = false;

        currentDirection = 0;
        rangedAttackReady = meleeAttackReady = 1;
        UpdateRotation();
    }
    private void Start () 
    {
        if (IsOwner || NetworkManager.Singleton == null)
        {
            _isPlayerControlsEnabled = true;
            m_playerCamera.gameObject.SetActive(true);
        }

        if(IsHost && IsOwner)
        {
            _cube.GetComponent<Renderer>().material.color = Color.green;
            _capsule.GetComponent<Renderer>().material.color = Color.green;
        }
        else if (IsHost && !IsOwner)
        {
            _cube.GetComponent<Renderer>().material.color = Color.red;
            _capsule.GetComponent<Renderer>().material.color = Color.red;
        }
        else if (!IsHost && IsOwner)
        {
            _cube.GetComponent<Renderer>().material.color = Color.red;
            _capsule.GetComponent<Renderer>().material.color = Color.red;
        }
        else if (!IsHost && !IsOwner)
        {
            _cube.GetComponent<Renderer>().material.color = Color.green;
            _capsule.GetComponent<Renderer>().material.color = Color.green;
        }
    }
    public void EquipWeapon(WeaponData w) 
    {
        switch (w.BaseWeapon.Slot) 
        {
            case WeaponSO.WeaponSlot.Melee: 
                {
                    if (meleeWeapon != null) 
                    {
                        ObjectPoolManager.GetWeaponPickupFromPool().SetUp(meleeWeapon, transform.position);
                    }

                    meleeWeapon = w;
                    meleeWeaponStats = meleeWeapon.GetStats();
                    break;
                }
            case WeaponSO.WeaponSlot.Ranged: 
                {
                    if (rangedWeapon != null)
                    {
                        ObjectPoolManager.GetWeaponPickupFromPool().SetUp(rangedWeapon, transform.position);
                    }

                    rangedWeapon = w;
                    rangedWeaponStats = rangedWeapon.GetStats();
                    break;
                }
        }
    }
    private void UpdateRotation() 
    {
        m_rotationParent.transform.rotation = Quaternion.Euler(0, currentDirection, 0);
    }
    private void Update()
    {
        movementInputH = InputManager.Instance.GetMovementInput().x;
        movementInputV = InputManager.Instance.GetMovementInput().y;
        rotationInputH = InputManager.Instance.GetLookInput().x;
        rotationInputV = InputManager.Instance.GetLookInput().y;

        currentDirection += rotationInputH * Time.deltaTime * ROTATION_SPEED;
        UpdateRotation();
        m_camVerticalRotationAxis.Rotate(-rotationInputV * Time.deltaTime * ROTATION_SPEED, 0, 0);
        ClampCamVerticalRotation();

        rangedAttackReady += Time.deltaTime * rangedWeaponStats.Firerate;
        heatDecayRanged += Time.deltaTime * HEAT_DECAY_GROWTH;
        StatusHeatRanged = Mathf.MoveTowards(StatusHeatRanged, 0, Time.deltaTime * heatDecayRanged);
        if (StatusOverheatRanged && StatusHeatRanged <= 0) { StatusOverheatRanged = false; }

        meleeAttackReady += Time.deltaTime * meleeWeaponStats.Firerate;
        heatDecayMelee += Time.deltaTime * HEAT_DECAY_GROWTH;
        StatusHeatMelee = Mathf.MoveTowards(StatusHeatMelee, 0, Time.deltaTime * heatDecayMelee);
        if (StatusOverheatMelee && StatusHeatMelee <= 0) { StatusOverheatMelee = false; }

        if (InputManager.Instance.GetRangedAttackInput() && rangedAttackReady >= 1 && !StatusOverheatRanged) 
        {
            heatDecayRanged = 0;
            rangedAttackReady = 0;
            StatusHeatRanged += rangedWeaponStats.HeatGen;
            if (StatusHeatRanged > 1) { StatusOverheatRanged = true; }
            Attack(WeaponSO.WeaponSlot.Ranged);
        }
        if (InputManager.Instance.GetMeleeAttackInput() && meleeAttackReady >= 1 && !StatusOverheatMelee)
        {
            heatDecayMelee = 0;
            meleeAttackReady = 0;
            StatusHeatMelee += meleeWeaponStats.HeatGen;
            if (StatusHeatMelee > 1) { StatusOverheatMelee = true; }
            Attack(WeaponSO.WeaponSlot.Melee);
        }
    }
    private void Attack(WeaponSO.WeaponSlot s) 
    {
        WeaponData.WeaponStats wpn = s == WeaponSO.WeaponSlot.Ranged ? rangedWeaponStats : meleeWeaponStats;
        float attackMagnitudeMultiplier = s == WeaponSO.WeaponSlot.Ranged ? 
            GameTools.DexterityToDamageMultiplier(stats.GetStat(StatGroup.Stat.Dexterity)) : GameTools.MightToDamageMultiplier(stats.GetStat(StatGroup.Stat.Might));

        // Projectile component
        if (wpn.ProjectileComponentID != "") 
        {
            float currentMultishootAngle = -wpn.Arc / 2;
            for (int i = 0; i < wpn.Multishoot; i++)
            {
                WeaponAttackSetupData sd = new WeaponAttackSetupData()
                {
                    User = this,
                    Magnitude = wpn.ProjectileComponentMagnitude * attackMagnitudeMultiplier,
                    Bounces = wpn.Bounces,
                    BuildupRate = wpn.BuildupRate,
                    CritDamage = wpn.CritMultiplier,
                    CritChance = 0,
                    Pierces = wpn.Pierces,
                    SizeMultiplier = wpn.SizeMultiplier,
                    Splash = wpn.Splash,
                    Timescale = wpn.Timescale,
                    EnemyIgnored = -1,
                    Knockback = wpn.Knockback,
                    Element = wpn.Element
                };
                ObjectPoolManager.GetPlayerAttackFromPool(wpn.ProjectileComponentID).SetUp(transform.position, currentDirection + currentMultishootAngle, sd, null);
                currentMultishootAngle += wpn.Multishoot < 2 ? 0 : wpn.Arc / (wpn.Multishoot - 1);
            }
        }
        // Cleave component
        if (wpn.CleaveComponentID != "") 
        {
            WeaponAttackSetupData sd = new WeaponAttackSetupData()
            {
                User = this,
                Magnitude = wpn.CleaveComponentMagnitude * attackMagnitudeMultiplier,
                Bounces = wpn.Bounces,
                BuildupRate = wpn.BuildupRate,
                CritDamage = wpn.CritMultiplier,
                CritChance = 0,
                Pierces = wpn.Pierces,
                SizeMultiplier = wpn.SizeMultiplier,
                Splash = wpn.Splash,
                Timescale = wpn.Timescale,
                EnemyIgnored = -1,
                Knockback = wpn.Knockback,
                Element = wpn.Element
            };
            ObjectPoolManager.GetPlayerAttackFromPool(wpn.CleaveComponentID).SetUp(transform.position, currentDirection, sd, null);
        }
    }
    private void FixedUpdate()
    {
        if (!_isPlayerControlsEnabled) return;

        m_rb.linearVelocity = movementInputV * BASE_MOVEMENT_SPEED * stats.GetStat(StatGroup.Stat.Speed) * m_rotationParent.forward;
        m_rb.linearVelocity += movementInputH * BASE_MOVEMENT_SPEED * stats.GetStat(StatGroup.Stat.Speed) * m_rotationParent.right;
        transform.position = new Vector3(transform.position.x, LOCK_Y, transform.position.z);
        StageManagerBase.UpdatePlayerPosition(transform.position);
    }
    private void ClampCamVerticalRotation() 
    {
        float x = m_camVerticalRotationAxis.localRotation.eulerAngles.x;
        x = x < 180 ? Mathf.Clamp(x, 0, MAX_CAM_VERTICAL_ROTATION_X) : Mathf.Clamp(x, MIN_CAM_VERTICAL_ROTATION_X, 360); 
        m_camVerticalRotationAxis.localRotation = Quaternion.Euler(x, 0, 0);
    }
    public Camera GetCamera() 
    {
        return m_playerCamera;
    }
    public void DealDamage(float magnitude) 
    {
        currentHealth -= magnitude;
        if (currentHealth <= 0) 
        {
            // Kill?

        }
    }
    public void Heal(float amount) 
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, stats.GetStat(StatGroup.Stat.MaxHealth));
    }
    public float GetHealthPercent() 
    {
        return currentHealth / stats.GetStat(StatGroup.Stat.MaxHealth);
    }
}
