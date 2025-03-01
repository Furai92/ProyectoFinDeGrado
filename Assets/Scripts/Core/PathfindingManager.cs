using UnityEngine;
using System.Collections.Generic;
using Utils;

public class PathfindingManager : MonoBehaviour
{
    public enum TerrainType { Wall, Ground }
    [SerializeField] private GameObject _debugDrawGridPrefab;

    TerrainType[,] _terrainGrid;
    PathfindingCellData[,] _pathfindingData;
    GameObject[,] debugDrawGridMatrix;

    private int _currentIteration;

    private const int NODE_GRID_SIZE = 10;
    private const float OVERLAP_CHECK_SIZE = 0.2f;
    private const float STAGE_WALL_SCAN_HEIGHT = 0.5f;
    private const float MOVEMENT_COST_STRAIGHT = 1f;
    private const float MOVEMENT_COST_DIAGONAL = 2f;

    public void Initialize(List<StagePiece> stagePieces) {

        GenerateTerrainGrid(stagePieces);
        _currentIteration = 0;
        _pathfindingData = new PathfindingCellData[GetGridSize(), GetGridSize()];
        for (int i = 0; i < GetGridSize(); i++)
        {
            for (int j = 0; j < GetGridSize(); j++)
            {
                _pathfindingData[i, j] = new PathfindingCellData(i, j);
            }
        }
    }
    public Stack<Vector3> GetPath(Vector3 src, Vector3 dst) 
    {
        Vector2Int srcVector = WorldToGraph(src);
        Vector2Int dstVector = WorldToGraph(dst);
        return GetPath(srcVector, dstVector);
    }
    private Stack<Vector3> GetPath(Vector2Int src, Vector2Int dst) 
    {
        if (src.x < 0 || src.y < 0 || dst.x < 0 || dst.y < 0) { return new Stack<Vector3>(); } // Out of bounds.
        if (src.x >= GetGridSize() || src.y >= GetGridSize() || dst.x >= GetGridSize() || dst.y >= GetGridSize()) { return new Stack<Vector3>(); } // Out of bounds

        _currentIteration++;
        PriorityQueue<Vector2Int, float> openQueue = new PriorityQueue<Vector2Int, float>();
        openQueue.Enqueue(new Vector2Int(src.x, src.y), 1);
        bool endReached = false;

        while (openQueue.Count > 0 && !endReached) 
        {
            Vector2Int current = openQueue.Dequeue();

            for (int i = -1; i <= 1; i++) 
            {
                for (int j = -1; j <= 1; j++) 
                {
                    int checkX = current.x + i;
                    int checkY = current.y + j;

                    if (checkX < 0 || checkX >= GetGridSize()) { continue; } // Out of bounds
                    if (checkY < 0 || checkY >= GetGridSize()) { continue; } // Out of bounds
                    if (_terrainGrid[checkX, checkY] == TerrainType.Wall) { continue; } // Invalid Terrain
                    if (_pathfindingData[checkX, checkY].iteration >= _currentIteration) { continue; } // Already visited
                    if (i != 0 && j != 0) // Logic to prevent diagonals from going trough walls
                    {
                        if (_terrainGrid[checkX, checkY-j] == TerrainType.Wall || _terrainGrid[checkX-i, checkY] == TerrainType.Wall) { continue; }
                    }

                    float movementCost = i != 0 && j != 0 ? MOVEMENT_COST_DIAGONAL : MOVEMENT_COST_STRAIGHT;
                    _pathfindingData[checkX, checkY].prev_x = current.x;
                    _pathfindingData[checkX, checkY].prev_y = current.y;
                    _pathfindingData[checkX, checkY].g = _pathfindingData[current.x, current.y].g + movementCost;
                    _pathfindingData[checkX, checkY].h = GetH(checkX, checkY, dst.x, dst.y);
                    _pathfindingData[checkX, checkY].f = _pathfindingData[checkX, checkY].g + _pathfindingData[checkX, checkY].h;
                    _pathfindingData[checkX, checkY].iteration = _currentIteration;
                    openQueue.Enqueue(new Vector2Int(current.x + i, current.y + j), _pathfindingData[checkX, checkY].f);

                    if (checkX == dst.x && checkY == dst.y) { endReached = true; }
                }
            }
        }

        if (endReached)
        {
            Stack<Vector3> path = new Stack<Vector3>();
            PathfindingCellData currentPathNode = _pathfindingData[dst.x, dst.y];
            path.Push(GraphToWorld(currentPathNode.x, currentPathNode.y));
            while (currentPathNode.x != src.x || currentPathNode.y != src.y) 
            {
                currentPathNode = _pathfindingData[currentPathNode.prev_x, currentPathNode.prev_y];
                path.Push(GraphToWorld(currentPathNode.x, currentPathNode.y));
            }
            return path;
        }
        print("Path not found");
        return new Stack<Vector3>();
    }
    private float GetH(float srcx, float srcy, float dstx, float dsty) 
    {
        return Mathf.Sqrt(Mathf.Pow(srcx - dstx, 2) + Mathf.Pow(srcy - dsty, 2));
    }
    public TerrainType GetTerrainType(Vector3 position) 
    {
        Vector2Int matpos = WorldToGraph(position);

        return _terrainGrid[matpos.x, matpos.y];
    }
    public Vector2Int WorldToGraph(Vector3 wpos) 
    {
        int adjustedX = (int)(wpos.x / (StagePiece.PIECE_SPACING / NODE_GRID_SIZE));
        int adjustedY = (int)(wpos.z / (StagePiece.PIECE_SPACING / NODE_GRID_SIZE));
        return new Vector2Int(adjustedX, adjustedY);
    }
    public Vector3 GraphToWorld(int x, int y) 
    {
        float tileSpacing = (StagePiece.PIECE_SPACING / NODE_GRID_SIZE);
        return new Vector3((x + 0.5f) * tileSpacing, 0, (y + 0.5f) * tileSpacing);
    }
    private void GenerateTerrainGrid(List<StagePiece> stagePieces) 
    {
        _terrainGrid = new TerrainType[GetGridSize(), GetGridSize()];
        LayerMask wallMask = LayerMask.GetMask("Walls");

        foreach (StagePiece sp in stagePieces) 
        {
            for (int i = 0; i < NODE_GRID_SIZE; i++) 
            {
                for (int j = 0; j < NODE_GRID_SIZE; j++) 
                {
                    float checkX = sp.transform.position.x - (StagePiece.PIECE_SPACING / 2) + (StagePiece.PIECE_SPACING / NODE_GRID_SIZE/2) + (i * StagePiece.PIECE_SPACING / NODE_GRID_SIZE);
                    float checkY = sp.transform.position.z - (StagePiece.PIECE_SPACING / 2) + (StagePiece.PIECE_SPACING / NODE_GRID_SIZE/2) + (j * StagePiece.PIECE_SPACING / NODE_GRID_SIZE);
                    Vector3 checkPosition = new Vector3(checkX, STAGE_WALL_SCAN_HEIGHT, checkY);
                    Vector2Int matrixPos = WorldToGraph(checkPosition);

                    if(  Physics.OverlapBox(checkPosition, Vector3.one * OVERLAP_CHECK_SIZE, Quaternion.identity, wallMask).Length == 0)
                    {
                        _terrainGrid[matrixPos.x, matrixPos.y] = TerrainType.Ground;

                        //GameObject go = GameObject.Instantiate(_debugDrawGridPrefab, transform); // Debug only
                        //go.transform.position = new Vector3(checkX, 0.1f, checkY);
                    } 
                    else 
                    {
                        _terrainGrid[matrixPos.x, matrixPos.y] = TerrainType.Wall;
                    }
                }
            }
        }
    }
    private int GetGridSize() 
    {
        return StageManagerBase.STAGE_SIZE * NODE_GRID_SIZE;
    }
    public struct Pair
    {
        public int X;
        public int Y;

        public Pair(int _x, int _y) 
        {
            X = _x;
            Y = _y;
        }
    }
    public struct PathfindingCellData
    {
        public int iteration;
        public int x;
        public int y;
        public int prev_x;
        public int prev_y;
        public float h;
        public float g;
        public float f;

        public PathfindingCellData(int _x, int _y)
        {
            iteration = -1;
            x = _x;
            y = _y;
            prev_x = -1;
            prev_y = -1;
            g = 0;
            h = Mathf.Infinity;
            f = Mathf.Infinity;
        }
    }
}
