using UnityEngine;
using System.Collections.Generic;

public abstract class StageManagerBase : MonoBehaviour
{
    [SerializeField] private PathfindingManager pathfindingMng;
    [SerializeField] private GameObject playerPrefab;

    private Vector3 playerPosition;
    private IMapData stageMapData;

    private static StageManagerBase _instance;

    public const int STAGE_SIZE = 100;

    private void OnEnable()
    {
        _instance = this;
        stageMapData = GenerateMap(0);
        SpawnPlayer(stageMapData.GetPlayerSpawnPosition());
        pathfindingMng.Initialize();
        InitializeStage();
    }

    private void SpawnPlayer(Vector3 pos) 
    {
        GameObject p = Instantiate(playerPrefab);
        p.transform.position = pos;
    }

    
    public static Stack<Vector3> GetPath(Vector3 src, Vector3 dst, PathfindingManager.TerrainType movementTier) 
    {
        return _instance == null ? new Stack<Vector3>() : _instance.pathfindingMng.GetPath(src, dst, movementTier);
    }
    public static void UpdatePlayerPosition(Vector3 newpos) 
    {
        if (_instance != null) { _instance.playerPosition = newpos; }  
    }
    public static Vector3 GetPlayerPosition() 
    {
        return _instance == null ? Vector3.zero : _instance.playerPosition;
    }

    public abstract IMapData GenerateMap(int seed);
    public abstract void InitializeStage();
}
