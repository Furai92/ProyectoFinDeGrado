using UnityEngine;
using System.Collections.Generic;

public interface IMapData
{
    public List<Vector3> GetEnemySpawnPositions();
    public List<Vector3> GetChestSpawnPositions();
    public Vector3 GetPlayerSpawnPosition();
    public MapNode[,] GetLayoutMatrix();
    public List<MapNode> GetLayoutList();
    public List<StagePiece> GetStagePieces();
}
