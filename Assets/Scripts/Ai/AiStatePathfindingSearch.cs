using UnityEngine;
using System.Collections.Generic;

/// <summary>
///  Finds a path using A* from the enemy position to the nearest player.
///  This state ends after a step if the timer has run out or the path is empty.
/// </summary>
public class AiStatePathfindingSearch : AiStateBase
{
    private float stateEndTime;
    private Stack<Vector3> path;

    private const float STATE_DURATION = 2f;
    private const float DISTANCE_TO_REACH_CHECKPOINT = 1.5f;

    public AiStatePathfindingSearch(EnemyEntity e) 
    {
        EnemyControlled = e;
    }

    public override void EndState()
    {

    }

    public override void FixedUpdateState()
    {
        if (path.Count > 0)
        {
            float distanceMag = (EnemyControlled.transform.position - path.Peek()).sqrMagnitude;
            if (distanceMag < DISTANCE_TO_REACH_CHECKPOINT)
            {
                if (Time.time > stateEndTime) { path.Clear(); }
                else 
                {
                    path.Pop();
                    if (path.Count > 0) 
                    {
                        EnemyControlled.TargetMovementPosition = path.Peek();
                        EnemyControlled.TargetLookPosition = path.Peek();
                    }
                }
            }
        }
    }

    public override bool IsFinished()
    {
        return path.Count == 0;
    }

    public override void StartState()
    {
        path = StageManagerBase.GetPath(EnemyControlled.transform.position, StageManagerBase.GetClosestPlayerPosition(EnemyControlled.transform.position));
        if (path.Count > 0) 
        {
            EnemyControlled.TargetMovementPosition = path.Peek();
            EnemyControlled.TargetLookPosition = path.Peek();
        }
        stateEndTime = Time.time + STATE_DURATION;
    }
}
