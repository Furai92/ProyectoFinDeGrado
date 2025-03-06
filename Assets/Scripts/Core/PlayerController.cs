using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Transform m_camVerticalRotationAxis;
    [SerializeField] private Transform m_rotationParent;
    [SerializeField] private Camera m_playerCamera;
    [SerializeField] private GameObject _cube;
    [SerializeField] private GameObject _capsule;
    [SerializeField] private Rigidbody m_rb;

    [field: SerializeField] public float StatusHeatRanged { get; private set; }
    [field: SerializeField] public float StatusHeatMelee { get; private set; }
    [field: SerializeField] public bool StatusOverheatRanged { get; private set; }
    [field: SerializeField] public bool StatusOverheatMelee { get; private set; }

    [field: SerializeField] public float StatMight { get; private set; }
    [field: SerializeField] public float StatDexterity { get; private set; }
    [field: SerializeField] public float StatEndurance { get; private set; }
    [field: SerializeField] public float StatIntellect { get; private set; }
    [field: SerializeField] public float StatSpeed { get; private set; }

    [field: SerializeField] public GameEnums.DamageElement RWElement { get; private set; }
    [field: SerializeField] public string RWAttackID { get; private set; }
    [field: SerializeField] public float RWMagnitude { get; private set; }
    [field: SerializeField] public float RWFirerate { get; private set; }
    [field: SerializeField] public int RWMultishoot { get; private set; }
    [field: SerializeField] public int RWBounces { get; private set; }
    [field: SerializeField] public int RWPierces { get; private set; }
    [field: SerializeField] public float RWSplash { get; private set; }
    [field: SerializeField] public float RWCritMultiplier { get; private set; }
    [field: SerializeField] public float RWBuildupRate { get; private set; }
    [field: SerializeField] public float RWReloadSpeed { get; private set; }
    [field: SerializeField] public float RWKnockback { get; private set; }
    [field: SerializeField] public float RWHeatGen { get; private set; }
    [field: SerializeField] public float RWTimescale { get; private set; }
    [field: SerializeField] public float RWSizeMultiplier { get; private set; }


    private float rangedAttackReady;
    private float meleeAttackReady;
    private float currentDirection;
    private float heatDecayMelee;
    private float heatDecayRanged;

    float movementInputH;
    float movementInputV;
    float rotationInputH;
    float rotationInputV;

    private const float HEAT_DECAY_GROWTH = 0.05f;
    private const float BASE_MOVEMENT_SPEED = 500f;
    private const float ROTATION_SPEED = 60f;
    private const float MIN_CAM_VERTICAL_ROTATION_X = 350f;
    private const float MAX_CAM_VERTICAL_ROTATION_X = 50f;
    private const float LOCK_Y = 1.0f;

    private bool _isPlayerControlsEnabled = false;

    private void OnEnable()
    {
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

        rangedAttackReady += Time.deltaTime * RWFirerate;
        heatDecayRanged += Time.deltaTime * HEAT_DECAY_GROWTH;
        StatusHeatRanged = Mathf.MoveTowards(StatusHeatRanged, 0, Time.deltaTime * heatDecayRanged);
        if (StatusOverheatRanged && StatusHeatRanged == 0) { StatusOverheatRanged = false; }

        if (InputManager.Instance.GetRangedAttackInput() && rangedAttackReady > 1 && !StatusOverheatRanged) 
        {
            heatDecayRanged = 0;
            StatusHeatRanged += RWHeatGen;
            if (StatusHeatRanged > 1) { StatusOverheatRanged = true; }
            WeaponAttackSetupData sd = new WeaponAttackSetupData()
            {
                User = this,
                Magnitude = RWMagnitude * GameTools.DexterityToDamageMultiplier(StatDexterity),
                Bounces = RWBounces,
                BuildupRate = RWBuildupRate,
                CritDamage = RWCritMultiplier,
                CritChance = 0,
                Pierces = RWPierces,
                SizeMultiplier = RWSizeMultiplier,
                Splash = RWSplash,
                Timescale = RWTimescale,
                EnemyIgnored = -1,
                Knockback = RWKnockback,
                Element = RWElement
            };

            StageManagerBase.GetObjectPool().GetPlayerAttackFromPool(RWAttackID).SetUp(transform.position, currentDirection, sd, null);
            rangedAttackReady = 0;
        }
    }
    private void FixedUpdate()
    {
        if (!_isPlayerControlsEnabled) return;

        m_rb.linearVelocity = movementInputV * Time.fixedDeltaTime * BASE_MOVEMENT_SPEED * StatSpeed * m_rotationParent.forward;
        m_rb.linearVelocity += movementInputH * Time.fixedDeltaTime * BASE_MOVEMENT_SPEED * StatSpeed * m_rotationParent.right;
        transform.position = new Vector3(transform.position.x, LOCK_Y, transform.position.z);
        StageManagerBase.UpdatePlayerPosition(transform.position);
    }
    private void ClampCamVerticalRotation() 
    {
        float x = m_camVerticalRotationAxis.localRotation.eulerAngles.x;
        x = x < 180 ? Mathf.Clamp(x, 0, MAX_CAM_VERTICAL_ROTATION_X) : Mathf.Clamp(x, MIN_CAM_VERTICAL_ROTATION_X, 360); 
        m_camVerticalRotationAxis.localRotation = Quaternion.Euler(x, 0, 0);
    }
}
