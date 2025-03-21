using UnityEngine;
using System.Collections.Generic;


public class ProceduralStagemanager : StageManagerBase
{
    [SerializeField] private Transform stagePartsInstParent;

    [SerializeField] private List<GameObject> roomPrefabs;
    [SerializeField] private List<GameObject> corridorPrefabs;
    [SerializeField] private List<GameObject> decoPrefabs;

    public override IMapData GenerateMap(int seed)
    {
        return new ProceduralStageData(seed, 30, roomPrefabs, corridorPrefabs, decoPrefabs, stagePartsInstParent);
    }

    public override void InitializeStage()
    {
        currentState = new StageStateEnemyWave(40);
        StageStateBase rest = new StageStateRest();
        currentState.SetNextState(rest);
        rest.SetNextState(currentState);
    }
}
