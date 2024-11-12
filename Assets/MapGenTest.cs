using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class MapGenTest : MonoBehaviour
{
    public int seed;
    [SerializeField] private GameObject _elemPrefab;
    [SerializeField] private TextMeshProUGUI _stepinfo;
    [SerializeField] private Transform _instParent;
    private MapGenNodeData[,] map;
    private MapNode[,] mapClean;

    private const int MATRIX_SIZE = 50;
    private const int PLAYABLE_MAP_SIZE = 20;
    private const float ROOM_PERCENT = 0.4f;


    private void OnEnable()
    {
        //StartCoroutine(TestCR());
        // Initialize the map matrix
        mapClean = new MapNode[MATRIX_SIZE, MATRIX_SIZE];
        for (int i = 0; i < MATRIX_SIZE; i++)
        {
            for (int j = 0; j < MATRIX_SIZE; j++)
            {
                GameObject go = Instantiate(_elemPrefab, _instParent) as GameObject; // Debug only, remove later
                mapClean[i, j] = new MapNode(i, j, go.GetComponent<MapGenTestElement>());
            }
        }
        GenerateMap();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) { GenerateMap(); }
    }
    private void GenerateMap()
    {
        // Initialize the map matrix
        for (int i = 0; i < MATRIX_SIZE; i++)
        {
            for (int j = 0; j < MATRIX_SIZE; j++) {
                mapClean[i, j].currentType = MapNode.RoomType.None;
            }
        }
        for (int i = 0; i < MATRIX_SIZE; i++)
        {
            for (int j = 0; j < MATRIX_SIZE; j++)
            {
                mapClean[i, j].UpdateConnections(mapClean);
            }
        }
        // Generate the layout
        List<MapNode> potentialCells = new List<MapNode>();
        List<MapNode> usedCells = new List<MapNode>();
        // Place the starting room at the center of the map
        MapNode initialCell = mapClean[MATRIX_SIZE / 2, MATRIX_SIZE / 2];
        initialCell.currentType = MapNode.RoomType.Potential;
        potentialCells.Add(initialCell);
        int roomsCreated = 0;
        while (roomsCreated < PLAYABLE_MAP_SIZE)
        {
            int indexSelected = Random.Range(0, potentialCells.Count);
            MapNode currentCell = potentialCells[indexSelected];
            potentialCells.RemoveAt(indexSelected);
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
                    if (mapClean[checkX, checkY].currentType != MapNode.RoomType.None) { continue; }

                    mapClean[checkX, checkY].currentType = MapNode.RoomType.Potential;
                    potentialCells.Add(mapClean[checkX, checkY]);
                }
            }
        }
        // Update connections
        for (int i = 0; i < usedCells.Count; i++)
        {
            usedCells[i].UpdateConnections(mapClean);
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
                            option1.Add(mapClean[singleConnectionCells[i].pos_x, singleConnectionCells[i].pos_y - 1]);
                            if (!mapClean[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y - 1].IsActive())
                            {
                                option1.Add(mapClean[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y - 1]);
                            }
                        }
                        if (singleConnectionCells[i].pos_y + 1 < MATRIX_SIZE)
                        {
                            option2.Add(mapClean[singleConnectionCells[i].pos_x, singleConnectionCells[i].pos_y + 1]);
                            if (!mapClean[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y + 1].IsActive())
                            {
                                option2.Add(mapClean[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y + 1]);
                            }
                        }
                        break;
                    }
                case 1: // UP
                    {
                        if (singleConnectionCells[i].pos_x - 1 >= 0)
                        {
                            option1.Add(mapClean[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y]);
                            if (!mapClean[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y - 1].IsActive())
                            {
                                option1.Add(mapClean[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y - 1]);
                            }
                        }
                        if (singleConnectionCells[i].pos_x + 1 < MATRIX_SIZE)
                        {
                            option2.Add(mapClean[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y]);
                            if (!mapClean[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y - 1].IsActive())
                            {
                                option2.Add(mapClean[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y - 1]);
                            }
                        }
                        break;
                    }
                case 2: // RIGHT
                    {
                        if (singleConnectionCells[i].pos_y - 1 >= 0)
                        {
                            option1.Add(mapClean[singleConnectionCells[i].pos_x, singleConnectionCells[i].pos_y - 1]);
                            if (!mapClean[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y - 1].IsActive())
                            {
                                option1.Add(mapClean[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y - 1]);
                            }
                        }
                        if (singleConnectionCells[i].pos_y + 1 < MATRIX_SIZE)
                        {
                            option2.Add(mapClean[singleConnectionCells[i].pos_x, singleConnectionCells[i].pos_y + 1]);
                            if (!mapClean[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y + 1].IsActive())
                            {
                                option2.Add(mapClean[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y + 1]);
                            }
                        }
                        break;
                    }
                case 3: // DOWN
                    {
                        if (singleConnectionCells[i].pos_x - 1 >= 0)
                        {
                            option1.Add(mapClean[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y]);
                            if (!mapClean[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y + 1].IsActive())
                            {
                                option1.Add(mapClean[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y + 1]);
                            }
                        }
                        if (singleConnectionCells[i].pos_x + 1 < MATRIX_SIZE)
                        {
                            option2.Add(mapClean[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y]);
                            if (!mapClean[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y + 1].IsActive())
                            {
                                option2.Add(mapClean[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y + 1]);
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
                if (mapClean[extraNeededCells[i].pos_x, extraNeededCells[i].pos_y].currentType == MapNode.RoomType.Room) { continue; } // Added by another iteration of this loop
                usedCells.Add(extraNeededCells[i]);
                extraNeededCells[i].currentType = MapNode.RoomType.Room;
                roomsCreated++;
            }
        }
        for (int i = 0; i < usedCells.Count; i++) { usedCells[i].UpdateConnections(mapClean); }
        
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

            for (int i = 0; i < usedCells.Count; i++) { usedCells[i].UpdateConnections(mapClean); }

            redundantCells = new List<MapNode>();
            for (int i = 0; i < usedCells.Count; i++)
            {
                if (usedCells[i].IsRedundant()) { redundantCells.Add(usedCells[i]); }
            }
        }

        for (int i = 0; i < usedCells.Count; i++)
        {
            usedCells[i].UpdateConnections(mapClean);
        }

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

        // Debug only
        for (int i = 0; i < MATRIX_SIZE; i++) 
        {
            for (int j = 0; j < MATRIX_SIZE; j++) 
            {
                mapClean[i, j].instance.Setup(mapClean[i, j]);
            }
        }
        print("Final count " + usedCells.Count);
    }

    IEnumerator TestCR() 
    {
        Random.InitState(12345);
        print(Random.Range(0, 100));
        print(Random.Range(0, 100));
        print(Random.Range(0, 100));
        print(Random.Range(0, 100));
        _stepinfo.text = "Press SPACE to go to the next step";
        while (!Input.GetKeyDown(KeyCode.Space)) { yield return null; }
        yield return null;

        map = new MapGenNodeData[MATRIX_SIZE, MATRIX_SIZE];
        for (int i = 0; i < MATRIX_SIZE; i++) 
        {
            for (int j = 0; j < MATRIX_SIZE; j++) 
            {
                GameObject go = Instantiate(_elemPrefab, _instParent) as GameObject;
                map[i, j] = new MapGenNodeData(i, j, go.GetComponent<MapGenTestElement>());
            }
        }
        _stepinfo.text = "Generating empty matrix, size " + MATRIX_SIZE + "," + MATRIX_SIZE;
        while (!Input.GetKeyDown(KeyCode.Space)) { yield return null; }
        yield return null;

        List<MapGenNodeData> possibleCells = new List<MapGenNodeData>();
        List<MapGenNodeData> usedCells = new List<MapGenNodeData>();

        while (!Input.GetKeyDown(KeyCode.Space)) { yield return null; }
        yield return null;
        _stepinfo.text = "Creating rooms ";

        possibleCells.Add(map[MATRIX_SIZE / 2, MATRIX_SIZE / 2]);
        int roomsCreated = 0;
        while (roomsCreated < PLAYABLE_MAP_SIZE) 
        {
            int indexSelected = Random.Range(0, possibleCells.Count);
            MapGenNodeData currentCell = possibleCells[indexSelected];
            possibleCells.RemoveAt(indexSelected);
            usedCells.Add(currentCell);
            currentCell.UpdateStatus(MapGenNodeData.RoomType.Room, false, false, false, false);
            roomsCreated++;


            if (currentCell.pos_x - 1 >= 0 && !map[currentCell.pos_x - 1, currentCell.pos_y].Active) { possibleCells.Add(map[currentCell.pos_x - 1, currentCell.pos_y]); }
            if (currentCell.pos_x + 1 < MATRIX_SIZE && !map[currentCell.pos_x + 1, currentCell.pos_y].Active) { possibleCells.Add(map[currentCell.pos_x + 1, currentCell.pos_y]); }
            if (currentCell.pos_y - 1 >= 0 && !map[currentCell.pos_x, currentCell.pos_y - 1].Active) { possibleCells.Add(map[currentCell.pos_x, currentCell.pos_y - 1]); }
            if (currentCell.pos_y + 1 < MATRIX_SIZE && !map[currentCell.pos_x, currentCell.pos_y + 1].Active) { possibleCells.Add(map[currentCell.pos_x, currentCell.pos_y + 1]); }


            _stepinfo.text = "Rooms placed: " + roomsCreated + "/" + PLAYABLE_MAP_SIZE;
            while (!Input.GetKeyDown(KeyCode.Space)) { yield return null; }
            yield return null;
        }

        _stepinfo.text = "Updating connection info for active cells";
        while (!Input.GetKeyDown(KeyCode.Space)) { yield return null; }
        yield return null;

        for (int i = 0; i < usedCells.Count; i++) 
        {
            bool u, l, r, d;

            l = usedCells[i].pos_x - 1 >= 0 && map[usedCells[i].pos_x - 1, usedCells[i].pos_y].Active;
            r = usedCells[i].pos_x + 1 < MATRIX_SIZE && map[usedCells[i].pos_x + 1, usedCells[i].pos_y].Active;
            d = usedCells[i].pos_y - 1 >= 0 && map[usedCells[i].pos_x, usedCells[i].pos_y - 1].Active;
            u = usedCells[i].pos_y + 1 < MATRIX_SIZE && map[usedCells[i].pos_x, usedCells[i].pos_y + 1].Active;
            usedCells[i].UpdateConnections(u, d, l, r);
        }

        _stepinfo.text = "Checking rooms with only one connection";
        while (!Input.GetKeyDown(KeyCode.Space)) { yield return null; }
        yield return null;

        List<MapGenNodeData> singleConnectionCells = new List<MapGenNodeData>();
        for (int i = 0; i < usedCells.Count; i++) 
        {
            if (usedCells[i].connections < 2 && usedCells[i].currentType == MapGenNodeData.RoomType.Room) { singleConnectionCells.Add(usedCells[i]); }
        }

        _stepinfo.text = singleConnectionCells.Count + " cells with only a single connection.";
        while (!Input.GetKeyDown(KeyCode.Space)) { yield return null; }
        yield return null;

        List<MapGenNodeData> extraNeededCells = new List<MapGenNodeData>();
        for (int i = 0; i < singleConnectionCells.Count; i++) 
        {
            int dir;
            if (singleConnectionCells[i]._r) { dir = 2; }
            else if (singleConnectionCells[i]._u) { dir = 3; }
            else if (singleConnectionCells[i]._l) { dir = 0; }
            else { dir = 1; }

            switch (dir) 
            {
                case 0: // RIGHT
                    {
                        List<MapGenNodeData> option1 = new List<MapGenNodeData>();
                        List<MapGenNodeData> option2 = new List<MapGenNodeData>();

                        if (singleConnectionCells[i].pos_y-1 >= 0) 
                        {
                            option1.Add(map[singleConnectionCells[i].pos_x, singleConnectionCells[i].pos_y - 1]);
                            if (!map[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y - 1].Active) 
                            {
                                option1.Add(map[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y - 1]);
                            } 
                        }
                        if (singleConnectionCells[i].pos_y + 1 < MATRIX_SIZE)
                        {
                            option2.Add(map[singleConnectionCells[i].pos_x, singleConnectionCells[i].pos_y + 1]);
                            if (!map[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y + 1].Active)
                            {
                                option2.Add(map[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y + 1]);
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
                        break;
                    }
                case 1: // UP
                    {
                        List<MapGenNodeData> option1 = new List<MapGenNodeData>();
                        List<MapGenNodeData> option2 = new List<MapGenNodeData>();

                        if (singleConnectionCells[i].pos_x - 1 >= 0)
                        {
                            option1.Add(map[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y]);
                            if (!map[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y - 1].Active)
                            {
                                option1.Add(map[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y - 1]);
                            }
                        }
                        if (singleConnectionCells[i].pos_x + 1 < MATRIX_SIZE)
                        {
                            option2.Add(map[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y]);
                            if (!map[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y - 1].Active)
                            {
                                option2.Add(map[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y - 1]);
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
                        break;
                    }
                case 2: // LEFT
                    {
                        List<MapGenNodeData> option1 = new List<MapGenNodeData>();
                        List<MapGenNodeData> option2 = new List<MapGenNodeData>();

                        if (singleConnectionCells[i].pos_y - 1 >= 0)
                        {
                            option1.Add(map[singleConnectionCells[i].pos_x, singleConnectionCells[i].pos_y - 1]);
                            if (!map[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y - 1].Active)
                            {
                                option1.Add(map[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y - 1]);
                            }
                        }
                        if (singleConnectionCells[i].pos_y + 1 < MATRIX_SIZE)
                        {
                            option2.Add(map[singleConnectionCells[i].pos_x, singleConnectionCells[i].pos_y + 1]);
                            if (!map[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y + 1].Active)
                            {
                                option2.Add(map[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y + 1]);
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
                        break;
                    }
                case 3: // DOWN
                    {
                        List<MapGenNodeData> option1 = new List<MapGenNodeData>();
                        List<MapGenNodeData> option2 = new List<MapGenNodeData>();

                        if (singleConnectionCells[i].pos_x - 1 >= 0)
                        {
                            option1.Add(map[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y]);
                            if (!map[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y + 1].Active)
                            {
                                option1.Add(map[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y + 1]);
                            }
                        }
                        if (singleConnectionCells[i].pos_x + 1 < MATRIX_SIZE)
                        {
                            option2.Add(map[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y]);
                            if (!map[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y + 1].Active)
                            {
                                option2.Add(map[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y + 1]);
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
                        break;
                    }
            }

        }
        if (singleConnectionCells.Count > 0)
        {
            _stepinfo.text = "Adding additional needed cells (" + extraNeededCells.Count + ")";
            while (!Input.GetKeyDown(KeyCode.Space)) { yield return null; }
            yield return null;

            for (int i = 0; i < extraNeededCells.Count; i++) 
            {
                if (map[extraNeededCells[i].pos_x, extraNeededCells[i].pos_y].Active) { continue; } // Added by another iteration of this loop
                usedCells.Add(extraNeededCells[i]);
                extraNeededCells[i].UpdateStatus(MapGenNodeData.RoomType.Room, false, false, false, false);
                roomsCreated++;
            }

            _stepinfo.text = "Updating connections again.";
            while (!Input.GetKeyDown(KeyCode.Space)) { yield return null; }
            yield return null;

            for (int i = 0; i < usedCells.Count; i++)
            {
                bool u, l, r, d;

                l = usedCells[i].pos_x - 1 >= 0 && map[usedCells[i].pos_x - 1, usedCells[i].pos_y].Active;
                r = usedCells[i].pos_x + 1 < MATRIX_SIZE && map[usedCells[i].pos_x + 1, usedCells[i].pos_y].Active;
                d = usedCells[i].pos_y - 1 >= 0 && map[usedCells[i].pos_x, usedCells[i].pos_y - 1].Active;
                u = usedCells[i].pos_y + 1 < MATRIX_SIZE && map[usedCells[i].pos_x, usedCells[i].pos_y + 1].Active;
                usedCells[i].UpdateConnections(u, d, l, r);
            }
        }
        else 
        {
            _stepinfo.text = "No more cells need to be added.";
            while (!Input.GetKeyDown(KeyCode.Space)) { yield return null; }
            yield return null;
        }
        _stepinfo.text = "Checking clustered rooms.";
        while (!Input.GetKeyDown(KeyCode.Space)) { yield return null; }
        yield return null;

        List<MapGenNodeData> redundantCells = new List<MapGenNodeData>();

        for (int i = 0; i < usedCells.Count; i++)
        {
            if (usedCells[i]._l && map[usedCells[i].pos_x - 1, usedCells[i].pos_y].connections <= 2) { continue; }
            if (usedCells[i]._r && map[usedCells[i].pos_x + 1, usedCells[i].pos_y].connections <= 2) { continue; }
            if (usedCells[i]._u && map[usedCells[i].pos_x, usedCells[i].pos_y - 1].connections <= 2) { continue; }
            if (usedCells[i]._d && map[usedCells[i].pos_x, usedCells[i].pos_y + 1].connections <= 2) { continue; }
            if (usedCells[i].connections <= 2) { continue; }

            redundantCells.Add(usedCells[i]);
        }

        _stepinfo.text = "Removing the " + redundantCells.Count + " redundant cells";
        while (!Input.GetKeyDown(KeyCode.Space)) { yield return null; }
        yield return null;

        for (int i = 0; i < redundantCells.Count; i++) 
        {
            redundantCells[i].UpdateStatus(MapGenNodeData.RoomType.None, false, false, false, false);
            usedCells.Remove(redundantCells[i]);
        }

        _stepinfo.text = "Updating connections again.";
        while (!Input.GetKeyDown(KeyCode.Space)) { yield return null; }
        yield return null;

        for (int i = 0; i < usedCells.Count; i++)
        {
            bool u, l, r, d;

            l = usedCells[i].pos_x - 1 >= 0 && map[usedCells[i].pos_x - 1, usedCells[i].pos_y].Active;
            r = usedCells[i].pos_x + 1 < MATRIX_SIZE && map[usedCells[i].pos_x + 1, usedCells[i].pos_y].Active;
            d = usedCells[i].pos_y - 1 >= 0 && map[usedCells[i].pos_x, usedCells[i].pos_y - 1].Active;
            u = usedCells[i].pos_y + 1 < MATRIX_SIZE && map[usedCells[i].pos_x, usedCells[i].pos_y + 1].Active;
            usedCells[i].UpdateConnections(u, d, l, r);
        }

        int maxRooms = (int)(usedCells.Count * ROOM_PERCENT);
        _stepinfo.text = "The map contains " + usedCells.Count + " cells, a maximum of " + maxRooms + " is allowed. The rest must be corridors, converting...";
        while (!Input.GetKeyDown(KeyCode.Space)) { yield return null; }
        yield return null;
        List<MapGenNodeData> convertibleCells = new List<MapGenNodeData>();
        for (int i = 0; i < usedCells.Count; i++) { convertibleCells.Add(usedCells[i]); }

        for (int i = 0; i < usedCells.Count - maxRooms; i++)
        {
            if (convertibleCells.Count > 0)
            {
                int index = Random.Range(0, convertibleCells.Count);
                convertibleCells[index].UpdateStatus(MapGenNodeData.RoomType.Corridor, convertibleCells[index]._u, convertibleCells[index]._d, convertibleCells[index]._l, convertibleCells[index]._r);
                convertibleCells.RemoveAt(index);
            }
        }

        _stepinfo.text = "Map finished";
        while (!Input.GetKeyDown(KeyCode.Space)) { yield return null; }
        yield return null;
    }
    public class MapNode
    {
        public enum RoomType { None, Potential, Room, Corridor }

        public RoomType currentType;
        public MapGenTestElement instance;
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
    public class MapGenNodeData 
    {
        public enum RoomType { None, Room, Corridor }

        public bool Active { get { return currentType != RoomType.None; } }
        public RoomType currentType;
        MapGenTestElement instance;
        public bool potential;
        public int pos_x;
        public int pos_y;
        public int connections;
        public bool _r;
        public bool _l;
        public bool _u;
        public bool _d;


        public MapGenNodeData(int x, int y, MapGenTestElement inst) 
        {
            pos_x = x;
            pos_y = y;
            instance = inst;
            potential = false;
            instance.Setup(pos_x, pos_y, currentType, false, false, false, false, 0);
        }
        public void UpdateConnections(bool u, bool d, bool l, bool r) 
        {
            UpdateStatus(currentType, u, d, l, r);
        }
        public void UpdateStatus(RoomType rt, bool u, bool d, bool l, bool r) 
        {
            currentType = rt;
            int connectionCount = 0;
            if (l) { connectionCount++; }
            if (r) { connectionCount++; }
            if (u) { connectionCount++; }
            if (d) { connectionCount++; }

            _r = r;
            _u = u;
            _d = d;
            _l = l;

            connections = connectionCount;

            instance.Setup(pos_x, pos_y, currentType, u, d, l, r, connections);
        }
    }
}
