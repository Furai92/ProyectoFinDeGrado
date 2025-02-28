using UnityEngine;

public class TestStageManager : StageManagerBase
{
    public override IMapData GenerateMap(int seed)
    {
        return new TestMapGeneration();
    }

    public override void InitializeStage()
    {

    }
}
