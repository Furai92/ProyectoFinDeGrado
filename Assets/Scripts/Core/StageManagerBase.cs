using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class StageManagerBase : MonoBehaviour
{
    [SerializeField] protected PathfindingManager pathfindingMng;
    [SerializeField] protected ObjectPoolManager objectPoolMng;
    [SerializeField] protected IngameHudManager hudMng;
    [SerializeField] protected GameObject playerPrefab;

    private Vector3 playerPosition;
    protected List<EnemyEntity> enemiesInStage;
    protected StageStateBase currentState;
    private List<PlayerEntity> players;
    private List<float> playerCurrencies;
    protected IMapData stageMapData;
    private int enemyIDCounter;
    private bool initializationFinished;

    private static StageManagerBase _instance;

    public const float MAX_BOUNCE_DISTANCE_MAG = 200f;
    public const int STAGE_SIZE = 100;

    private void OnEnable()
    {
        initializationFinished = false;
        StartCoroutine(Startcr());
    }
    private IEnumerator Startcr() 
    {
        _instance = this;
        objectPoolMng.InitializePools();
        stageMapData = GenerateMap(Random.Range(0, 999999));
        yield return new WaitForFixedUpdate(); // This is needed for collision overlap to work after spawning the map assets
        pathfindingMng.Initialize(stageMapData.GetStagePieces());
        enemiesInStage = new List<EnemyEntity>();
        SetupPlayers(stageMapData.GetPlayerSpawnPosition());
        hudMng.SetUp(players[0]); // TEMP
        InitializeStage();
        initializationFinished = true;
        currentState.StateStart();
    }
    private void Update()
    {
        if (!initializationFinished) { return; }

        currentState.UpdateState();
        if (currentState.IsFinished()) 
        {
            currentState.StateEnd();
            currentState = currentState.GetNextState();
            currentState.StateStart();
        }
    }

    private void SetupPlayers(Vector3 pos) 
    {
        players = new List<PlayerEntity>();
        playerCurrencies = new List<float>();
        GameObject p = Instantiate(playerPrefab);
        p.transform.position = pos;
        players.Add(p.GetComponent<PlayerEntity>());
        playerCurrencies.Add(0);
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
    public static float GetTimerDisplay() 
    {
        if (_instance == null) { return -1; }

        return _instance.currentState.GetTimerDisplay();
    }
    public static void AddCurrency(float amount) 
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
        if (_instance != null) { _instance.playerPosition = newpos; }  
    }
    public static Vector3 GetClosestPlayerPosition(Vector3 pos) 
    {
        return _instance == null ? Vector3.zero : _instance.playerPosition;
    }
    public static Vector3 GetRandomPlayerPosition()
    {
        return _instance == null ? Vector3.zero : _instance.playerPosition;
    }
    public static PlayerEntity GetClosestPlayer(Vector3 pos) 
    {
        if (_instance == null) { return null; }

        return _instance.players[0];
    }
    public abstract IMapData GenerateMap(int seed);
    public abstract void InitializeStage();
}
