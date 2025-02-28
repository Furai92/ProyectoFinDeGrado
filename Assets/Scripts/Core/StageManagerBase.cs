using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class StageManagerBase : MonoBehaviour
{
    [SerializeField] private PathfindingManager pathfindingMng;
    [SerializeField] private GameObject playerPrefab;

    private Vector3 playerPosition;
    private List<PlayerController> players;
    private IMapData stageMapData;

    private static StageManagerBase _instance;

    public const int STAGE_SIZE = 100;

    private void OnEnable()
    {
        StartCoroutine(Startcr());
        
    }
    private IEnumerator Startcr() 
    {
        _instance = this;
        stageMapData = GenerateMap(Random.Range(0, 999999));
        yield return new WaitForFixedUpdate();
        SpawnPlayers(stageMapData.GetPlayerSpawnPosition());
        pathfindingMng.Initialize(stageMapData.GetStagePieces());
        InitializeStage();
    }

    private void SpawnPlayers(Vector3 pos) 
    {
        players = new List<PlayerController>();
        GameObject p = Instantiate(playerPrefab);
        p.transform.position = pos;
        players.Add(p.GetComponent<PlayerController>());
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
    public static PlayerController GetClosestPlayer(Vector3 pos) 
    {
        if (_instance == null) { return null; }

        return _instance.players[0];
    }

    public abstract IMapData GenerateMap(int seed);
    public abstract void InitializeStage();
}
