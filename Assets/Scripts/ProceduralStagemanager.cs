using UnityEngine;
using System.Collections.Generic;


public class ProceduralStagemanager : StageManagerBase
{
    [SerializeField] private Transform stagePartsInstParent;

    [SerializeField] private ProceduralStagePropertiesSO debugStageProperties;
    [SerializeField] private StageWaveSetupSO enemyWaveData;

    protected override IMapData GenerateMap(int seed)
    {
        return new ProceduralStageData(seed, 30, debugStageProperties, stagePartsInstParent);
    }

    public override void InitializeStage()
    {
        StageStateIntro introState = new StageStateIntro();
        StageStateShortDelay delayBeforeStart = new StageStateShortDelay();
        introState.SetNextState(delayBeforeStart);

        StageStateBase prevState = delayBeforeStart;
        StageStateBase lastState = introState;
        foreach (StageWaveSetupSO.EnemyWave ew in enemyWaveData.Waves) 
        {
            StageStateBase newCombatWave = new StageStateCombatWave(ew);
            StageStateBase newRestWave = new StageStateRest();
            prevState.SetNextState(newCombatWave);
            newCombatWave.SetNextState(newRestWave);
            prevState = newRestWave;
            lastState = newCombatWave;
        }
        StageStateVictory victoryState = new StageStateVictory();
        lastState.SetNextState(victoryState);

        currentState = introState;
    }

    protected override StageStatGroup GetInitialStageStats()
    {
        StageStatGroup ssg = new StageStatGroup();
        ssg.ChangeStat(StageStatGroup.StageStat.EnemyDamageMult, 1);
        ssg.ChangeStat(StageStatGroup.StageStat.EnemyHealthMult, 1);
        ssg.ChangeStat(StageStatGroup.StageStat.EnemySpawnRate, 1);
        ssg.ChangeStat(StageStatGroup.StageStat.HealthOrbDropRate, 1);
        ssg.ChangeStat(StageStatGroup.StageStat.ChestSpawnRate, 1);
        ssg.ChangeStat(StageStatGroup.StageStat.ChestDropRate, 1);
        ssg.ChangeStat(StageStatGroup.StageStat.ShopPriceMult, 1);
        ssg.ChangeStat(StageStatGroup.StageStat.WeaponDropRate, 1);

        return ssg;
    }
}
