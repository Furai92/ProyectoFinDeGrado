using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class StageManagerBase : MonoBehaviour
{
    [field: SerializeField] public StageWaveSetupSO StageWaveData { get; private set; }

    [SerializeField] protected PathfindingManager pathfindingMng;
    [SerializeField] protected ObjectPoolManager objectPoolMng;
    [SerializeField] protected IngameHudManager hudMng;
    [SerializeField] protected GameObject playerPrefab;
    [SerializeField] protected GameDatabaseSO database;

    [SerializeField] public int CombatWavesCompleted { get; private set; }
    private int playerRoomX = -1;
    private int playerRoomY = -1;
    private List<MapNode> validEnemySpawnNodes;
    private Vector3 playerPosition;
    protected List<EnemyEntity> enemiesInStage;
    protected StageStateBase currentState;
    private List<PlayerEntity> players;
    private List<float> playerCurrencies;
    protected IMapData stageMapData;
    private int enemyIDCounter;
    private bool initializationFinished = false;
    private Dictionary<string, List<WeaponSO>> weaponPools;
    protected StageStatGroup stageStats;

    private static StageManagerBase _instance;

    private float nextCurrencyDrop;
    private float nextWeaponDrop;
    private float nextHealthDrop;

    public const float MAX_BOUNCE_DISTANCE_MAG = 200f;
    public const int STAGE_SIZE = 100;

    private void OnEnable()
    {
        EventManager.EnemyDefeatedEvent += OnEnemyDefeated;
        StartCoroutine(Startcr());
    }
    private void OnDisable()
    {
        EventManager.EnemyDefeatedEvent -= OnEnemyDefeated;
    }
    private IEnumerator Startcr() 
    {
        _instance = this;
        SetUpItemPools();
        objectPoolMng.InitializePools();
        stageMapData = GenerateMap(Random.Range(0, 999999));
        stageStats = GetInitialStageStats();

        yield return new WaitForFixedUpdate(); // This is needed for collision overlap to work after spawning the map assets

        pathfindingMng.Initialize(stageMapData.GetStagePieces());
        enemiesInStage = new List<EnemyEntity>();
        SetupPlayers(stageMapData.GetPlayerSpawnPosition());
        hudMng.SetUp(players[0]); // TEMP
        InitializeStage();
        CombatWavesCompleted = 0;
        nextCurrencyDrop = Random.Range(0, 1);
        nextWeaponDrop = Random.Range(0, 1);

        initializationFinished = true;

        currentState.StateStart();
        EventManager.OnStageStateStarted(currentState);
    }
    private void Update()
    {
        if (!initializationFinished) { return; }

        currentState.UpdateState();
        if (currentState.IsFinished()) 
        {
            if (currentState.GetStateType() == StageStateBase.StateType.Combat) { CombatWavesCompleted++; }

            currentState.StateEnd();
            EventManager.OnStageStateEnded(currentState);
            currentState = currentState.GetNextState();
            currentState.StateStart();
            EventManager.OnStageStateStarted(currentState);
        }
    }

    private void SetupPlayers(Vector3 pos) 
    {
        players = new List<PlayerEntity>();
        playerCurrencies = new List<float>();
        GameObject p = Instantiate(playerPrefab);
        p.GetComponent<PlayerEntity>().SetUp(pos);
        players.Add(p.GetComponent<PlayerEntity>());
        playerCurrencies.Add(0);
    }
    private void SetUpItemPools() 
    {
        weaponPools = new Dictionary<string, List<WeaponSO>>();
        for (int i = 0; i < database.Weapons.Count; i++) 
        {
            WeaponSO weaponReaded = database.Weapons[i];
            for (int j = 0; j < weaponReaded.Pools.Count; j++) 
            {
                string poolReaded = weaponReaded.Pools[j];
                if (weaponPools.ContainsKey(poolReaded))
                {
                    weaponPools[poolReaded].Add(weaponReaded);
                }
                else 
                {
                    weaponPools.Add(poolReaded, new List<WeaponSO>() { weaponReaded });
                }
            }
        }
    }
    private void OnEnemyDefeated(EnemyEntity e) 
    {
        nextWeaponDrop += e.ItemDropRate;
        nextCurrencyDrop += e.CurrencyDropRate;
        nextHealthDrop += e.HealthDroprate;

        for (int i = 0; i < nextWeaponDrop; i++) 
        {
            ObjectPoolManager.GetWeaponPickupFromPool().SetUp(new WeaponData(GetWeaponFromItemPool("STANDARD")), e.transform.position);
            nextWeaponDrop -= 1;
        }
        for (int i = 0; i < nextCurrencyDrop; i++) 
        {
            ObjectPoolManager.GetCurrencyPickupFromPool().SetUp(e.transform.position);
            nextCurrencyDrop -= 1;
        }
        for (int i = 0; i < nextHealthDrop; i++) 
        {
            ObjectPoolManager.GetHealthPickupFromPool().SetUp(e.transform.position);
            nextHealthDrop -= 1;
        }
    }
    private void CalculateValidEnemySpawns(int px, int py) 
    {
        validEnemySpawnNodes = new List<MapNode>();

        MapNode playerPositionNode = _instance.stageMapData.GetLayoutMatrix()[px, py];

        for (int i = -2; i <= 2; i++)
        {
            MapNode readed;
            if (i == 0) { continue; } // Removes straight up, down, left, and right

            if (playerPositionNode.pos_y + i >= 0 && playerPositionNode.pos_y + i < STAGE_SIZE) // Removes Y out of bounds
            {
                if (playerPositionNode.pos_x + 2 < STAGE_SIZE) // Removes positive X out of bounds
                {
                    readed = _instance.stageMapData.GetLayoutMatrix()[playerPositionNode.pos_x + 2, playerPositionNode.pos_y + i];
                    if (readed.piece != null) { validEnemySpawnNodes.Add(readed); }
                }
                if (playerPositionNode.pos_x - 2 >= 0) // Removes negative X out of bounds
                {
                    readed = _instance.stageMapData.GetLayoutMatrix()[playerPositionNode.pos_x - 2, playerPositionNode.pos_y + i];
                    if (readed.piece != null) { validEnemySpawnNodes.Add(readed); }
                }
            }

            if (i < 1 || i > 1) { continue; } // Removes double checking corners

            if (playerPositionNode.pos_x + i >= 0 && playerPositionNode.pos_x + i < STAGE_SIZE) // Removes X out of bounds
            {
                if (playerPositionNode.pos_x + 2 < STAGE_SIZE) // Removes positive X out of bounds
                {
                    readed = _instance.stageMapData.GetLayoutMatrix()[playerPositionNode.pos_x + i, playerPositionNode.pos_y + 2];
                    if (readed.piece != null) { validEnemySpawnNodes.Add(readed); }
                }
                if (playerPositionNode.pos_x - 2 >= 0) // Removes negative X out of bounds
                {
                    readed = _instance.stageMapData.GetLayoutMatrix()[playerPositionNode.pos_x + i, playerPositionNode.pos_y - 2];
                    if (readed.piece != null) { validEnemySpawnNodes.Add(readed); }
                } 
            }
        }
        // For rare cases where enemies cannot be spawned following the previous logic
        if (validEnemySpawnNodes.Count == 0) 
        {
            if (playerPositionNode.pos_x + 2 < STAGE_SIZE && _instance.stageMapData.GetLayoutMatrix()[playerPositionNode.pos_x + 2, playerPositionNode.pos_y].piece != null) 
            {
                validEnemySpawnNodes.Add(_instance.stageMapData.GetLayoutMatrix()[playerPositionNode.pos_x + 2, playerPositionNode.pos_y]);
            }
            if (playerPositionNode.pos_x - 2 >= 0 && _instance.stageMapData.GetLayoutMatrix()[playerPositionNode.pos_x - 2, playerPositionNode.pos_y].piece != null)
            {
                validEnemySpawnNodes.Add(_instance.stageMapData.GetLayoutMatrix()[playerPositionNode.pos_x - 2, playerPositionNode.pos_y]);
            }
            if (playerPositionNode.pos_y + 2 < STAGE_SIZE && _instance.stageMapData.GetLayoutMatrix()[playerPositionNode.pos_x, playerPositionNode.pos_y + 2].piece != null)
            {
                validEnemySpawnNodes.Add(_instance.stageMapData.GetLayoutMatrix()[playerPositionNode.pos_x, playerPositionNode.pos_y + 2]);
            }
            if (playerPositionNode.pos_y - 2 >= 0 && _instance.stageMapData.GetLayoutMatrix()[playerPositionNode.pos_x, playerPositionNode.pos_y - 2].piece != null)
            {
                validEnemySpawnNodes.Add(_instance.stageMapData.GetLayoutMatrix()[playerPositionNode.pos_x, playerPositionNode.pos_y - 2]);
            }
        }
        // For extreme cases, debug only
        if (validEnemySpawnNodes.Count == 0) 
        {
            validEnemySpawnNodes.Add(_instance.stageMapData.GetLayoutList()[Random.Range(0, _instance.stageMapData.GetLayoutList().Count)]);
        }
    }
    #region Public Static Methods
    public static StageWaveSetupSO.EnemyWave GetCurrentWaveData() 
    {
        if (_instance == null) { return null; }

        return _instance.StageWaveData.Waves[Mathf.Min(_instance.CombatWavesCompleted, _instance.StageWaveData.Waves.Count-1)];
    }
    public static Vector3 GetRandomEnemySpawnPosition(int playerindex) 
    {
        if (_instance == null) { return Vector3.zero; }

        return _instance.validEnemySpawnNodes[Random.Range(0, _instance.validEnemySpawnNodes.Count)].piece.GetRandomEnemySpawnPosition();
    }
    public static WeaponSO GetWeaponFromItemPool(string pool) 
    {
        if (_instance == null) { return null; }

        if (_instance.weaponPools.ContainsKey(pool)) 
        {
            return _instance.weaponPools[pool][Random.Range(0, _instance.weaponPools[pool].Count)];
        }
        return null;
    }
    public static int RegisterEnemy(EnemyEntity e)
    {
        if (_instance != null) 
        {
            _instance.enemiesInStage.Add(e);
            _instance.enemyIDCounter++;
            return _instance.enemyIDCounter;
        }
        return -1;
    }
    public static StageStatGroup GetStageStats() 
    {
        if (_instance == null) { return new StageStatGroup(); }

        return _instance.stageStats;
    }
    public static StageStateBase.StateType GetCurrentStateType() 
    {
        if (_instance == null) { return StageStateBase.StateType.Rest; }

        return _instance.currentState.GetStateType();
    }
    public static float GetTimerDisplay() 
    {
        if (_instance == null) { return -1; }

        return _instance.currentState.GetTimerDisplay();
    }
    public static void ChangeCurrency(float amount) 
    {
        if (_instance == null) { return; }

        _instance.playerCurrencies[0] += amount;
        EventManager.OnCurrencyUpdated();
    }
    public static float GetPlayerCurrency(int playerindex) 
    {
        if (_instance == null) { return 0; }

        return _instance.playerCurrencies[0];
    }
    public static List<Vector3> GetEnemySpawnPositions() 
    {
        return _instance == null ? new List<Vector3>() : _instance.stageMapData.GetEnemySpawnPositions();
    }
    public static List<Vector3> GetChestSpawnPositions()
    {
        return _instance == null ? new List<Vector3>() : _instance.stageMapData.GetChestSpawnPositions();
    }
    public static int GetEnemyCount() 
    {
        return _instance == null ? 0 : _instance.enemiesInStage.Count;
    }
    public static void DisableAllEnemies() 
    {
        if (_instance == null) { return; }

        for (int i = _instance.enemiesInStage.Count-1; i >= 0; i--) 
        {
            _instance.enemiesInStage[i].gameObject.SetActive(false);
        }
    }
    public static void UnregisterEnemy(EnemyEntity e)
    {
        if (_instance != null) { _instance.enemiesInStage.Remove(e); }
    }
    public static Stack<Vector3> GetPath(Vector3 src, Vector3 dst) 
    {
        return _instance == null ? new Stack<Vector3>() : _instance.pathfindingMng.GetPath(src, dst);
    }
    public static void UpdatePlayerPosition(Vector3 newpos) 
    {
        if (_instance == null) { return; }

        _instance.playerPosition = newpos;
        int newRoomX = (int)((newpos.x + StagePiece.PIECE_SPACING/2) / StagePiece.PIECE_SPACING);
        int newRoomY = (int)((newpos.z + StagePiece.PIECE_SPACING/2) / StagePiece.PIECE_SPACING);

        if (newRoomX != _instance.playerRoomX || newRoomY != _instance.playerRoomY) 
        {
            _instance.playerRoomX = newRoomX;
            _instance.playerRoomY = newRoomY;
            _instance.CalculateValidEnemySpawns(newRoomX, newRoomY);
        }
    }
    public static Vector3 GetClosestPlayerPosition(Vector3 pos) 
    {
        return _instance == null ? Vector3.zero : _instance.playerPosition;
    }
    public static Vector3 GetRandomPlayerPosition()
    {
        return _instance == null ? Vector3.zero : _instance.playerPosition;
    }
    public static PlayerEntity GetPlayerReference(int pindex) 
    {
        return _instance == null ? null : _instance.players[Mathf.Min(_instance.players.Count-1, pindex)];
    }
    public static PlayerEntity GetClosestPlayer(Vector3 pos) 
    {
        if (_instance == null) { return null; }

        return _instance.players[0];
    }
    public static EnemyEntity GetClosestEnemyInRange(EnemyEntity ignored, Vector3 pos, float range)
    {
        if (_instance == null) { return null; }

        LayerMask mask = LayerMask.GetMask("EnemyHitboxes");
        Collider[] hitboxesFound = Physics.OverlapSphere(pos, range, mask);

        EnemyEntity closest = null;
        float closestMag = Mathf.Infinity;

        for (int i = 0; i < hitboxesFound.Length; i++) 
        {
            float mag = (pos - hitboxesFound[i].transform.position).sqrMagnitude;
            if (mag >= closestMag) { continue; }
            EnemyEntity readedEnemy = hitboxesFound[i].GetComponentInParent<EnemyEntity>();
            if (readedEnemy == ignored) { continue; }
            closest = readedEnemy;
            closestMag = mag;
        }
        return closest;
    }
    public static EnemyEntity GetRandomEnemyInRange(EnemyEntity ignored, Vector3 pos, float range) 
    {
        LayerMask mask = LayerMask.GetMask("EnemyHitboxes");
        Collider[] hitboxesFound = Physics.OverlapSphere(pos, range, mask);

        EnemyEntity enemySelected;

        if (hitboxesFound.Length == 0)
        {
            return null;
        }
        else
        {
            int randomIndexSelected = Random.Range(0, hitboxesFound.Length);
            enemySelected = hitboxesFound[Random.Range(0, hitboxesFound.Length)].GetComponentInParent<EnemyEntity>();
            if (enemySelected == ignored)
            {
                randomIndexSelected++;
                if (randomIndexSelected >= hitboxesFound.Length) { randomIndexSelected = 0; }
                enemySelected = hitboxesFound[randomIndexSelected].GetComponentInParent<EnemyEntity>();
                if (enemySelected == ignored)
                {
                    return null;
                }
            }
        }
        return enemySelected;
    }
    #endregion

    protected abstract StageStatGroup GetInitialStageStats();
    protected abstract IMapData GenerateMap(int seed);
    public abstract void InitializeStage();
}
