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

    private const float HEAT_DECAY_GROWTH = 5f;

    // Stats and equipment =================================================================================

    private PlayerStatGroup stats;
    public WeaponData MeleeWeapon { get; private set; }
    public WeaponData RangedWeapon { get; private set; }

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

    // Buffs ===============================================================================================

    private Dictionary<string, BuffEffectSO> buffDictionary;
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

        ActiveTechDictionary = new Dictionary<string, TechGroup>();
        ActiveTechList = new List<TechGroup>();

        ActiveBuffs = new List<ActiveBuff>();

        for (int i = 0; i < debugTechSO.Count; i++) { EquipTech(debugTechSO[i]); } // Test

        List<PlayerTraitSO> selectedTraits = PersistentDataManager.GetTraitSelection();
        for (int i = 0; i < selectedTraits.Count; i++)
        {
            if (selectedTraits[i].Tech != null) { EquipTech(selectedTraits[i].Tech); }
        }

        EquipWeapon(new WeaponData(debugRangedWeaponSO, GameEnums.Rarity.Common));
        EquipWeapon(new WeaponData(debugMeleeWeaponSO, GameEnums.Rarity.Common));

        StatusOverheatRanged = StatusOverheatMelee = false;
        StatusHeatMelee = StatusHeatRanged = -stats.GetStat(PlayerStatGroup.Stat.HeatFloor);
        rangedAttackReady = meleeAttackReady = 1;
        CurrentHealth = stats.GetStat(PlayerStatGroup.Stat.MaxHealth);
        CurrentDashes = (int)stats.GetStat(PlayerStatGroup.Stat.DashCount);
        DashRechargePercent = 0;

        currentDirection = 0; UpdateRotation();
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
        StatusHeatRanged = Mathf.MoveTowards(StatusHeatRanged, -stats.GetStat(PlayerStatGroup.Stat.HeatFloor), Time.deltaTime * heatDecayRanged);
        if (StatusOverheatRanged && StatusHeatRanged <= 0) { StatusOverheatRanged = false; }

        meleeAttackReady += Time.deltaTime * meleeWeaponStats.Firerate * stats.GetStat(PlayerStatGroup.Stat.Firerate);
        heatDecayMelee += Time.deltaTime * HEAT_DECAY_GROWTH;
        StatusHeatMelee = Mathf.MoveTowards(StatusHeatMelee, -stats.GetStat(PlayerStatGroup.Stat.HeatFloor), Time.deltaTime * heatDecayMelee);
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
        CurrentShield = Mathf.MoveTowards(CurrentShield, 0, Time.deltaTime * SHIELD_DECAY_RATE);

        if (InputManager.Instance.GetRangedAttackInput() && rangedAttackReady >= 1 && !StatusOverheatRanged && inputsAllowed)
        {
            Attack(WeaponSO.WeaponSlot.Ranged, currentDirection, true);
        }
        if (InputManager.Instance.GetMeleeAttackInput() && meleeAttackReady >= 1 && !StatusOverheatMelee && inputsAllowed)
        {
            Attack(WeaponSO.WeaponSlot.Melee, currentDirection, true);
        }
        if (InputManager.Instance.GetDashInput() && inputsAllowed) 
        {
            StartDash();
        }
        if (Time.time > buffUpdateTime) { UpdateBuffDurations(); }
    }
    public void Attack(WeaponSO.WeaponSlot s, float attackDir, bool generateHeat)
    {
        if (defeated) { return; }
        EventManager.OnPlayerAttackStarted(transform.position, s, attackDir);
        if (generateHeat) 
        {
            if (s == WeaponSO.WeaponSlot.Melee)
            {
                heatDecayMelee = 0;
                meleeAttackReady = 0;
                ChangeWeaponHeat(meleeWeaponStats.HeatGen, WeaponSO.WeaponSlot.Melee);
            }
            else 
            {
                heatDecayRanged = 0;
                rangedAttackReady = 0;
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
    }
    private void FixedUpdate()
    {
        if (dashDurationRemaining > 0)
        {
            m_rb.linearVelocity = DASH_MOVEMENT_SPEED * stats.GetStat(PlayerStatGroup.Stat.Speed) * dashMovementVector;
            RaycastHit rh;
            LayerMask m = LayerMask.GetMask("Walls");
            Physics.Raycast(transform.position, dashMovementVector, out rh, DASH_SLAM_RAYCAST_LENGHT, m);
            if (rh.collider != null) 
            {
                dashDurationRemaining = 0;
                EventManager.OnPlayerWallSlam(transform.position);
                ObjectPoolManager.GetImpactEffectFromPool("SLAM").SetUp(Vector3.MoveTowards(rh.point, transform.position, DASH_SLAM_WALL_SEPARATION), GameTools.NormalToEuler(rh.normal)-90, GameEnums.DamageElement.NonElemental);
            }
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

        dashMovementVector = movementInputV * m_rotationParent.forward;
        dashMovementVector += movementInputH * m_rotationParent.right;
        dashDurationRemaining = DASH_DURATION;
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
        if (s == WeaponSO.WeaponSlot.Melee) 
        {
            if (change > 0)
            {
                change *= (1 + stats.GetStat(PlayerStatGroup.Stat.HeatGenIncrease));
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
                change *= (1 + stats.GetStat(PlayerStatGroup.Stat.HeatGenIncrease));
                StatusHeatRanged = Mathf.MoveTowards(StatusHeatRanged, stats.GetStat(PlayerStatGroup.Stat.HeatCap), change);
                if (StatusHeatRanged >= stats.GetStat(PlayerStatGroup.Stat.HeatCap)) { StatusOverheatRanged = true; EventManager.OnPlayerWeaponOverheated(WeaponSO.WeaponSlot.Ranged); }
            }
            else
            {
                StatusHeatRanged = Mathf.MoveTowards(StatusHeatRanged, -stats.GetStat(PlayerStatGroup.Stat.HeatFloor), -change);
            }
        }
    }
    public void DealDamage(float magnitude)
    {
        if (defeated) { return; }

        float damageToShield = Mathf.Min(CurrentShield, magnitude);
        CurrentShield -= damageToShield;
        magnitude -= damageToShield;
        if (damageToShield > 0) { EventManager.OnPlayerDamageAbsorbed(damageToShield); }

        CurrentHealth -= magnitude;
        AddShield(magnitude * stats.GetStat(PlayerStatGroup.Stat.DamageToShieldConversion));
        AddLightDamage(magnitude * stats.GetStat(PlayerStatGroup.Stat.DamageToLightConversion));
        if (magnitude > 0) { EventManager.OnPlayerDamageTaken(magnitude); }

        if (CurrentHealth <= 0)
        {
            EventManager.OnPlayerDefeated();
            // Kill?
        }
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
                ActiveBuffs.RemoveAt(i);
            }
        }
    }
    private BuffEffectSO GetBuffEffectInfo(string id)
    {
        if (!buffDictionary.ContainsKey(id)) { return null; }

        return buffDictionary[id];
    }
    private void SetUpBuffDictionary()
    {
        buffDictionary = new Dictionary<string, BuffEffectSO>();
        for (int i = 0; i < database.BuffEffects.Count; i++)
        {
            buffDictionary.Add(database.BuffEffects[i].ID, database.BuffEffects[i]);
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
            ChangeBuffStacks(ActiveBuffs[i], -ActiveBuffs[i].Stacks);
            if (ActiveBuffs[i].SO.ID == buffID) { ActiveBuffs.RemoveAt(i); }
        }
    }
    public void ChangeBuff(string buffID, float effectMultiplier, int stacks)
    {
        BuffEffectSO bso = GetBuffEffectInfo(buffID);
        if (bso == null) { return; }

        for (int i = ActiveBuffs.Count-1; i >= 0; i--)
        {
            if (ActiveBuffs[i].SO.ID == bso.ID && ActiveBuffs[i].EffectMultiplier == effectMultiplier)
            {
                ChangeBuffStacks(ActiveBuffs[i], stacks);
                if (ActiveBuffs[i].Stacks <= 0) { ActiveBuffs.RemoveAt(i); }
                EventManager.OnPlayerStatsUpdated(this);
                return;
            }
        }
        if (stacks <= 0) { return; }

        ActiveBuff newBuff = new ActiveBuff(bso, effectMultiplier);
        ChangeBuffStacks(newBuff, stacks);
        ActiveBuffs.Add(newBuff);
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
