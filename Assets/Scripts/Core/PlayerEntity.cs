using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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
    private const float SHIELD_DECAY_RATE = 10f;

    // Weapon status =================================================================================

    public float StatusHeatRanged { get; private set; }
    public float StatusHeatMelee { get; private set; }
    public bool StatusOverheatRanged { get; private set; }
    public bool StatusOverheatMelee { get; private set; }
    public float CurrentLookDirection { get; private set; }

    [SerializeField] private WeaponSO debugRangedWeaponSO;
    [SerializeField] private WeaponSO debugMeleeWeaponSO;
    [SerializeField] private List<TechSO> debugTechSO;

    private float rangedAttackReady;
    private float meleeAttackReady;
    private float heatDecayMelee;
    private float heatDecayRanged;
    private float coolingReadyTimeMelee;
    private float coolingReadyTimeRanged;

    private const float COOLING_START_DELAY = 2f;
    private const float HEAT_DECAY_GROWTH = 8f;
    private const float ECHO_RANDOM_SPREAD_MIN = 2f;
    private const float ECHO_RANDOM_SPREAD_MAX = 7f;

    // Stats and equipment =================================================================================

    private PlayerStatGroup stats;
    public WeaponData MeleeWeapon { get; private set; }
    public WeaponData RangedWeapon { get; private set; }

    private WeaponData.WeaponStats rangedWeaponStats;
    private WeaponData.WeaponStats meleeWeaponStats;

    // Movement and camera =================================================================================

    public Transform CamTargetTransform { get { return m_camTargetTransform; } }

    [SerializeField] public Transform m_camTargetTransform;
    [SerializeField] private Transform m_camVerticalRotationAxis;
    [SerializeField] private Transform m_rotationParent;
    [SerializeField] private Rigidbody m_rb;
    [SerializeField] private Collider m_agentCollisionsCollider;

    // Tech ================================================================================================

    public List<TechGroup> ActiveTechList { get; private set; }
    public Dictionary<string, TechGroup> ActiveTechDictionary { get; private set; }

    private float nextTechTimeIntervalUpdate;

    // Buffs ===============================================================================================

    private Dictionary<string, BuffEffectSO> buffSODictionary;
    public Dictionary<string, ActiveBuff> ActiveBuffDictionary { get; private set; }
    public List<ActiveBuff> ActiveBuffs { get; private set; }
    private float buffUpdateTime;

    private const float BUFF_UPDATE_INTERVAL = 0.5f;

    // Scriptable Objects ==================================================================================

    [SerializeField] private GameDatabaseSO database;

    // Static reference ====================================================================================

    public static PlayerEntity ActiveInstance { get; private set; }

    private bool inputsAllowed;
    private float movementInputH;
    private float movementInputV;
    private float rotationInputH;
    private float rotationInputV;
    private Vector3 dashMovementVector;
    private float dashDurationRemaining;
    private bool defeated = false;

    private const float LOCK_Y = 1.0f;
    private const float BASE_MOVEMENT_SPEED = 10f;
    private const float DASH_MOVEMENT_SPEED = 35f;
    public const float DASH_DURATION = 0.5f;
    private const float DASH_SLAM_RAYCAST_LENGHT = 2.5f;
    private const float DASH_SLAM_WALL_SEPARATION = 0.5f;
    private const float ROTATION_SPEED = 60f;
    private const float MIN_CAM_VERTICAL_ROTATION_X = 350f;
    private const float MAX_CAM_VERTICAL_ROTATION_X = 50f;

    public void SetUp(Vector3 pos)
    {
        gameObject.SetActive(true);
        SetUpBuffDictionary();
        buffUpdateTime = Time.time + BUFF_UPDATE_INTERVAL;
        transform.position = pos;
        StageManagerBase.UpdatePlayerPosition(transform.position);
        ActiveInstance = this;
        stats = new PlayerStatGroup();
        stats.ChangeStat(PlayerStatGroup.Stat.Speed, 1f);
        stats.ChangeStat(PlayerStatGroup.Stat.MaxHealth, 500f);
        stats.ChangeStat(PlayerStatGroup.Stat.DashCount, 3);
        stats.ChangeStat(PlayerStatGroup.Stat.DashRechargeRate, 1);
        stats.ChangeStat(PlayerStatGroup.Stat.Firerate, 1);
        stats.ChangeStat(PlayerStatGroup.Stat.CritChance, 15);
        stats.ChangeStat(PlayerStatGroup.Stat.HeatCap, 100);

        nextTechTimeIntervalUpdate = 0;
        ActiveTechDictionary = new Dictionary<string, TechGroup>();
        ActiveTechList = new List<TechGroup>();

        ActiveBuffs = new List<ActiveBuff>();
        ActiveBuffDictionary = new Dictionary<string, ActiveBuff>();

        for (int i = 0; i < debugTechSO.Count; i++) { EquipTech(debugTechSO[i]); } // Test

        List<PlayerTraitSO> selectedTraits = PersistentDataManager.GetTraitSelection();
        for (int i = 0; i < selectedTraits.Count; i++)
        {
            if (selectedTraits[i].Tech != null) { EquipTech(selectedTraits[i].Tech); }
        }

        EquipWeapon(new WeaponData(debugRangedWeaponSO, GameEnums.Rarity.Common));
        EquipWeapon(new WeaponData(debugMeleeWeaponSO, GameEnums.Rarity.Common));

        StatusOverheatRanged = StatusOverheatMelee = false;
        coolingReadyTimeMelee = coolingReadyTimeRanged = 0;
        StatusHeatMelee = StatusHeatRanged = -stats.GetStat(PlayerStatGroup.Stat.HeatFloor);
        rangedAttackReady = meleeAttackReady = 1;
        CurrentHealth = stats.GetStat(PlayerStatGroup.Stat.MaxHealth);
        CurrentDashes = (int)stats.GetStat(PlayerStatGroup.Stat.DashCount);
        DashRechargePercent = 0;

        CurrentLookDirection = 0; UpdateRotation();
        inputsAllowed = true;

        EventManager.OnPlayerStatsUpdated(this);

        EventManager.UiMenuFocusChangedEvent += OnUiMenuChanged;
    }
    private void OnDisable()
    {
        EventManager.UiMenuFocusChangedEvent -= OnUiMenuChanged;
        for (int i = 0; i < ActiveTechList.Count; i++) { ActiveTechList[i].Script?.OnTechRemoved(); }
    }
    private void OnUiMenuChanged(IGameMenu m) 
    {
        inputsAllowed = m == null;
    }
    private void UpdateRotation()
    {
        m_rotationParent.transform.rotation = Quaternion.Euler(0, CurrentLookDirection, 0);
    }
    private void Update()
    {
        movementInputH = InputManager.Instance.GetMovementInput().x;
        movementInputV = InputManager.Instance.GetMovementInput().y;
        rotationInputH = inputsAllowed ? InputManager.Instance.GetLookInput().x : 0;
        rotationInputV = inputsAllowed ? InputManager.Instance.GetLookInput().y : 0;

        CurrentLookDirection += rotationInputH * Time.deltaTime * ROTATION_SPEED;
        UpdateRotation();
        m_camVerticalRotationAxis.Rotate(-rotationInputV * Time.deltaTime * ROTATION_SPEED, 0, 0);
        ClampCamVerticalRotation();

        rangedAttackReady += Time.deltaTime * rangedWeaponStats.Firerate * stats.GetStat(PlayerStatGroup.Stat.Firerate);
        if (Time.time > coolingReadyTimeRanged) 
        {
            StatusHeatRanged = Mathf.MoveTowards(StatusHeatRanged, -stats.GetStat(PlayerStatGroup.Stat.HeatFloor), Time.deltaTime * heatDecayRanged);
            heatDecayRanged += Time.deltaTime * HEAT_DECAY_GROWTH;
        }
        if (StatusOverheatRanged && StatusHeatRanged <= 0) { StatusOverheatRanged = false; }

        meleeAttackReady += Time.deltaTime * meleeWeaponStats.Firerate * stats.GetStat(PlayerStatGroup.Stat.Firerate);
        if (Time.time > coolingReadyTimeMelee) 
        {
            heatDecayMelee += Time.deltaTime * HEAT_DECAY_GROWTH;
            StatusHeatMelee = Mathf.MoveTowards(StatusHeatMelee, -stats.GetStat(PlayerStatGroup.Stat.HeatFloor), Time.deltaTime * heatDecayMelee);
        }
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
        CurrentShield = Mathf.MoveTowards(CurrentShield, 0, Time.deltaTime * SHIELD_DECAY_RATE * (1 - Mathf.Clamp01(stats.GetStat(PlayerStatGroup.Stat.ShieldDecayRateReduction))));

        if (stats.GetFlag(PlayerStatGroup.PlayerFlags.DisableRangedAttack) <= 0) 
        {
            if ((InputManager.Instance.GetRangedAttackInput() || stats.GetFlag(PlayerStatGroup.PlayerFlags.NoStopAttackRanged) > 0) && rangedAttackReady >= 1 && !StatusOverheatRanged && inputsAllowed)
            {
                Attack(WeaponSO.WeaponSlot.Ranged, CurrentLookDirection, true, true);
            }
        }
        if (stats.GetFlag(PlayerStatGroup.PlayerFlags.DisableMeleeAttack) <= 0) 
        {
            if ((InputManager.Instance.GetMeleeAttackInput() || stats.GetFlag(PlayerStatGroup.PlayerFlags.NoStopAttackMelee) > 0) && meleeAttackReady >= 1 && !StatusOverheatMelee && inputsAllowed)
            {
                Attack(WeaponSO.WeaponSlot.Melee, CurrentLookDirection, true, true);
            }
        }

        if (InputManager.Instance.GetDashInput() && inputsAllowed) 
        {
            StartDash();
        }
        if (Time.time > buffUpdateTime) { UpdateBuffDurations(); }
        if (Time.time > nextTechTimeIntervalUpdate) { nextTechTimeIntervalUpdate = Time.time + 1; EventManager.OnPlayerFixedTimeInterval(); } 
    }
    public void Attack(WeaponSO.WeaponSlot s, float attackDir, bool generateHeat, bool canEcho)
    {
        if (defeated) { return; }

        EventManager.OnPlayerAttackStarted(transform.position, s, attackDir);

        if (generateHeat) 
        {
            if (s == WeaponSO.WeaponSlot.Melee)
            {
                heatDecayMelee = 0;
                meleeAttackReady = 0;
                coolingReadyTimeMelee = Time.time + COOLING_START_DELAY;
                ChangeWeaponHeat(meleeWeaponStats.HeatGen, WeaponSO.WeaponSlot.Melee);
            }
            else 
            {
                heatDecayRanged = 0;
                rangedAttackReady = 0;
                coolingReadyTimeRanged = Time.time + COOLING_START_DELAY;
                ChangeWeaponHeat(rangedWeaponStats.HeatGen, WeaponSO.WeaponSlot.Ranged);
            }
        }

        WeaponData.WeaponStats wpn = s == WeaponSO.WeaponSlot.Ranged ? rangedWeaponStats : meleeWeaponStats;
        float attackMagnitudeMultiplier = s == WeaponSO.WeaponSlot.Ranged ?
            GameTools.DexterityToDamageMultiplier(stats.GetStat(PlayerStatGroup.Stat.Dexterity)) : GameTools.MightToDamageMultiplier(stats.GetStat(PlayerStatGroup.Stat.Might));

        float baseRandomSpread = Mathf.Max(0, wpn.RandomSpread + stats.GetStat(PlayerStatGroup.Stat.RandomSpread));

        // Projectile component
        if (wpn.ProjectileComponentID != "")
        {
            float shootRandomSpread = Random.Range(-baseRandomSpread, baseRandomSpread);
            float currentMultishootAngle = -wpn.Arc / 2;
            for (int i = 0; i < wpn.Multishoot; i++)
            {
                WeaponAttackSetupData sd = new WeaponAttackSetupData()
                {
                    User = this,
                    Magnitude = wpn.ProjectileComponentMagnitude * attackMagnitudeMultiplier,
                    Bounces = wpn.Bounces + (int)stats.GetStat(PlayerStatGroup.Stat.BonusRangedBounces),
                    BuildupRate = wpn.BuildupRate,
                    CritDamage = wpn.CritMultiplier + stats.GetStat(PlayerStatGroup.Stat.CritBonusDamage),
                    CritChance = stats.GetStat(PlayerStatGroup.Stat.CritChance),
                    Pierces = wpn.Pierces + (int)stats.GetStat(PlayerStatGroup.Stat.BonusRangedPierces),
                    SizeMultiplier = wpn.SizeMultiplier,
                    Splash = wpn.Splash + stats.GetStat(PlayerStatGroup.Stat.BonusSplash),
                    Timescale = wpn.Timescale,
                    EnemyIgnored = -1,
                    Knockback = wpn.Knockback,
                    Element = wpn.Element,
                    ImpactEffectID = wpn.ImpactEffectID,
                    DamageType = s == WeaponSO.WeaponSlot.Melee ? GameEnums.DamageType.Melee : GameEnums.DamageType.Ranged
                    
                };
                ObjectPoolManager.GetPlayerAttackFromPool(wpn.ProjectileComponentID).SetUp(transform.position, attackDir + currentMultishootAngle + shootRandomSpread, sd, null);
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
                CritDamage = wpn.CritMultiplier + stats.GetStat(PlayerStatGroup.Stat.CritBonusDamage),
                CritChance = stats.GetStat(PlayerStatGroup.Stat.CritChance),
                Pierces = wpn.Pierces,
                SizeMultiplier = wpn.SizeMultiplier,
                Splash = wpn.Splash,
                Timescale = wpn.Timescale,
                EnemyIgnored = -1,
                Knockback = wpn.Knockback,
                Element = wpn.Element,
                ImpactEffectID = wpn.ImpactEffectID,
                DamageType = s == WeaponSO.WeaponSlot.Melee ? GameEnums.DamageType.Melee : GameEnums.DamageType.Ranged
            };
            ObjectPoolManager.GetPlayerAttackFromPool(wpn.CleaveComponentID).SetUp(transform.position, attackDir, sd, null);
        }

        if (stats.GetStat(PlayerStatGroup.Stat.EchoChance) > 0 && canEcho) 
        {
            if (Random.Range(0, 101) < stats.GetStat(PlayerStatGroup.Stat.EchoChance)) 
            {
                float echoRandomSpread = Random.Range(ECHO_RANDOM_SPREAD_MIN, ECHO_RANDOM_SPREAD_MAX);
                echoRandomSpread *= Random.Range(0, 2) == 0 ? 1 : -1;
                Attack(s, attackDir + echoRandomSpread, false, false);
            }
        }
    }
    private void FixedUpdate()
    {
        if (dashDurationRemaining > 0)
        {
            m_rb.linearVelocity = DASH_MOVEMENT_SPEED * stats.GetStat(PlayerStatGroup.Stat.Speed) * dashMovementVector;
            RaycastHit rh;
            LayerMask m = LayerMask.GetMask("Walls");
            Physics.Raycast(transform.position, dashMovementVector, out rh, DASH_SLAM_RAYCAST_LENGHT, m);
            dashDurationRemaining -= Time.fixedDeltaTime;
            if (rh.collider != null) 
            {
                dashDurationRemaining = 0;
                EventManager.OnPlayerWallSlam(transform.position);
                ObjectPoolManager.GetImpactEffectFromPool("SLAM").SetUp(Vector3.MoveTowards(rh.point, transform.position, DASH_SLAM_WALL_SEPARATION), GameTools.NormalToEuler(rh.normal)-90, GameEnums.DamageElement.NonElemental);
            }
            if (dashDurationRemaining <= 0) { EventManager.OnPlayerDashEnded(transform.position, GameTools.VectorToAngle(dashMovementVector)); }
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
    private void StartDash() 
    {
        if (CurrentDashes < 1) { return; }
        if (dashDurationRemaining > 0) { return; }

        CurrentDashes -= 1;


        dashMovementVector = (movementInputV == 0 && movementInputH == 0 ? 1 : movementInputV) * m_rotationParent.forward;
        dashMovementVector += movementInputH * m_rotationParent.right;
        dashDurationRemaining = DASH_DURATION;
        Debug.Log("Dashing");
        EventManager.OnPlayerDashStarted(transform.position, GameTools.VectorToAngle(dashMovementVector));
    }
    #region Public Methods
    public void EquipWeapon(WeaponData w)
    {
        switch (w.BaseWeapon.Slot)
        {
            case WeaponSO.WeaponSlot.Melee:
                {
                    if (MeleeWeapon != null)
                    {
                        ObjectPoolManager.GetWeaponPickupFromPool().SetUp(MeleeWeapon, transform.position);
                    }

                    MeleeWeapon = w;
                    meleeWeaponStats = MeleeWeapon.Stats;
                    break;
                }
            case WeaponSO.WeaponSlot.Ranged:
                {
                    if (RangedWeapon != null)
                    {
                        ObjectPoolManager.GetWeaponPickupFromPool().SetUp(RangedWeapon, transform.position);
                    }

                    RangedWeapon = w;
                    rangedWeaponStats = RangedWeapon.Stats;
                    break;
                }
        }
    }

    public void ChangeWeaponHeat(float change, WeaponSO.WeaponSlot s) 
    {
        
        change *= Mathf.Max(0, 1 + stats.GetStat(PlayerStatGroup.Stat.HeatGenIncrease));

        if (s == WeaponSO.WeaponSlot.Melee) 
        {
            if (change > 0)
            {
                StatusHeatMelee = Mathf.MoveTowards(StatusHeatMelee, stats.GetStat(PlayerStatGroup.Stat.HeatCap), change);
                if (StatusHeatMelee >= stats.GetStat(PlayerStatGroup.Stat.HeatCap)) { StatusOverheatMelee = true; EventManager.OnPlayerWeaponOverheated(WeaponSO.WeaponSlot.Melee); }
            }
            else 
            {
                StatusHeatMelee = Mathf.MoveTowards(StatusHeatMelee, -stats.GetStat(PlayerStatGroup.Stat.HeatFloor), -change);
            }
            
        }
        else
        {
            if (change > 0)
            {
                StatusHeatRanged = Mathf.MoveTowards(StatusHeatRanged, stats.GetStat(PlayerStatGroup.Stat.HeatCap), change);
                if (StatusHeatRanged >= stats.GetStat(PlayerStatGroup.Stat.HeatCap)) { StatusOverheatRanged = true; EventManager.OnPlayerWeaponOverheated(WeaponSO.WeaponSlot.Ranged); }
            }
            else
            {
                StatusHeatRanged = Mathf.MoveTowards(StatusHeatRanged, -stats.GetStat(PlayerStatGroup.Stat.HeatFloor), -change);
            }
        }
    }
    public void DealDamage(float magnitude, bool lethal, bool ignoreShield, bool ignoreEndurance)
    {
        if (defeated) { return; }

        if (!ignoreEndurance) { magnitude *= GameTools.EnduranceToDamageMultiplier(stats.GetStat(PlayerStatGroup.Stat.Endurance)); }

        if (!ignoreShield) 
        {
            float damageToShield = Mathf.Min(CurrentShield, magnitude);
            CurrentShield -= damageToShield;
            magnitude -= damageToShield;
            if (damageToShield > 0) { EventManager.OnPlayerDamageAbsorbed(damageToShield); }

        }

        CurrentHealth -= magnitude;
        AddLightDamage(magnitude * stats.GetStat(PlayerStatGroup.Stat.DamageToSoftDamageConversion));
        if (magnitude > 0) { EventManager.OnPlayerDamageTaken(magnitude); }

        if (CurrentHealth <= 0)
        {
            if (lethal) 
            {
                CurrentHealth = 1;
            }
            else 
            {
                CurrentHealth = 0;
                // Kill?
                EventManager.OnPlayerDefeated();
            }
        }
    }
    public void AddLightDamage(float amount)
    {
        if (defeated) { return; }

        CurrentLightDamage = Mathf.Clamp(CurrentLightDamage + amount, 0, stats.GetStat(PlayerStatGroup.Stat.MaxHealth) - CurrentHealth);
    }
    public void RemoveShield(float amount) 
    {
        amount = Mathf.Min(amount, CurrentShield);
        CurrentShield -= amount;
        EventManager.OnPlayerDamageAbsorbed(amount);
    }
    public void AddShield(float amount)
    {
        if (defeated) { return; }

        CurrentShield += amount;
        if (CurrentShield > stats.GetStat(PlayerStatGroup.Stat.MaxHealth))
        {
            CurrentShield = stats.GetStat(PlayerStatGroup.Stat.MaxHealth);
            EventManager.OnPlayerShieldCapped();
        }
    }
    public void Heal(float amount)
    {
        amount = Mathf.Max(amount, 0); // Healing cannot be negative

        if (defeated) { return; }
        if (stats.GetStat(PlayerStatGroup.Stat.HealingPrevention) > 0) 
        {
            float prevented = amount *= Mathf.Clamp01(stats.GetStat(PlayerStatGroup.Stat.HealingPrevention));
            amount -= prevented;
            EventManager.OnPlayerHealingPrevented(amount);
        }


        float lightDamageHealing = Mathf.Min(CurrentLightDamage / LIGHT_DAMAGE_HEALING_MULT, amount);
        CurrentLightDamage -= lightDamageHealing * LIGHT_DAMAGE_HEALING_MULT;
        amount -= lightDamageHealing;

        float totalHealing = lightDamageHealing * LIGHT_DAMAGE_HEALING_MULT;
        totalHealing += amount;
        CurrentHealth += totalHealing;
        if (CurrentHealth > stats.GetStat(PlayerStatGroup.Stat.MaxHealth)) 
        {
            EventManager.OnPlayerOverhealed(CurrentHealth-stats.GetStat(PlayerStatGroup.Stat.MaxHealth));
            CurrentHealth = stats.GetStat(PlayerStatGroup.Stat.MaxHealth);
        }

        EventManager.OnPlayerHealthRestored(totalHealing);
    }
    public void RestoreDashCharges(float dashes) 
    {
        DashRechargePercent += dashes;
        while (DashRechargePercent > 1 && CurrentDashes < stats.GetStat(PlayerStatGroup.Stat.DashCount)) { DashRechargePercent -= 1; CurrentDashes++; }
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
    #endregion
    #region Stat/Status Getters
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
    public bool GetFlag(PlayerStatGroup.PlayerFlags f) 
    {
        return stats.GetFlag(f) > 0;
    }
    public float GetHeatRange() 
    {
        return stats.GetStat(PlayerStatGroup.Stat.HeatFloor) + stats.GetStat(PlayerStatGroup.Stat.HeatCap);
    }
    public bool IsEvading()
    {
        return dashDurationRemaining > 0;
    }
    #endregion
    #region Tech Management
    public int GetTechLevel(string id)
    {
        if (ActiveTechDictionary.ContainsKey(id)) { return ActiveTechDictionary[id].Level; }
        return 0;
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
        for (int i = 0; i < tg.SO.BonusStageStats.Count; i++)
        {
            StageManagerBase.ChangeStageStat(tg.SO.BonusStageStats[i].First, tg.SO.BonusStageStats[i].Second);
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
            for (int i = 0; i < tg.SO.BonusStageStats.Count; i++)
            {
                StageManagerBase.ChangeStageStat(tg.SO.BonusStageStats[i].First, -tg.SO.BonusStageStats[i].Second * tg.Level);
            }
            ActiveTechDictionary[t.ID].Script?.OnTechRemoved();
            ActiveTechDictionary.Remove(t.ID);
            ActiveTechList.Remove(tg);
        }
        EventManager.OnPlayerStatsUpdated(this);
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
    #endregion
    #region Buff Management
    private void UpdateBuffDurations() 
    {
        for (int i = ActiveBuffs.Count-1; i >= 0; i--) 
        {
            if (ActiveBuffs[i].GetDurationRemaining() <= 0) 
            {
                ChangeBuffStacks(ActiveBuffs[i], -ActiveBuffs[i].Stacks);
                ActiveBuffDictionary.Remove(ActiveBuffs[i].SO.ID);
                ActiveBuffs.RemoveAt(i);
            }
        }
    }
    private BuffEffectSO GetBuffEffectInfo(string id)
    {
        if (!buffSODictionary.ContainsKey(id)) { return null; }

        return buffSODictionary[id];
    }
    private void SetUpBuffDictionary()
    {
        buffSODictionary = new Dictionary<string, BuffEffectSO>();
        for (int i = 0; i < database.BuffEffects.Count; i++)
        {
            buffSODictionary.Add(database.BuffEffects[i].ID, database.BuffEffects[i]);
        }
    }
    public void ChangePermanentStat(PlayerStatGroup.Stat s, float variation) 
    {
        stats.ChangeStat(s, variation);
        EventManager.OnPlayerStatsUpdated(this);
    }
    public void RemoveBuff(string buffID) 
    {
        for (int i = ActiveBuffs.Count-1; i >= 0; i--) 
        {
            if (ActiveBuffs[i].SO.ID == buffID) 
            {
                ChangeBuffStacks(ActiveBuffs[i], -ActiveBuffs[i].Stacks);
                ActiveBuffDictionary.Remove(ActiveBuffs[i].SO.ID);
                ActiveBuffs.RemoveAt(i);
                EventManager.OnPlayerStatsUpdated(this);
            }
        }
    }
    public void ChangeBuff(string buffID, float effectMultiplier, int stacks)
    {
        BuffEffectSO bso = GetBuffEffectInfo(buffID);
        if (bso == null) { return; }

        for (int i = ActiveBuffs.Count-1; i >= 0; i--)
        {
            if (ActiveBuffs[i].SO.ID == bso.ID)
            {
                if (ActiveBuffs[i].EffectMultiplier > effectMultiplier) { return; } // If the current buff has a higher effect multiplier, no changes.

                // If the effect multiplier is the same, change stacks normally
                if (ActiveBuffs[i].EffectMultiplier == effectMultiplier)
                {
                    ChangeBuffStacks(ActiveBuffs[i], stacks);
                    if (ActiveBuffs[i].Stacks <= 0)
                    {
                        ActiveBuffDictionary.Remove(ActiveBuffs[i].SO.ID);
                        ActiveBuffs.RemoveAt(i);
                    }
                    EventManager.OnPlayerStatsUpdated(this);
                    return;
                }
                // If the current buff has less effect multiplier than the new, remove the current buff. After the for loop is finished a new buff with the highest EM will be added.
                else
                {
                    ChangeBuffStacks(ActiveBuffs[i], -ActiveBuffs[i].Stacks);
                    ActiveBuffDictionary.Remove(ActiveBuffs[i].SO.ID);
                    ActiveBuffs.RemoveAt(i);
                    EventManager.OnPlayerStatsUpdated(this);
                }
            }
        }
        if (stacks <= 0) { return; }

        ActiveBuff newBuff = new ActiveBuff(bso, effectMultiplier);
        ChangeBuffStacks(newBuff, stacks);
        ActiveBuffs.Add(newBuff);
        ActiveBuffDictionary.Add(newBuff.SO.ID, newBuff);
        EventManager.OnPlayerStatsUpdated(this);
    }
    private void ChangeBuffStacks(ActiveBuff buff, int variation)
    {
        int previousStacks = buff.Stacks;
        buff.ChangeStacks(variation);
        int delta = buff.Stacks - previousStacks;
        if (delta != 0)
        {
            for (int i = 0; i < buff.SO.Effects.Count; i++) 
            {
                stats.ChangeStat(buff.SO.Effects[i].First, buff.SO.Effects[i].Second * buff.EffectMultiplier * delta);
            }
            for (int i = 0; i < buff.SO.Flags.Count; i++)
            {
                stats.ChangeFlag(buff.SO.Flags[i], delta);
            }
        }
    }
    public class ActiveBuff
    {
        public BuffEffectSO SO { get; private set; }
        public float EffectMultiplier { get; private set; }
        public int Stacks { get; private set; }
        public float RemoveTime { get; private set; }

        public ActiveBuff(BuffEffectSO bso, float _effectMult)
        {
            SO = bso;
            EffectMultiplier = _effectMult;
            RemoveTime = Time.time + SO.Duration;
        }
        public void ChangeStacks(int variation)
        {
            Stacks = Mathf.Clamp(Stacks + variation, 0, SO.MaxStacks);
            if (Stacks > 0) { RemoveTime = Time.time + SO.Duration; } // Update the duration if new stacks are added
        }
        public float GetDurationRemaining() { return RemoveTime - Time.time; }
    }
    #endregion
}
