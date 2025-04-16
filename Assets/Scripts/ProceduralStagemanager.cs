using UnityEngine;
using System.Collections.Generic;


public class ProceduralStagemanager : StageManagerBase
{
    [SerializeField] private Transform stagePartsInstParent;

    [SerializeField] private ProceduralStagePropertiesSO debugStageProperties;

    protected override IMapData GenerateMap(int seed)
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

    protected override StageStatGroup GetInitialStageStats()
    {
        StageStatGroup ssg = new StageStatGroup();
        ssg.ChangeStat(StageStatGroup.StageStat.EnemyDamageMult, 1);
        ssg.ChangeStat(StageStatGroup.StageStat.EnemyHealthMult, 1);
        ssg.ChangeStat(StageStatGroup.StageStat.EnemySpawnRate, 1);

        return ssg;
    }
}
