using UnityEngine;
using System.Collections.Generic;


public class ProceduralStagemanager : StageManagerBase
{
    [SerializeField] private Transform stagePartsInstParent;

    [SerializeField] private ProceduralStagePropertiesSO debugStageProperties;

    public override IMapData GenerateMap(int seed)
    {
        return new ProceduralStageData(seed, 30, debugStageProperties, stagePartsInstParent);
    }

    public override void InitializeStage()
    {
        currentState = new StageStateCombatWave(40);
        StageStateBase rest = new StageStateRest();
        currentState.SetNextState(rest);
        rest.SetNextState(currentState);
    }
}
