using UnityEngine;
using System.Collections.Generic;

public class TestAgent : MonoBehaviour
{
    [SerializeField] private Pathfinding.TerrainType movementTier;

    private const float SPEED = 5f;
    private const float FIND_PATH_COOLDOWN = 0.5f;

    private float _pathRequestAvailable;
    private Stack<Vector3> _path;

    private void OnEnable()
    {
        _path = new Stack<Vector3>();
        _pathRequestAvailable = 0;
    }
    private void FixedUpdate()
    {
        if (_path.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, _path.Peek(), Time.fixedDeltaTime * SPEED);
            if (transform.position == _path.Peek()) 
            {
                _path.Pop();
                if (Time.time > _pathRequestAvailable) { RequestNewPath(); }
            }
        }
        else 
        {
            if (Time.time > _pathRequestAvailable)
            {
                RequestNewPath();
            }
        }
    }
    private void RequestNewPath() 
    {
        _pathRequestAvailable = Time.time + FIND_PATH_COOLDOWN;
        _path = StageManager.GetPath(transform.position, StageManager.GetPlayerPosition(), movementTier);
    }
}
