using UnityEngine;
using System.Collections.Generic;

/// <summary>
///  Finds a path using A* from the enemy position to the nearest player.
///  This state ends after a step if the timer has run out or the path is empty.
/// </summary>
public class AiStatePathfindingSearch : AiStateBase
{
    private float timeoutTime;
    private float stateEndTime;
    private Stack<Vector3> path;

    private const float STATE_DURATION = 1f;
    private const float DISTANCE_TO_REACH_CHECKPOINT = 1.5f;
    private const float TIMEOUT = 2.5f;

    public AiStatePathfindingSearch() 
    {
    }

    public override void EndState(EnemyEntity e)
    {

    }

    public override void FixedUpdateState(EnemyEntity e)
    {
        if (path.Count > 0)
        {
            float distanceMag = (e.transform.position - path.Peek()).sqrMagnitude;
            if (distanceMag < DISTANCE_TO_REACH_CHECKPOINT)
            {
                if (Time.time > stateEndTime) { path.Clear(); }
                else 
                {
                    path.Pop();
                    if (path.Count > 0) 
                    {
                        e.TargetMovementPosition = path.Peek();
                        e.TargetLookPosition = path.Peek();
                    }
                }
            }
        }
    }

    public override bool IsFinished(EnemyEntity e)
    {
        return path.Count == 0 || Time.time > timeoutTime;
    }

    public override void StartState(EnemyEntity e)
    {
        path = StageManagerBase.GetPath(e.transform.position, PlayerEntity.ActiveInstance.transform.position);
        if (path.Count > 0) 
        {
            e.TargetMovementPosition = path.Peek();
            e.TargetLookPosition = path.Peek();
        }
        stateEndTime = Time.time + STATE_DURATION;
        timeoutTime = Time.time + TIMEOUT;
    }
}
