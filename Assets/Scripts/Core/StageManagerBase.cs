using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class StageManagerBase : MonoBehaviour
{
    [SerializeField] protected PathfindingManager pathfindingMng;
    [SerializeField] protected ObjectPoolManager objectPoolMng;
    [SerializeField] protected GameObject playerPrefab;

    private Vector3 playerPosition;
    protected List<EnemyEntity> enemiesInStage;
    private List<PlayerController> players;
    protected IMapData stageMapData;
    private int enemyIDCounter;

    private static StageManagerBase _instance;

    public const float MAX_BOUNCE_DISTANCE_MAG = 200f;
    public const int STAGE_SIZE = 100;

    private void OnEnable()
    {
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
        SpawnPlayers(stageMapData.GetPlayerSpawnPosition());
        InitializeStage();
    }

    private void SpawnPlayers(Vector3 pos) 
    {
        players = new List<PlayerController>();
        GameObject p = Instantiate(playerPrefab);
        p.transform.position = pos;
        players.Add(p.GetComponent<PlayerController>());
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
    public static EnemyEntity GetBounceTarget(Vector3 pos, int ignored = -1)
    {
        if (_instance == null) { return null; }

        EnemyEntity selected = null;
        float bestMag = Mathf.Infinity;
        for (int i = 0; i < _instance.enemiesInStage.Count; i++) 
        {
            if (_instance.enemiesInStage[i].EnemyInstanceID == ignored) { continue; }

            float mag = (_instance.enemiesInStage[i].transform.position - pos).sqrMagnitude;
            if (mag < bestMag && mag < MAX_BOUNCE_DISTANCE_MAG) { bestMag = mag; selected = _instance.enemiesInStage[i]; }
        }
        return selected;
    }
    public static Vector3 GetClosestPlayerPosition(Vector3 pos) 
    {
        return _instance == null ? Vector3.zero : _instance.playerPosition;
    }
    public static Vector3 GetRandomPlayerPosition()
    {
        return _instance == null ? Vector3.zero : _instance.playerPosition;
    }
    public static PlayerController GetClosestPlayer(Vector3 pos) 
    {
        if (_instance == null) { return null; }

        return _instance.players[0];
    }
    public static ObjectPoolManager GetObjectPool() 
    {
        if (_instance == null) { return null; }

        return _instance.objectPoolMng;
    }

    public abstract IMapData GenerateMap(int seed);
    public abstract void InitializeStage();
}
