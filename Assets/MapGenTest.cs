using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;

public class MapGenTest : MonoBehaviour
{
    public string forcedSeed;
    public int seed;
    [SerializeField] private GameObject _elemPrefab;
    [SerializeField] private TextMeshProUGUI _seedInfo;
    [SerializeField] private Transform _instParent;

    [SerializeField] private List<GameObject> _roomPrefabs;
    [SerializeField] private List<GameObject> _corridorPrefabs;
    [SerializeField] private GameObject _decoPrefab;
    [SerializeField] private Transform _stageInstParent;


    private List<MapNode> _finalResultMapNodes;
    private List<StagePiece> _finalResultMapPieces;
    private MapNode[,] stageMap;

    private const float DECO_CHANCE_0 = 0.05f;
    private const float DECO_CHANCE_1 = 5f;
    private const float DECO_CHANCE_2 = 10f;
    private const float DECO_CHANCE_3 = 40f;
    private const float DECO_CHANCE_4 = 75f;
    private const float CHANCE_TO_2 = 90f;
    private const float CHANCE_TO_3 = 5f;
    private const float CHANCE_TO_4 = 0f;
    private const int MATRIX_SIZE = 50;
    private const int PLAYABLE_MAP_SIZE = 20;
    private const float ROOM_PERCENT = 0.4f;


    private void OnEnable()
    {
        //StartCoroutine(TestCR());
        // Initialize the map matrix
        stageMap = new MapNode[MATRIX_SIZE, MATRIX_SIZE];
        for (int i = 0; i < MATRIX_SIZE; i++)
        {
            for (int j = 0; j < MATRIX_SIZE; j++)
            {
                GameObject go = Instantiate(_elemPrefab, _instParent) as GameObject; // Debug only, remove later
                stageMap[i, j] = new MapNode(i, j, go.GetComponent<MapGenTestElement>());
            }
        }
        GenerateMap();
    }
    private void InstantiateMapPieces() 
    {
        if (_finalResultMapNodes == null) { return; }
        if (_finalResultMapPieces != null) { return; }

        _finalResultMapPieces = new List<StagePiece>();

        for (int i = 0; i < _finalResultMapNodes.Count; i++) 
        {
            switch (_finalResultMapNodes[i].currentType) 
            {
                case MapNode.RoomType.Room: 
                    {
                        int index = Mathf.Min(_roomPrefabs.Count - 1, _finalResultMapNodes[i].variation);
                        GameObject rp = Instantiate(_roomPrefabs[index], _stageInstParent) as GameObject;
                        rp.GetComponent<StagePiece>().SetUp(_finalResultMapNodes[i]);
                        _finalResultMapPieces.Add(rp.GetComponent<StagePiece>());
                        break;
                    }
                case MapNode.RoomType.Corridor:
                    {
                        int index = Mathf.Min(_corridorPrefabs.Count - 1, _finalResultMapNodes[i].variation);
                        GameObject rp = Instantiate(_corridorPrefabs[index], _stageInstParent) as GameObject;
                        rp.GetComponent<StagePiece>().SetUp(_finalResultMapNodes[i]);
                        _finalResultMapPieces.Add(rp.GetComponent<StagePiece>());
                        break;
                    }
                case MapNode.RoomType.Deco: 
                    {
                        GameObject rp = Instantiate(_decoPrefab, _stageInstParent) as GameObject;
                        rp.GetComponent<StagePiece>().SetUp(_finalResultMapNodes[i]);
                        _finalResultMapPieces.Add(rp.GetComponent<StagePiece>());
                        break;
                    }
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) 
        {
            GenerateMap();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            InstantiateMapPieces();
        }
        if (Input.GetKeyDown(KeyCode.R)) 
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    private void GenerateMap()
    {
        if (forcedSeed == "") 
        {
            seed = Random.Range(0, 99999999);
            _seedInfo.text = "seed: " + seed.ToString();
        } else 
        {
            seed = forcedSeed.GetHashCode();
            _seedInfo.text = "seed: " + forcedSeed + " (" + seed.ToString() + ")";
        }

        Random.InitState(seed);
        // Initialize the map matrix
        for (int i = 0; i < MATRIX_SIZE; i++)
        {
            for (int j = 0; j < MATRIX_SIZE; j++) {
                stageMap[i, j].currentType = MapNode.RoomType.None;
                stageMap[i, j].variation = -1;
            }
        }
        for (int i = 0; i < MATRIX_SIZE; i++)
        {
            for (int j = 0; j < MATRIX_SIZE; j++)
            {
                stageMap[i, j].UpdateConnections(stageMap);
            }
        }
        // Generate the layout
        List<MapNode> potentialCells = new List<MapNode>();
        List<MapNode> usedCells = new List<MapNode>();
        // Place the starting room at the center of the map
        MapNode initialCell = stageMap[MATRIX_SIZE / 2, MATRIX_SIZE / 2];
        initialCell.currentType = MapNode.RoomType.Potential;
        potentialCells.Add(initialCell);
        int roomsCreated = 0;
        while (roomsCreated < PLAYABLE_MAP_SIZE)
        {
            List<MapNode> randomlySelectedCells = new List<MapNode>();
            for (int i = 0; i < potentialCells.Count; i++) 
            {
                int nearbyUsedCells = potentialCells[i].GetNearbyUsedRooms(stageMap);
                float chanceToAdd;
                switch (nearbyUsedCells)
                {
                    case 2: { chanceToAdd = CHANCE_TO_2; break; }
                    case 3: { chanceToAdd = CHANCE_TO_3; break; }
                    case 4: { chanceToAdd = CHANCE_TO_4; break; }
                    default: { chanceToAdd = 100; break; }
                }
                if (potentialCells.Count == 1 || Random.Range(0, 100) < chanceToAdd) 
                {
                    randomlySelectedCells.Add(potentialCells[i]);
                }
            }

            int indexSelected = Random.Range(0, randomlySelectedCells.Count);
            MapNode currentCell = randomlySelectedCells[indexSelected];
            potentialCells.Remove(currentCell);
            usedCells.Add(currentCell);
            currentCell.currentType = MapNode.RoomType.Room;
            roomsCreated++;

            // Check nearby spaces
            for (int i = -1; i <= 1; i++) 
            {
                for (int j = -1; j <= 1; j++) 
                {
                    if (i != 0 && j != 0) { continue; } // No diagonals

                    int checkX = currentCell.pos_x + i;
                    int checkY = currentCell.pos_y + j;

                    if (checkX < 0) { continue; }
                    if (checkY < 0) { continue; }
                    if (checkX >= MATRIX_SIZE) { continue; }
                    if (checkY >= MATRIX_SIZE) { continue; }
                    if (stageMap[checkX, checkY].currentType != MapNode.RoomType.None) { continue; }
                    

                    potentialCells.Add(stageMap[checkX, checkY]);
                    stageMap[checkX, checkY].currentType = MapNode.RoomType.Potential;

                }
            }
        }
        // Update connections
        for (int i = 0; i < usedCells.Count; i++)
        {
            usedCells[i].UpdateConnections(stageMap);
        }
        // Find single connection cells
        List<MapNode> singleConnectionCells = new List<MapNode>();
        for (int i = 0; i < usedCells.Count; i++)
        {
            if (usedCells[i].connectionCount == 1) { singleConnectionCells.Add(usedCells[i]); }
        }
        // Create a list of cells that need to be added to keep the map cyclical
        List<MapNode> extraNeededCells = new List<MapNode>();
        for (int i = 0; i < singleConnectionCells.Count; i++)
        {
            // What's the only connection that this cell has?
            int dir;
            if (singleConnectionCells[i].con_right != null) { dir = 2; }
            else if (singleConnectionCells[i].con_up != null) { dir = 3; }
            else if (singleConnectionCells[i].con_left != null) { dir = 0; }
            else { dir = 1; }

            List<MapNode> option1 = new List<MapNode>();
            List<MapNode> option2 = new List<MapNode>();

            switch (dir)
            {
                case 0: // LEFT
                    {
                        if (singleConnectionCells[i].pos_y - 1 >= 0)
                        {
                            option1.Add(stageMap[singleConnectionCells[i].pos_x, singleConnectionCells[i].pos_y - 1]);
                            if (!stageMap[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y - 1].IsActive())
                            {
                                option1.Add(stageMap[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y - 1]);
                            }
                        }
                        if (singleConnectionCells[i].pos_y + 1 < MATRIX_SIZE)
                        {
                            option2.Add(stageMap[singleConnectionCells[i].pos_x, singleConnectionCells[i].pos_y + 1]);
                            if (!stageMap[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y + 1].IsActive())
                            {
                                option2.Add(stageMap[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y + 1]);
                            }
                        }
                        break;
                    }
                case 1: // UP
                    {
                        if (singleConnectionCells[i].pos_x - 1 >= 0)
                        {
                            option1.Add(stageMap[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y]);
                            if (!stageMap[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y - 1].IsActive())
                            {
                                option1.Add(stageMap[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y - 1]);
                            }
                        }
                        if (singleConnectionCells[i].pos_x + 1 < MATRIX_SIZE)
                        {
                            option2.Add(stageMap[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y]);
                            if (!stageMap[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y - 1].IsActive())
                            {
                                option2.Add(stageMap[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y - 1]);
                            }
                        }
                        break;
                    }
                case 2: // RIGHT
                    {
                        if (singleConnectionCells[i].pos_y - 1 >= 0)
                        {
                            option1.Add(stageMap[singleConnectionCells[i].pos_x, singleConnectionCells[i].pos_y - 1]);
                            if (!stageMap[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y - 1].IsActive())
                            {
                                option1.Add(stageMap[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y - 1]);
                            }
                        }
                        if (singleConnectionCells[i].pos_y + 1 < MATRIX_SIZE)
                        {
                            option2.Add(stageMap[singleConnectionCells[i].pos_x, singleConnectionCells[i].pos_y + 1]);
                            if (!stageMap[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y + 1].IsActive())
                            {
                                option2.Add(stageMap[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y + 1]);
                            }
                        }
                        break;
                    }
                case 3: // DOWN
                    {
                        if (singleConnectionCells[i].pos_x - 1 >= 0)
                        {
                            option1.Add(stageMap[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y]);
                            if (!stageMap[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y + 1].IsActive())
                            {
                                option1.Add(stageMap[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y + 1]);
                            }
                        }
                        if (singleConnectionCells[i].pos_x + 1 < MATRIX_SIZE)
                        {
                            option2.Add(stageMap[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y]);
                            if (!stageMap[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y + 1].IsActive())
                            {
                                option2.Add(stageMap[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y + 1]);
                            }
                        }
                        break;
                    }

            }

            if (option1.Count == option2.Count)
            {
                if (Random.Range(0, 2) == 0)
                {
                    for (int j = 0; j < option1.Count; j++) { extraNeededCells.Add(option1[j]); }
                }
                else
                {
                    for (int j = 0; j < option2.Count; j++) { extraNeededCells.Add(option2[j]); }
                }
            }
            else if (option1.Count < option2.Count)
            {
                for (int j = 0; j < option1.Count; j++) { extraNeededCells.Add(option1[j]); }
            }
            else
            {
                for (int j = 0; j < option2.Count; j++) { extraNeededCells.Add(option2[j]); }
            }


        }
        // Add extra cells (if needed)
        if (singleConnectionCells.Count > 0)
        {
            for (int i = 0; i < extraNeededCells.Count; i++)
            {
                if (stageMap[extraNeededCells[i].pos_x, extraNeededCells[i].pos_y].currentType == MapNode.RoomType.Room) { continue; } // Added by another iteration of this loop
                usedCells.Add(extraNeededCells[i]);
                extraNeededCells[i].currentType = MapNode.RoomType.Room;
                roomsCreated++;
            }
        }
        for (int i = 0; i < usedCells.Count; i++) { usedCells[i].UpdateConnections(stageMap); }
        
        // Checking for redundant cells
        List<MapNode> redundantCells = new List<MapNode>();
        for (int i = 0; i < usedCells.Count; i++)
        {
            if (usedCells[i].IsRedundant()) { redundantCells.Add(usedCells[i]); }
        }
        print("Redundant " + redundantCells.Count);
        while (redundantCells.Count > 0)
        {
            redundantCells[0].currentType = MapNode.RoomType.None;
            usedCells.Remove(redundantCells[0]);
            redundantCells.RemoveAt(0);

            for (int i = 0; i < usedCells.Count; i++) { usedCells[i].UpdateConnections(stageMap); }

            redundantCells = new List<MapNode>();
            for (int i = 0; i < usedCells.Count; i++)
            {
                if (usedCells[i].IsRedundant()) { redundantCells.Add(usedCells[i]); }
            }
        }
        // Update connections after deleting
        for (int i = 0; i < usedCells.Count; i++)
        {
            usedCells[i].UpdateConnections(stageMap);
        }
        // Balance the room to corridor ratio
        int maxRooms = (int)(usedCells.Count * ROOM_PERCENT);
        List<MapNode> convertibleCells = new List<MapNode>();
        for (int i = 0; i < usedCells.Count; i++) { convertibleCells.Add(usedCells[i]); }

        for (int i = 0; i < usedCells.Count - maxRooms; i++)
        {
            if (convertibleCells.Count > 0)
            {
                int index = Random.Range(0, convertibleCells.Count);
                convertibleCells[index].currentType = MapNode.RoomType.Corridor;
                convertibleCells.RemoveAt(index);
            }
        }
        // Set the variation to each room
        for (int i = 0; i < usedCells.Count; i++) 
        {
            if (usedCells[i].variation >= 0) { continue; }

            int variationID = Random.Range(0, 2);
            List<MapNode> variationGroup = new List<MapNode>();
            variationGroup.Add(usedCells[i]);
            while (variationGroup.Count > 0) 
            {
                MapNode current = variationGroup[0];
                variationGroup.RemoveAt(0);
                current.variation = variationID;
                if (current.con_up != null && current.currentType == current.con_up.currentType && current.con_up.variation < 0) { variationGroup.Add(current.con_up); }
                if (current.con_down != null && current.currentType == current.con_down.currentType && current.con_down.variation < 0) { variationGroup.Add(current.con_down); }
                if (current.con_left != null && current.currentType == current.con_left.currentType && current.con_left.variation < 0) { variationGroup.Add(current.con_left); }
                if (current.con_right != null && current.currentType == current.con_right.currentType && current.con_right.variation < 0) { variationGroup.Add(current.con_right); }
            }
        }
        // Setup decorative nodes
        /*
        for (int i = 0; i < MATRIX_SIZE; i++)
        {
            for (int j = 0; j < MATRIX_SIZE; j++)
            {
                if (stageMap[i, j].currentType != MapNode.RoomType.None && stageMap[i,j].currentType != MapNode.RoomType.Potential) { continue; }

                int nearby = stageMap[i, j].GetNearbyUsedRooms(stageMap);
                float chance;
                switch (nearby) 
                {

                    case 1: { chance = DECO_CHANCE_1; break; }
                    case 2: { chance = DECO_CHANCE_2; break; }
                    case 3: { chance = DECO_CHANCE_3; break; }
                    case 4: { chance = DECO_CHANCE_4; break; }
                    default: { chance = DECO_CHANCE_0; break; }
                }
                if (Random.Range(0, 100) < chance) 
                {
                    stageMap[i, j].currentType = MapNode.RoomType.Deco;
                    usedCells.Add(stageMap[i, j]);
                }
                
            }
        }
        */
        _finalResultMapNodes = usedCells;
        // Debug only
        for (int i = 0; i < MATRIX_SIZE; i++) 
        {
            for (int j = 0; j < MATRIX_SIZE; j++) 
            {
                stageMap[i, j].instance.Setup(stageMap[i, j]);
            }
        }
        print("Final count " + usedCells.Count);
    }
    public class MapNode
    {
        public enum RoomType { None, Potential, Room, Corridor, Deco }

        public RoomType currentType;
        public MapGenTestElement instance;
        public int variation;
        public int pos_x;
        public int pos_y;
        public int connectionCount;
        public MapNode con_up;
        public MapNode con_down;
        public MapNode con_left;
        public MapNode con_right;
        public bool IsActive() { return currentType != RoomType.None && currentType != RoomType.Potential; }

        public MapNode(int x, int y, MapGenTestElement inst)
        {
            pos_x = x;
            pos_y = y;
            variation = -1;
            instance = inst;
            currentType = RoomType.None;
            instance.Setup(this);
        }
        public void UpdateConnections(MapNode[,] m) 
        {
            // Reset the
            connectionCount = 0;

            if (pos_x - 1 >= 0 && m[pos_x - 1, pos_y].IsActive()) { con_left = m[pos_x - 1, pos_y]; connectionCount++; } else { con_left = null; }
            if (pos_x + 1 < MATRIX_SIZE && m[pos_x + 1, pos_y].IsActive()) { con_right = m[pos_x + 1, pos_y]; connectionCount++; } else { con_right = null; }
            if (pos_y - 1 >= 0 && m[pos_x, pos_y - 1].IsActive()) { con_down = m[pos_x, pos_y - 1]; connectionCount++; } else { con_down = null; }
            if (pos_y + 1 < MATRIX_SIZE && m[pos_x, pos_y + 1].IsActive()) { con_up = m[pos_x, pos_y + 1]; connectionCount++; } else { con_up = null; }
        }
        public int GetNearbyUsedRooms(MapNode[,] m)
        {
            int cons = 0;

            if (pos_x - 1 >= 0 && m[pos_x - 1, pos_y].currentType != RoomType.None) {  cons++; }
            if (pos_x + 1 < MATRIX_SIZE && m[pos_x + 1, pos_y].currentType!= RoomType.None) { cons++; }
            if (pos_y - 1 >= 0 && m[pos_x, pos_y - 1].currentType != RoomType.None) { cons++; }
            if (pos_y + 1 < MATRIX_SIZE && m[pos_x, pos_y + 1].currentType != RoomType.None) { cons++; }

            return cons;
        }
        public bool IsRedundant() 
        {
            if (connectionCount <= 3) { return false; }
            if (con_left != null && con_left.connectionCount <= 2) { return false; }
            if (con_right != null && con_right.connectionCount <= 2) { return false; }
            if (con_up != null && con_up.connectionCount <= 2) { return false; }
            if (con_down != null && con_down.connectionCount <= 2) { return false; }

            if (connectionCount == 4 && con_up.connectionCount <= 3 && con_left.connectionCount <= 3 && con_down.connectionCount <= 3 && con_right.connectionCount <= 3) { return false; } 

            return true;
        }
    }
    
}
