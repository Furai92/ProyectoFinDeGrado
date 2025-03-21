using System.Collections.Generic;
using UnityEngine;

public class TestMapGeneration : IMapData
{


    public TestMapGeneration() 
    {

    }

    public List<Vector3> GetChestSpawnPositions()
    {
        throw new System.NotImplementedException();
    }

    public List<Vector3> GetEnemySpawnPositions()
    {
        return new List<Vector3>() { Vector3.zero };
    }

    public MapNode[,] GetLayout()
    {
        throw new System.NotImplementedException();
    }

    public List<MapNode> GetLayoutList()
    {
        throw new System.NotImplementedException();
    }

    public MapNode[,] GetLayoutMatrix()
    {
        throw new System.NotImplementedException();
    }

    public Vector3 GetPlayerSpawnPosition()
    {
        throw new System.NotImplementedException();
    }

    public List<StagePiece> GetStagePieces()
    {
        throw new System.NotImplementedException();
    }
}
