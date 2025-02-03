using UnityEngine;
using System.Collections.Generic;
using Utils;

public class PathfindingManager : MonoBehaviour
{
    public enum TerrainType { Ground, Pitfall, Wall }
    [SerializeField] private GameObject _debugDrawGridPrefab;

    TerrainType[,] _terrainGrid;
    PathfindingCellData[,] _pathfindingData;
    GameObject[,] debugDrawGridMatrix;

    private int _currentIteration;

    private const int GRID_SIZE = 50;
    private const float GRID_SPACING = 1f;
    private const float OVERLAP_CHECK_SIZE = 0.2f;
    private const float GRID_HEIGHT_OFFSET = 0.5f;
    private const float STAGE_WALL_SCAN_HEIGHT = 0.5f;
    private const float STAGE_GROUND_SCAN_HEIGHT = -2;
    private const float MOVEMENT_COST_STRAIGHT = 1f;
    private const float MOVEMENT_COST_DIAGONAL = 2f;

    public void Initialize() {

        GenerateTerrainGrid();
        _currentIteration = 0;
        _pathfindingData = new PathfindingCellData[GRID_SIZE, GRID_SIZE];
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                _pathfindingData[i, j] = new PathfindingCellData(i, j);
            }
        }
    }
    public Stack<Vector3> GetPath(Vector3 src, Vector3 dst, TerrainType movementTier) 
    {
        int srcx = Mathf.RoundToInt(src.x);
        int srcy = Mathf.RoundToInt(src.z);
        int dstx = Mathf.RoundToInt(dst.x);
        int dsty = Mathf.RoundToInt(dst.z);
        return GetPath(srcx, srcy, dstx, dsty, movementTier);
    }
    public Stack<Vector3> GetPath(int srcx, int srcy, int dstx, int dsty, TerrainType movementTier) 
    {
        if (srcx < 0 || srcy < 0 || dstx < 0 || dsty < 0) { return new Stack<Vector3>(); } // Out of bounds.
        if (srcx >= GRID_SIZE || srcy >= GRID_SIZE || dstx >= GRID_SIZE ||dsty >= GRID_SIZE) { return new Stack<Vector3>(); } // Out of bounds
        if (_terrainGrid[srcx, srcy] > movementTier) { return new Stack<Vector3>(); } // Invalid source
        if (_terrainGrid[dstx, dsty] > movementTier) { return new Stack<Vector3>(); } // Invalid destination

        _currentIteration++;
        PriorityQueue<Pair, float> openQueue = new PriorityQueue<Pair, float>();
        openQueue.Enqueue(new Pair((int)srcx, srcy), 1);
        bool endReached = false;

        while (openQueue.Count > 0 && !endReached) 
        {
            Pair current = openQueue.Dequeue();

            for (int i = -1; i <= 1; i++) 
            {
                for (int j = -1; j <= 1; j++) 
                {
                    int checkX = current.X + i;
                    int checkY = current.Y + j;

                    if (checkX < 0 || checkX >= GRID_SIZE) { continue; } // Out of bounds
                    if (checkY < 0 || checkY >= GRID_SIZE) { continue; } // Out of bounds
                    if (_terrainGrid[checkX, checkY] > movementTier) { continue; } // Invalid Terrain
                    if (_pathfindingData[checkX, checkY].iteration >= _currentIteration) { continue; } // Celda ya visitada

                    float movementCost = i != 0 && j != 0 ? MOVEMENT_COST_DIAGONAL : MOVEMENT_COST_STRAIGHT;
                    _pathfindingData[checkX, checkY].prev_x = current.X;
                    _pathfindingData[checkX, checkY].prev_y = current.Y;
                    _pathfindingData[checkX, checkY].g = _pathfindingData[current.X, current.Y].g + movementCost;
                    _pathfindingData[checkX, checkY].h = GetH(checkX, checkY, dstx, dsty);
                    _pathfindingData[checkX, checkY].f = _pathfindingData[checkX, checkY].g + _pathfindingData[checkX, checkY].h;
                    _pathfindingData[checkX, checkY].iteration = _currentIteration;
                    openQueue.Enqueue(new Pair(current.X + i, current.Y + j), _pathfindingData[checkX, checkY].f);

                    if (checkX == dstx && checkY == dsty) { endReached = true; }
                }
            }
        }

        if (endReached)
        {
            Stack<Vector3> path = new Stack<Vector3>();
            PathfindingCellData currentPathNode = _pathfindingData[dstx, dsty];
            path.Push(new Vector3(currentPathNode.x, GRID_HEIGHT_OFFSET, currentPathNode.y));
            while (currentPathNode.x != srcx || currentPathNode.y != srcy) 
            {
                currentPathNode = _pathfindingData[currentPathNode.prev_x, currentPathNode.prev_y];
                path.Push(new Vector3(currentPathNode.x, GRID_HEIGHT_OFFSET, currentPathNode.y));
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
        int gridx = Mathf.RoundToInt(position.x);
        int gridy = Mathf.RoundToInt(position.z);

        return _terrainGrid[gridx, gridy];
    }
    private void GenerateTerrainGrid() 
    {
        Collider[] wallCollisionsFound;
        Collider[] groundCollisionsFound;
        _terrainGrid = new TerrainType[GRID_SIZE, GRID_SIZE];
        LayerMask wallMask = LayerMask.GetMask("Walls");
        LayerMask groundMask = LayerMask.GetMask("Ground");

        for (int i = 0; i < GRID_SIZE; i++) 
        {
            for (int j = 0; j < GRID_SIZE; j++) 
            {
                // Scan for walls
                Vector3 wallCheckPosition = new Vector3(i * GRID_SPACING, STAGE_WALL_SCAN_HEIGHT, j * GRID_SPACING);
                wallCollisionsFound = Physics.OverlapBox(wallCheckPosition, new Vector3(OVERLAP_CHECK_SIZE, OVERLAP_CHECK_SIZE, OVERLAP_CHECK_SIZE), Quaternion.identity, wallMask);

                if (wallCollisionsFound.Length == 0)
                {
                    Vector3 groundCheckPosition = new Vector3(i * GRID_SPACING, STAGE_GROUND_SCAN_HEIGHT, j * GRID_SPACING);
                    groundCollisionsFound = Physics.OverlapBox(groundCheckPosition, new Vector3(OVERLAP_CHECK_SIZE, OVERLAP_CHECK_SIZE, OVERLAP_CHECK_SIZE), Quaternion.identity, groundMask);
                    _terrainGrid[i, j] = groundCollisionsFound.Length > 0 ? TerrainType.Ground : TerrainType.Pitfall;
                }
                else 
                {
                    _terrainGrid[i, j] = TerrainType.Wall;
                }
            }
        }
    }
    private void DebugDrawTerrainGrid() 
    {
        debugDrawGridMatrix = new GameObject[GRID_SIZE, GRID_SIZE];

        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                Vector3 drawPosition = new Vector3(i * GRID_SPACING, _terrainGrid[i,j] == TerrainType.Pitfall ? STAGE_GROUND_SCAN_HEIGHT : STAGE_WALL_SCAN_HEIGHT, j * GRID_SPACING);
                GameObject go = Instantiate(_debugDrawGridPrefab, transform) as GameObject;
                go.transform.position = drawPosition;
                go.SetActive(_terrainGrid[i, j] != TerrainType.Wall);
                debugDrawGridMatrix[i, j] = go;
            }
        }
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
