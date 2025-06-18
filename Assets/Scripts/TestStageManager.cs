using UnityEngine;

public class TestStageManager : StageManagerBase
{
    protected override IMapData GenerateMap(int seed)
    {
        return new TestMapGeneration();
    }

    public override void InitializeStage()
    {

    }
}
