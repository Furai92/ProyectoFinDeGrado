using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class MapGenTest : MonoBehaviour
{
    [SerializeField] private GameObject _elemPrefab;
    [SerializeField] private TextMeshProUGUI _stepinfo;
    [SerializeField] private Transform _instParent;
    private MapGenNodeData[,] map;

    private const int MATRIX_SIZE = 50;
    private const int PLAYABLE_MAP_SIZE = 8;
    private const float ROOM_PERCENT = 0.4f;


    private void OnEnable()
    {
        StartCoroutine(TestCR());
    }

    IEnumerator TestCR() 
    {
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
    }

    public class MapGenNodeData 
    {
        public enum RoomType { None, Room, Corridor }

        public bool Active { get { return currentType != RoomType.None; } }
        public RoomType currentType;
        MapGenTestElement instance;
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
