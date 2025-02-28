using UnityEngine;
using System.Collections.Generic;


public class ProceduralStagemanager : StageManagerBase
{
    [SerializeField] private List<GameObject> roomPrefabs;
    [SerializeField] private List<GameObject> corridorPrefabs;
    [SerializeField] private List<GameObject> decoPrefabs;

    public override IMapData GenerateMap(int seed)
    {
        return new ProceduralStageData(seed, 30, roomPrefabs, corridorPrefabs, decoPrefabs, transform);
    }

    public override void InitializeStage()
    {

    }

}
