using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    [SerializeField] private Pathfinding _pf;

    [SerializeField] private TestAgent _ta1;
    [SerializeField] private TestAgent _ta2;

    private Vector3 playerPosition;

    private static StageManager _instance;

    private void OnEnable()
    {
        _instance = this;
        playerPosition = Vector3.zero;
        _pf.Initialize();
    }

    public static Stack<Vector3> GetPath(Vector3 src, Vector3 dst, Pathfinding.TerrainType movementTier) 
    {
        return _instance == null ? new Stack<Vector3>() : _instance._pf.GetPath(src, dst, movementTier);
    }
    public static void UpdatePlayerPosition(Vector3 newpos) 
    {
        if (_instance != null) { _instance.playerPosition = newpos; }  
    }
    public static Vector3 GetPlayerPosition() 
    {
        return _instance == null ? Vector3.zero : _instance.playerPosition;
    }

}
