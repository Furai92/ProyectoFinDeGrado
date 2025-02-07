using System.Collections.Generic;
using UnityEngine;

public class ProceduralStageData : IMapData
{
    private MapNode[,] stageMatrix;
    private MapNode firstRoom;
    private List<MapNode> stageList;
    private List<Vector3> enemySpawnPositions;
    private List<StagePiece> stagePieces;

    private const float ROOM_PERCENT = 0.4f;
    private const float CHANCE_TO_2 = 0.2f;
    private const float CHANCE_TO_3 = 0.1f;
    private const float CHANCE_TO_4 = 0.5f;

    public ProceduralStageData(int seed, int size, List<GameObject> roomPrefabs, List<GameObject> corridorPrefabs, List<GameObject> decoPrefabs, Transform instParent) 
    {
        // Initialize the map matrix
        stageMatrix = new MapNode[StageManagerBase.STAGE_SIZE, StageManagerBase.STAGE_SIZE];
        for (int i = 0; i < StageManagerBase.STAGE_SIZE; i++)
        {
            for (int j = 0; j < StageManagerBase.STAGE_SIZE; j++)
            {
                stageMatrix[i, j] = new MapNode(i, j);
                stageMatrix[i, j].currentType = MapNode.RoomType.None;
                stageMatrix[i, j].variation = -1;
            }
        }
        for (int i = 0; i < StageManagerBase.STAGE_SIZE; i++)
        {
            for (int j = 0; j < StageManagerBase.STAGE_SIZE; j++)
            {
                stageMatrix[i, j].UpdateConnections(stageMatrix);
            }
        }
        // Generate the layout
        List<MapNode> potentialCells = new List<MapNode>();
        stageList = new List<MapNode>();
        // Place the starting room at the center of the map
        MapNode initialCell = stageMatrix[StageManagerBase.STAGE_SIZE / 2, StageManagerBase.STAGE_SIZE / 2];
        initialCell.currentType = MapNode.RoomType.Potential;
        potentialCells.Add(initialCell);
        int roomsCreated = 0;
        while (roomsCreated < size)
        {
            List<MapNode> randomlySelectedCells = new List<MapNode>();
            for (int i = 0; i < potentialCells.Count; i++)
            {
                int nearbyUsedCells = potentialCells[i].GetNearbyUsedRooms(stageMatrix);
                float chanceToAdd;
                switch (nearbyUsedCells)
                {
                    case 2: { chanceToAdd = CHANCE_TO_2; break; }
                    case 3: { chanceToAdd = CHANCE_TO_3; break; }
                    case 4: { chanceToAdd = CHANCE_TO_4; break; }
                    default: { chanceToAdd = 100; break; } // Cells with a single room connecting will have a 100% chance to be used.
                }
                if (potentialCells.Count == 1 || Random.Range(0, 100) < chanceToAdd)
                {
                    randomlySelectedCells.Add(potentialCells[i]);
                }
            }

            int indexSelected = Random.Range(0, randomlySelectedCells.Count);
            MapNode currentCell = randomlySelectedCells[indexSelected];
            potentialCells.Remove(currentCell);
            if (stageList.Count == 0) { firstRoom = currentCell; }
            stageList.Add(currentCell);
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
                    if (checkX >= StageManagerBase.STAGE_SIZE) { continue; }
                    if (checkY >= StageManagerBase.STAGE_SIZE) { continue; }
                    if (stageMatrix[checkX, checkY].currentType != MapNode.RoomType.None) { continue; }


                    potentialCells.Add(stageMatrix[checkX, checkY]);
                    stageMatrix[checkX, checkY].currentType = MapNode.RoomType.Potential;

                }
            }
        }
        // Update connections
        for (int i = 0; i < stageList.Count; i++)
        {
            stageList[i].UpdateConnections(stageMatrix);
        }
        // Find single connection cells
        List<MapNode> singleConnectionCells = new List<MapNode>();
        for (int i = 0; i < stageList.Count; i++)
        {
            if (stageList[i].connectionCount == 1) { singleConnectionCells.Add(stageList[i]); }
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
                            option1.Add(stageMatrix[singleConnectionCells[i].pos_x, singleConnectionCells[i].pos_y - 1]);
                            if (!stageMatrix[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y - 1].IsActive())
                            {
                                option1.Add(stageMatrix[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y - 1]);
                            }
                        }
                        if (singleConnectionCells[i].pos_y + 1 < StageManagerBase.STAGE_SIZE)
                        {
                            option2.Add(stageMatrix[singleConnectionCells[i].pos_x, singleConnectionCells[i].pos_y + 1]);
                            if (!stageMatrix[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y + 1].IsActive())
                            {
                                option2.Add(stageMatrix[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y + 1]);
                            }
                        }
                        break;
                    }
                case 1: // UP
                    {
                        if (singleConnectionCells[i].pos_x - 1 >= 0)
                        {
                            option1.Add(stageMatrix[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y]);
                            if (!stageMatrix[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y - 1].IsActive())
                            {
                                option1.Add(stageMatrix[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y - 1]);
                            }
                        }
                        if (singleConnectionCells[i].pos_x + 1 < StageManagerBase.STAGE_SIZE)
                        {
                            option2.Add(stageMatrix[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y]);
                            if (!stageMatrix[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y - 1].IsActive())
                            {
                                option2.Add(stageMatrix[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y - 1]);
                            }
                        }
                        break;
                    }
                case 2: // RIGHT
                    {
                        if (singleConnectionCells[i].pos_y - 1 >= 0)
                        {
                            option1.Add(stageMatrix[singleConnectionCells[i].pos_x, singleConnectionCells[i].pos_y - 1]);
                            if (!stageMatrix[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y - 1].IsActive())
                            {
                                option1.Add(stageMatrix[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y - 1]);
                            }
                        }
                        if (singleConnectionCells[i].pos_y + 1 < StageManagerBase.STAGE_SIZE)
                        {
                            option2.Add(stageMatrix[singleConnectionCells[i].pos_x, singleConnectionCells[i].pos_y + 1]);
                            if (!stageMatrix[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y + 1].IsActive())
                            {
                                option2.Add(stageMatrix[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y + 1]);
                            }
                        }
                        break;
                    }
                case 3: // DOWN
                    {
                        if (singleConnectionCells[i].pos_x - 1 >= 0)
                        {
                            option1.Add(stageMatrix[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y]);
                            if (!stageMatrix[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y + 1].IsActive())
                            {
                                option1.Add(stageMatrix[singleConnectionCells[i].pos_x - 1, singleConnectionCells[i].pos_y + 1]);
                            }
                        }
                        if (singleConnectionCells[i].pos_x + 1 < StageManagerBase.STAGE_SIZE)
                        {
                            option2.Add(stageMatrix[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y]);
                            if (!stageMatrix[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y + 1].IsActive())
                            {
                                option2.Add(stageMatrix[singleConnectionCells[i].pos_x + 1, singleConnectionCells[i].pos_y + 1]);
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
                if (stageMatrix[extraNeededCells[i].pos_x, extraNeededCells[i].pos_y].currentType == MapNode.RoomType.Room) { continue; } // Added by another iteration of this loop
                stageList.Add(extraNeededCells[i]);
                extraNeededCells[i].currentType = MapNode.RoomType.Room;
                roomsCreated++;
            }
        }
        for (int i = 0; i < stageList.Count; i++) { stageList[i].UpdateConnections(stageMatrix); }

        // Checking for redundant cells
        List<MapNode> redundantCells = new List<MapNode>();
        for (int i = 0; i < stageList.Count; i++)
        {
            if (stageList[i].IsRedundant()) { redundantCells.Add(stageList[i]); }
        }
        Debug.Log("Redundant " + redundantCells.Count);
        while (redundantCells.Count > 0)
        {
            redundantCells[0].currentType = MapNode.RoomType.None;
            stageList.Remove(redundantCells[0]);
            redundantCells.RemoveAt(0);

            for (int i = 0; i < stageList.Count; i++) { stageList[i].UpdateConnections(stageMatrix); }

            redundantCells = new List<MapNode>();
            for (int i = 0; i < stageList.Count; i++)
            {
                if (stageList[i].IsRedundant()) { redundantCells.Add(stageList[i]); }
            }
        }
        // Update connections after deleting
        for (int i = 0; i < stageList.Count; i++)
        {
            stageList[i].UpdateConnections(stageMatrix);
        }
        // Balance the room to corridor ratio
        int maxRooms = (int)(stageList.Count * ROOM_PERCENT);
        List<MapNode> convertibleCells = new List<MapNode>();
        for (int i = 0; i < stageList.Count; i++) { convertibleCells.Add(stageList[i]); }

        for (int i = 0; i < stageList.Count - maxRooms; i++)
        {
            if (convertibleCells.Count > 0)
            {
                int index = Random.Range(0, convertibleCells.Count);
                convertibleCells[index].currentType = MapNode.RoomType.Corridor;
                convertibleCells.RemoveAt(index);
            }
        }
        // Set the variation to each room
        for (int i = 0; i < stageList.Count; i++)
        {
            if (stageList[i].variation >= 0) { continue; }

            int variationID = Random.Range(0, 2);
            List<MapNode> variationGroup = new List<MapNode>();
            variationGroup.Add(stageList[i]);
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

        // Instantiate prefabs
        stagePieces = new List<StagePiece>();
        for (int i = 0; i < stageList.Count; i++)
        {
            switch (stageList[i].currentType)
            {
                case MapNode.RoomType.Room:
                    {
                        int index = Mathf.Min(roomPrefabs.Count - 1, stageList[i].variation);
                        GameObject rp = GameObject.Instantiate(roomPrefabs[index], instParent) as GameObject;
                        rp.GetComponent<StagePiece>().SetUp(stageList[i]);
                        stagePieces.Add(rp.GetComponent<StagePiece>());
                        stageList[i].piece = rp.GetComponent<StagePiece>();
                        break;
                    }
                case MapNode.RoomType.Corridor:
                    {
                        int index = Mathf.Min(corridorPrefabs.Count - 1, stageList[i].variation);
                        GameObject rp = GameObject.Instantiate(corridorPrefabs[index], instParent) as GameObject;
                        rp.GetComponent<StagePiece>().SetUp(stageList[i]);
                        stagePieces.Add(rp.GetComponent<StagePiece>());
                        stageList[i].piece = rp.GetComponent<StagePiece>();
                        break;
                    }
                case MapNode.RoomType.Deco:
                    {
                        GameObject rp = GameObject.Instantiate(decoPrefabs[Random.Range(0, decoPrefabs.Count)], instParent) as GameObject;
                        rp.GetComponent<StagePiece>().SetUp(stageList[i]);
                        stagePieces.Add(rp.GetComponent<StagePiece>());
                        stageList[i].piece = rp.GetComponent<StagePiece>();
                        break;
                    }
            }
        }
    }

    public List<Vector3> GetEnemySpawnPositions()
    {
        return enemySpawnPositions;
    }

    public Vector3 GetPlayerSpawnPosition()
    {
        return firstRoom.piece.transform.position;
    }

    public MapNode[,] GetLayoutMatrix()
    {
        return stageMatrix;
    }

    public List<MapNode> GetLayoutList()
    {
        return stageList;
    }

}
