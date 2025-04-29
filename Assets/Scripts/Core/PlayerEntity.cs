using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class PlayerEntity : NetworkBehaviour
{
    // Status ========================================================================================

    public float CurrentLightDamage { get; private set; }
    public float CurrentShield { get; private set; }
    public float CurrentHealth { get; private set; }
    public int CurrentDashes { get; private set; }
    public float DashRechargePercent { get; private set; }
    public float Money { get; private set; }

    private const float BASE_DASH_RECHARGE_RATE = 0.25f;
    private const float LIGHT_DAMAGE_HEALING_MULT = 3f;

    // Weapon status =================================================================================

    [field: SerializeField] public float StatusHeatRanged { get; private set; }
    [field: SerializeField] public float StatusHeatMelee { get; private set; }
    [field: SerializeField] public bool StatusOverheatRanged { get; private set; }
    [field: SerializeField] public bool StatusOverheatMelee { get; private set; }

    [SerializeField] private WeaponSO debugRangedWeaponSO;
    [SerializeField] private WeaponSO debugMeleeWeaponSO;
    [SerializeField] private List<TechSO> debugTechSO;

    private float rangedAttackReady;
    private float meleeAttackReady;
    private float currentDirection;
    private float heatDecayMelee;
    private float heatDecayRanged;

    private const float HEAT_DECAY_GROWTH = 0.05f;

    // Stats and equipment =================================================================================

    private PlayerStatGroup stats;
    private WeaponData meleeWeapon;
    private WeaponData rangedWeapon;

    private WeaponData.WeaponStats rangedWeaponStats;
    private WeaponData.WeaponStats meleeWeaponStats;

    // Movement and camera =================================================================================

    public Transform CamTargetTransform { get { return m_camTargetPos; } }

    [SerializeField] private Transform m_camTargetPos;
    [SerializeField] private Transform m_camVerticalRotationAxis;
    [SerializeField] private Transform m_rotationParent;
    [SerializeField] private Rigidbody m_rb;
    [SerializeField] private Collider m_agentCollisionsCollider;

    // Tech ================================================================================================

    public List<TechGroup> ActiveTechList { get; private set; }
    public Dictionary<string, TechGroup> ActiveTechDictionary { get; private set; }

    // Static reference

    public static PlayerEntity ActiveInstance { get; private set; }

    private bool inputsAllowed;
    private float movementInputH;
    private float movementInputV;
    private float rotationInputH;
    private float rotationInputV;
    private Vector3 dashMovementVector;
    private float dashDurationRemaining;
    private bool _isPlayerControlsEnabled = false;
    private bool defeated = false;

    private const float LOCK_Y = 1.0f;
    private const float BASE_MOVEMENT_SPEED = 10f;
    private const float DASH_MOVEMENT_SPEED = 35f;
    private const float DASH_DURATION = 0.5f;
    private const float ROTATION_SPEED = 60f;
    private const float MIN_CAM_VERTICAL_ROTATION_X = 350f;
    private const float MAX_CAM_VERTICAL_ROTATION_X = 50f;
    private const float CAM_HORIZONTAL_DISPLACEMENT_WHEN_MOVING = 2f;

    // Buffs

    // Debug ========================================================================================

    [SerializeField] private GameObject _cube;
    [SerializeField] private GameObject _capsule;


    public void SetUp(Vector3 pos)
    {
        gameObject.SetActive(true);
        transform.position = pos;
        StageManagerBase.UpdatePlayerPosition(transform.position);
        ActiveInstance = this;
        stats = new PlayerStatGroup();
        stats.ChangeStat(PlayerStatGroup.Stat.Speed, 1f);
        stats.ChangeStat(PlayerStatGroup.Stat.MaxHealth, 500f);
        stats.ChangeStat(PlayerStatGroup.Stat.DashCount, 3);
        stats.ChangeStat(PlayerStatGroup.Stat.DashRechargeRate, 1);
        stats.ChangeStat(PlayerStatGroup.Stat.Firerate, 1);

        CurrentHealth = stats.GetStat(PlayerStatGroup.Stat.MaxHealth);
        CurrentDashes = (int)stats.GetStat(PlayerStatGroup.Stat.DashCount);
        DashRechargePercent = 0;
        ActiveTechDictionary = new Dictionary<string, TechGroup>();
        ActiveTechList = new List<TechGroup>();
        StatusOverheatRanged = StatusOverheatMelee = false;
        rangedAttackReady = meleeAttackReady = 1;

        for (int i = 0; i < debugTechSO.Count; i++) { EquipTech(debugTechSO[i]); } // Test

        List<PlayerTraitSO> selectedTraits = PersistentDataManager.GetTraitSelection();
        for (int i = 0; i < selectedTraits.Count; i++)
        {
            if (selectedTraits[i].Tech != null) { EquipTech(selectedTraits[i].Tech); }
        }

        WeaponData rangedweapon = new WeaponData(debugRangedWeaponSO);

        EquipWeapon(rangedweapon);
        EquipWeapon(new WeaponData(debugMeleeWeaponSO));

        currentDirection = 0; UpdateRotation();
        inputsAllowed = true;

        EventManager.OnPlayerStatsUpdated(this);

        EventManager.UiMenuFocusChangedEvent += OnUiMenuChanged;
    }
    private void OnDisable()
    {
        EventManager.UiMenuFocusChangedEvent -= OnUiMenuChanged;
    }
    private void OnUiMenuChanged(IGameMenu m) 
    {
        inputsAllowed = m == null;
    }
    private void Start()
    {
        if (IsOwner || NetworkManager.Singleton == null)
        {
            _isPlayerControlsEnabled = true;
        }

        if (IsHost && IsOwner)
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
    public void EquipTech(TechSO t)
    {
        TechGroup tg;
        if (ActiveTechDictionary.ContainsKey(t.ID))
        {
            tg = ActiveTechDictionary[t.ID];
            tg.Upgrade();
        }
        else 
        {
            tg = new TechGroup(t);
            if (tg.Script != null) { tg.Script.PlayerRef = this; }
            ActiveTechDictionary.Add(tg.SO.ID, tg);
            ActiveTechList.Add(tg);
        }
        // Add stat and flag bonuses
        for (int i = 0; i < tg.SO.BonusStats.Count; i++)
        {
            stats.ChangeStat(tg.SO.BonusStats[i].First, tg.SO.BonusStats[i].Second);
        }
        for (int i = 0; i < tg.SO.BonusFlags.Count; i++)
        {
            stats.ChangeFlag(tg.SO.BonusFlags[i], 1);
        }
        EventManager.OnPlayerStatsUpdated(this);
    }
    public void UnequipTech(TechSO t)
    {
        if (ActiveTechDictionary.ContainsKey(t.ID)) 
        {
            TechGroup tg = ActiveTechDictionary[t.ID];
            for (int i = 0; i < tg.SO.BonusStats.Count; i++) 
            {
                stats.ChangeStat(tg.SO.BonusStats[i].First, -tg.SO.BonusStats[i].Second * tg.Level);
            }
            for (int i = 0; i < tg.SO.BonusFlags.Count; i++)
            {
                stats.ChangeFlag(tg.SO.BonusFlags[i], -tg.Level);
            }
            ActiveTechDictionary[t.ID].Script?.OnTechRemoved();
            ActiveTechDictionary.Remove(t.ID);
            ActiveTechList.Remove(tg);
        }
        EventManager.OnPlayerStatsUpdated(this);
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
    public int GetTechLevel(string id) 
    {
        if (ActiveTechDictionary.ContainsKey(id)) { return ActiveTechDictionary[id].Level; }
        return 0;
    }
    private void UpdateRotation()
    {
        m_rotationParent.transform.rotation = Quaternion.Euler(0, currentDirection, 0);
    }
    private void Update()
    {
        movementInputH = InputManager.Instance.GetMovementInput().x;
        movementInputV = InputManager.Instance.GetMovementInput().y;
        rotationInputH = inputsAllowed ? InputManager.Instance.GetLookInput().x : 0;
        rotationInputV = inputsAllowed ? InputManager.Instance.GetLookInput().y : 0;

        currentDirection += rotationInputH * Time.deltaTime * ROTATION_SPEED;
        UpdateRotation();
        m_camVerticalRotationAxis.Rotate(-rotationInputV * Time.deltaTime * ROTATION_SPEED, 0, 0);
        ClampCamVerticalRotation();

        rangedAttackReady += Time.deltaTime * rangedWeaponStats.Firerate * stats.GetStat(PlayerStatGroup.Stat.Firerate);
        heatDecayRanged += Time.deltaTime * HEAT_DECAY_GROWTH;
        StatusHeatRanged = Mathf.MoveTowards(StatusHeatRanged, 0, Time.deltaTime * heatDecayRanged);
        if (StatusOverheatRanged && StatusHeatRanged <= 0) { StatusOverheatRanged = false; }

        meleeAttackReady += Time.deltaTime * meleeWeaponStats.Firerate * stats.GetStat(PlayerStatGroup.Stat.Firerate);
        heatDecayMelee += Time.deltaTime * HEAT_DECAY_GROWTH;
        StatusHeatMelee = Mathf.MoveTowards(StatusHeatMelee, 0, Time.deltaTime * heatDecayMelee);
        if (StatusOverheatMelee && StatusHeatMelee <= 0) { StatusOverheatMelee = false; }

        if (CurrentDashes < (int)stats.GetStat(PlayerStatGroup.Stat.DashCount)) 
        {
            DashRechargePercent += Time.deltaTime * BASE_DASH_RECHARGE_RATE * stats.GetStat(PlayerStatGroup.Stat.DashRechargeRate);
            if (DashRechargePercent >= 1) 
            {
                DashRechargePercent = 0;
                CurrentDashes++;
            }
        }
        dashDurationRemaining -= Time.deltaTime;

        if (InputManager.Instance.GetRangedAttackInput() && rangedAttackReady >= 1 && !StatusOverheatRanged && inputsAllowed)
        {
            heatDecayRanged = 0;
            rangedAttackReady = 0;
            StatusHeatRanged += rangedWeaponStats.HeatGen;
            if (StatusHeatRanged > 1) { StatusOverheatRanged = true; }
            Attack(WeaponSO.WeaponSlot.Ranged);
        }
        if (InputManager.Instance.GetMeleeAttackInput() && meleeAttackReady >= 1 && !StatusOverheatMelee && inputsAllowed)
        {
            heatDecayMelee = 0;
            meleeAttackReady = 0;
            StatusHeatMelee += meleeWeaponStats.HeatGen;
            if (StatusHeatMelee > 1) { StatusOverheatMelee = true; }
            Attack(WeaponSO.WeaponSlot.Melee);
        }
        if (InputManager.Instance.GetDashInput() && inputsAllowed) 
        {
            Dash();
        }
    }
    public void CoolDownWeapons(float tempRemoved) 
    {
        StatusHeatMelee = Mathf.MoveTowards(StatusHeatMelee, 0, tempRemoved);
        StatusHeatRanged = Mathf.MoveTowards(StatusHeatRanged, 0, tempRemoved);
    }
    private void Attack(WeaponSO.WeaponSlot s)
    {
        if (defeated) { return; }

        WeaponData.WeaponStats wpn = s == WeaponSO.WeaponSlot.Ranged ? rangedWeaponStats : meleeWeaponStats;
        float attackMagnitudeMultiplier = s == WeaponSO.WeaponSlot.Ranged ?
            GameTools.DexterityToDamageMultiplier(stats.GetStat(PlayerStatGroup.Stat.Dexterity)) : GameTools.MightToDamageMultiplier(stats.GetStat(PlayerStatGroup.Stat.Might));

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

        if (dashDurationRemaining > 0)
        {
            m_rb.linearVelocity = DASH_MOVEMENT_SPEED * stats.GetStat(PlayerStatGroup.Stat.Speed) * dashMovementVector;
        }
        else 
        {
            m_rb.linearVelocity = movementInputV * BASE_MOVEMENT_SPEED * stats.GetStat(PlayerStatGroup.Stat.Speed) * m_rotationParent.forward;
            m_rb.linearVelocity += movementInputH * BASE_MOVEMENT_SPEED * stats.GetStat(PlayerStatGroup.Stat.Speed) * m_rotationParent.right;
        }
        m_agentCollisionsCollider.enabled = dashDurationRemaining <= 0;
        transform.position = new Vector3(transform.position.x, LOCK_Y, transform.position.z);
        StageManagerBase.UpdatePlayerPosition(transform.position);
    }
    private void ClampCamVerticalRotation()
    {
        float x = m_camVerticalRotationAxis.localRotation.eulerAngles.x;
        x = x < 180 ? Mathf.Clamp(x, 0, MAX_CAM_VERTICAL_ROTATION_X) : Mathf.Clamp(x, MIN_CAM_VERTICAL_ROTATION_X, 360);
        m_camVerticalRotationAxis.localRotation = Quaternion.Euler(x, 0, 0);
    }
    private void Dash() 
    {
        if (CurrentDashes < 1) { return; }
        if (dashDurationRemaining > 0) { return; }

        CurrentDashes -= 1;

        dashMovementVector = movementInputV * m_rotationParent.forward;
        dashMovementVector += movementInputH * m_rotationParent.right;
        dashDurationRemaining = DASH_DURATION * 1 + stats.GetStat(PlayerStatGroup.Stat.DashBonusDuration);
    }
    public void DealDamage(float magnitude)
    {
        if (defeated) { return; }

        float damageToShield = Mathf.Min(CurrentShield, magnitude);
        CurrentShield -= damageToShield;
        magnitude -= damageToShield;

        CurrentHealth -= magnitude;
        AddShield(magnitude * stats.GetStat(PlayerStatGroup.Stat.DamageToShieldConversion));
        AddLightDamage(magnitude * stats.GetStat(PlayerStatGroup.Stat.DamageToLightConversion));

        if (CurrentHealth <= 0)
        {
            EventManager.OnPlayerDefeated();
            // Kill?
        }
    }
    public bool IsEvading() 
    {
        return dashDurationRemaining > 0;
    }
    public void AddLightDamage(float amount) 
    {
        if (defeated) { return; }

        CurrentLightDamage = Mathf.Clamp(CurrentLightDamage + amount, 0, stats.GetStat(PlayerStatGroup.Stat.MaxHealth) - CurrentHealth);
    }
    public void AddShield(float amount) 
    {
        if (defeated) { return; }

        CurrentShield = Mathf.Clamp(CurrentShield + amount, 0, stats.GetStat(PlayerStatGroup.Stat.MaxHealth));
    }
    public void Heal(float amount)
    {
        if (defeated) { return; }

        amount = Mathf.Max(amount, 0); // Healing cannot be negative
        float lightDamageHealing = Mathf.Min(CurrentLightDamage / LIGHT_DAMAGE_HEALING_MULT, amount);
        CurrentLightDamage -= lightDamageHealing * LIGHT_DAMAGE_HEALING_MULT;
        amount -= lightDamageHealing;

        float totalHealing = lightDamageHealing * LIGHT_DAMAGE_HEALING_MULT;
        totalHealing += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth + totalHealing, 0, stats.GetStat(PlayerStatGroup.Stat.MaxHealth));
    }
    public float GetHealthPercent()
    {
        return CurrentHealth / stats.GetStat(PlayerStatGroup.Stat.MaxHealth);
    }
    public float GetLightDamagePercent() 
    {
        return GetHealthPercent() + CurrentLightDamage / stats.GetStat(PlayerStatGroup.Stat.MaxHealth);
    }
    public float GetShieldPercent() 
    {
        return CurrentShield / stats.GetStat(PlayerStatGroup.Stat.MaxHealth);
    }
    public float GetStat(PlayerStatGroup.Stat s) 
    {
        return stats.GetStat(s);
    }
    public void AddMoney(float amount)
    {
        Money += amount;
        EventManager.OnCurrencyUpdated();
    }
    public void SpendMoney(float amount)
    {
        Money -= amount;
        Money = Mathf.Max(0, Money);
        EventManager.OnCurrencyUpdated();
    }

    public class TechGroup
    {
        public int Level { get; private set; }
        public TechSO SO { get; private set; }
        public TechBase Script { get; private set; }

        public TechGroup(TechSO t) 
        {
            Level = 1;
            SO = t;
            if (SO.Script != "") 
            {
                Script = System.Activator.CreateInstance(System.Type.GetType(SO.Script)) as TechBase;
                Script.Group = this;
                Script.OnTechAdded();
            }
            
        }
        public void Upgrade() 
        {
            Level++;
            Script?.OnTechUpgraded();
        }
    }
}
