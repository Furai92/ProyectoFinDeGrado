using System.Collections.Generic;
using UnityEngine;

public class ProceduralStageData : IMapData
{
    private MapNode[,] stageMatrix;
    private MapNode firstRoom;
    private List<MapNode> stageList;
    private List<Vector3> enemySpawnPositions;
    private List<Vector3> chestSpawnPositions;
    private List<StagePiece> stagePieces;
    private List<MapNode> stageDecos;

    private const float ROOM_PERCENT = 0.6f;
    private const float CHANCE_TO_2 = 20f;
    private const float CHANCE_TO_3 = 10f;
    private const float CHANCE_TO_4 = 5f;
    private const float CHANCE_TO_ROOM_PER_CONNECTION = 20f;
    private const float DECO_CHANCE_BASE = 10f;
    private const float DECO_CHANCE_PER_NEARBY_NODE = 10f;

    public ProceduralStageData(int seed, int size, ProceduralStagePropertiesSO stageProperties, Transform instParent) 
    {
        Random.InitState(seed);
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
        while (singleConnectionCells.Count > 0) 
        {
            // Find out what is needed to remove the dead ends
            for (int i = 0; i < singleConnectionCells.Count; i++)
            {
                List<MapNode> antiDeadEndStrip = GetAntiDeadEndStrip(singleConnectionCells[i]);
                while (antiDeadEndStrip.Count > 0) 
                {
                    // Check if this position is already used, if it is, stop adding cells, dead end is already removed
                    if (stageMatrix[antiDeadEndStrip[0].pos_x, antiDeadEndStrip[0].pos_y].IsActive()) 
                    {
                        antiDeadEndStrip.Clear(); 
                    } 
                    else 
                    {
                        antiDeadEndStrip[0].currentType = MapNode.RoomType.Room;
                        stageList.Add(antiDeadEndStrip[0]);
                        roomsCreated++;
                    }
                    // If after adding a cell, there's more than one nearby room, stop adding cells, dead end is already removed
                    if (antiDeadEndStrip[0].GetNearbyUsedRooms(stageMatrix) > 1) 
                    {
                        antiDeadEndStrip.Clear(); 
                    }
                    else 
                    {
                        antiDeadEndStrip.RemoveAt(0); // Remove from the pending list
                    }
                }
            }
            // Update the connections after removing dead ends
            for (int i = 0; i < stageList.Count; i++) { stageList[i].UpdateConnections(stageMatrix); }
            // Check for more single connection cells
            singleConnectionCells = new List<MapNode>();
            for (int i = 0; i < stageList.Count; i++)
            {
                if (stageList[i].connectionCount == 1) { singleConnectionCells.Add(stageList[i]); }
            }
        }
        
        // Set as rooms the required amount of rooms
        int maxRooms = (int)(stageList.Count * ROOM_PERCENT);
        // Get a copied list of all the active rooms
        List<MapNode> convertibleCells = new List<MapNode>();
        for (int i = 0; i < stageList.Count; i++)
        {
            convertibleCells.Add(stageList[i]);
            stageList[i].currentType = MapNode.RoomType.Corridor; // Initially, all corridors
        }

        for (int i = convertibleCells.Count-1; i >= 0; i--)
        {
            if (convertibleCells.Count < roomsCreated - maxRooms) { break; } // Skip if enough rooms have been created
            float chanceToRoom = convertibleCells[i].connectionCount * CHANCE_TO_ROOM_PER_CONNECTION; // Less than 100% chance on 4 connections (maximum)
            if (Random.Range(0, 100) < chanceToRoom) 
            {
                convertibleCells[i].currentType = MapNode.RoomType.Room;
                convertibleCells.RemoveAt(i);
            }
        }
        while (convertibleCells.Count > roomsCreated - maxRooms && convertibleCells.Count > 0) 
        {
            int randomindex = Random.Range(0, convertibleCells.Count);
            convertibleCells[randomindex].currentType = MapNode.RoomType.Room;
            convertibleCells.RemoveAt(randomindex);
        }

        // Place deco elements outside of the playable area, the chance of a deco being placed is based on how many playable nodes exist around a specific point in the grid
        // Go trough all the nodes placed in the stage, and node the highest/lowest X/Y to determine a bounding box
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = -1;
        int maxY = -1;
        for (int i = 0; i < stageList.Count; i++) 
        {
            if (stageList[i].pos_x < minX) { minX = stageList[i].pos_x; }
            if (stageList[i].pos_y < minY) { minY = stageList[i].pos_y; }
            if (stageList[i].pos_x > maxX) { maxX = stageList[i].pos_x; }
            if (stageList[i].pos_y > maxY) { maxY = stageList[i].pos_y; }
        }
        // If possible, increase min/max one cell
        if (minX > 0) { minX--; }
        if (minY > 0)  { minY--; }
        if (maxX < StageManagerBase.STAGE_SIZE - 1) { maxX++; }
        if (maxY < StageManagerBase.STAGE_SIZE - 1) { maxY++; }

        stageDecos = new List<MapNode>();
        for (int i = minX; i <= maxX; i++) 
        {
            for (int j = minY; j <= maxY; j++)
            {
                if (stageMatrix[i, j].currentType != MapNode.RoomType.None && stageMatrix[i, j].currentType != MapNode.RoomType.Potential) { continue; } // If it's not empty, check next
                float chance = DECO_CHANCE_BASE;
                if (i - 1 > 0 && (stageMatrix[i - 1, j].currentType == MapNode.RoomType.Corridor || stageMatrix[i - 1, j].currentType == MapNode.RoomType.Room)) { chance += DECO_CHANCE_PER_NEARBY_NODE; }
                if (j - 1 > 0 && (stageMatrix[i, j - 1].currentType == MapNode.RoomType.Corridor || stageMatrix[i, j - 1].currentType == MapNode.RoomType.Room)) { chance += DECO_CHANCE_PER_NEARBY_NODE; }
                if (i + 1 < StageManagerBase.STAGE_SIZE && (stageMatrix[i + 1, j].currentType == MapNode.RoomType.Corridor || stageMatrix[i + 1, j].currentType == MapNode.RoomType.Room)) { chance += DECO_CHANCE_PER_NEARBY_NODE; }
                if (j + 1 < StageManagerBase.STAGE_SIZE && (stageMatrix[i, j + 1].currentType == MapNode.RoomType.Corridor || stageMatrix[i, j + 1].currentType == MapNode.RoomType.Room)) { chance += DECO_CHANCE_PER_NEARBY_NODE; }

                if (Random.Range(0, 101) < chance) { stageMatrix[i, j].currentType = MapNode.RoomType.Deco; stageDecos.Add(stageMatrix[i, j]); } 
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
        // Instantiate playable areas
        stagePieces = new List<StagePiece>();
        for (int i = 0; i < stageList.Count; i++)
        {
            switch (stageList[i].currentType)
            {
                case MapNode.RoomType.Room:
                    {
                        int index = Mathf.Min(stageProperties.RoomPrefabs.Count - 1, stageList[i].variation);
                        GameObject rp = GameObject.Instantiate(stageProperties.RoomPrefabs[index], instParent) as GameObject;
                        rp.GetComponent<StagePiece>().SetUp(stageList[i]);
                        stagePieces.Add(rp.GetComponent<StagePiece>());
                        stageList[i].piece = rp.GetComponent<StagePiece>();
                        rp.name = string.Format("stage_piece_{0}_{1}", stageList[i].pos_x, stageList[i].pos_y);
                        break;
                    }
                case MapNode.RoomType.Corridor:
                    {
                        int index = Mathf.Min(stageProperties.CorridorPrefabs.Count - 1, stageList[i].variation);
                        GameObject rp = GameObject.Instantiate(stageProperties.CorridorPrefabs[index], instParent) as GameObject;
                        rp.GetComponent<StagePiece>().SetUp(stageList[i]);
                        stagePieces.Add(rp.GetComponent<StagePiece>());
                        stageList[i].piece = rp.GetComponent<StagePiece>();
                        rp.name = string.Format("stage_piece_{0}_{1}", stageList[i].pos_x, stageList[i].pos_y);
                        break;
                    }
            }
        }
        // Instantiate deco pieces
        for (int i = 0; i < stageDecos.Count; i++)
        {
            GameObject rp = GameObject.Instantiate(stageProperties.DecoPrefabs[Random.Range(0, stageProperties.DecoPrefabs.Count)], instParent) as GameObject;
            rp.GetComponent<StageDeco>().SetUp(stageDecos[i]);
            rp.name = string.Format("deco_{0}_{1}", stageDecos[i].pos_x, stageDecos[i].pos_y);
        }
        // Setup enemy spawn positions
        // TO DO: Improve logic, for now just use room center
        enemySpawnPositions = new List<Vector3>();
        for (int i = 0; i < stagePieces.Count; i++) 
        {
            enemySpawnPositions.Add(stagePieces[i].transform.position);
        }
        // Setup chest spawn positions
        chestSpawnPositions = new List<Vector3>();
        for (int i = 0; i < stagePieces.Count; i++) 
        {
            for (int j = 0; j < stagePieces[i].ChestSpawnPoints.Count; j++) 
            {
                chestSpawnPositions.Add(stagePieces[i].ChestSpawnPoints[j].position);
            }
        }
        Debug.Log("Map finished with a size of " + stageList.Count);
    }
    private List<MapNode> GetAntiDeadEndStrip(MapNode deadEndNode) 
    {
        // What's the only connection that this cell has?
        int dir;
        if (deadEndNode.con_right != null) { dir = 2; }
        else if (deadEndNode.con_up != null) { dir = 3; }
        else if (deadEndNode.con_left != null) { dir = 0; }
        else { dir = 1; }

        List<MapNode> option1 = new List<MapNode>(); // Strip candidate 1
        List<MapNode> option2 = new List<MapNode>(); // Strip candidate 2

        switch (dir)
        {
            case 0: // LEFT
                {
                    if (deadEndNode.pos_y - 2 >= 0)
                    {
                        option1.Add(stageMatrix[deadEndNode.pos_x, deadEndNode.pos_y - 1]);
                        option1.Add(stageMatrix[deadEndNode.pos_x, deadEndNode.pos_y - 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x - 1, deadEndNode.pos_y - 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x - 2, deadEndNode.pos_y - 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x - 2, deadEndNode.pos_y - 1]);
                        option1.Add(stageMatrix[deadEndNode.pos_x - 2, deadEndNode.pos_y]);

                    }
                    if (deadEndNode.pos_y + 2 < StageManagerBase.STAGE_SIZE)
                    {
                        option1.Add(stageMatrix[deadEndNode.pos_x, deadEndNode.pos_y + 1]);
                        option1.Add(stageMatrix[deadEndNode.pos_x, deadEndNode.pos_y + 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x - 1, deadEndNode.pos_y + 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x - 2, deadEndNode.pos_y + 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x - 2, deadEndNode.pos_y + 1]);
                        option1.Add(stageMatrix[deadEndNode.pos_x - 2, deadEndNode.pos_y]);
                    }
                    break;
                }
            case 1: // UP
                {
                    if (deadEndNode.pos_x + 2 < StageManagerBase.STAGE_SIZE)
                    {
                        option1.Add(stageMatrix[deadEndNode.pos_x + 1, deadEndNode.pos_y]);
                        option1.Add(stageMatrix[deadEndNode.pos_x + 2, deadEndNode.pos_y]);
                        option1.Add(stageMatrix[deadEndNode.pos_x + 2, deadEndNode.pos_y - 1]);
                        option1.Add(stageMatrix[deadEndNode.pos_x + 2, deadEndNode.pos_y - 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x + 1, deadEndNode.pos_y - 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x, deadEndNode.pos_y - 2]);

                    }
                    if (deadEndNode.pos_x - 2 >= 0)
                    {
                        option1.Add(stageMatrix[deadEndNode.pos_x - 1, deadEndNode.pos_y]);
                        option1.Add(stageMatrix[deadEndNode.pos_x - 2, deadEndNode.pos_y]);
                        option1.Add(stageMatrix[deadEndNode.pos_x - 2, deadEndNode.pos_y - 1]);
                        option1.Add(stageMatrix[deadEndNode.pos_x - 2, deadEndNode.pos_y - 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x - 1, deadEndNode.pos_y - 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x, deadEndNode.pos_y - 2]);
                    }
                    break;
                }
            case 2: // RIGHT
                {
                    if (deadEndNode.pos_y - 2 >= 0)
                    {
                        option1.Add(stageMatrix[deadEndNode.pos_x, deadEndNode.pos_y - 1]);
                        option1.Add(stageMatrix[deadEndNode.pos_x, deadEndNode.pos_y - 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x + 1, deadEndNode.pos_y - 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x + 2, deadEndNode.pos_y - 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x + 2, deadEndNode.pos_y - 1]);
                        option1.Add(stageMatrix[deadEndNode.pos_x + 2, deadEndNode.pos_y]);

                    }
                    if (deadEndNode.pos_y + 2 < StageManagerBase.STAGE_SIZE)
                    {
                        option1.Add(stageMatrix[deadEndNode.pos_x, deadEndNode.pos_y + 1]);
                        option1.Add(stageMatrix[deadEndNode.pos_x, deadEndNode.pos_y + 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x + 1, deadEndNode.pos_y + 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x + 2, deadEndNode.pos_y + 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x + 2, deadEndNode.pos_y + 1]);
                        option1.Add(stageMatrix[deadEndNode.pos_x + 2, deadEndNode.pos_y]);
                    }
                    break;
                }
            case 3: // DOWN
                {
                    if (deadEndNode.pos_x + 2 < StageManagerBase.STAGE_SIZE)
                    {
                        option1.Add(stageMatrix[deadEndNode.pos_x + 1, deadEndNode.pos_y]);
                        option1.Add(stageMatrix[deadEndNode.pos_x + 2, deadEndNode.pos_y]);
                        option1.Add(stageMatrix[deadEndNode.pos_x + 2, deadEndNode.pos_y + 1]);
                        option1.Add(stageMatrix[deadEndNode.pos_x + 2, deadEndNode.pos_y + 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x + 1, deadEndNode.pos_y + 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x, deadEndNode.pos_y + 2]);

                    }
                    if (deadEndNode.pos_x - 2 >= 0)
                    {
                        option1.Add(stageMatrix[deadEndNode.pos_x - 1, deadEndNode.pos_y]);
                        option1.Add(stageMatrix[deadEndNode.pos_x - 2, deadEndNode.pos_y]);
                        option1.Add(stageMatrix[deadEndNode.pos_x - 2, deadEndNode.pos_y + 1]);
                        option1.Add(stageMatrix[deadEndNode.pos_x - 2, deadEndNode.pos_y + 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x - 1, deadEndNode.pos_y + 2]);
                        option1.Add(stageMatrix[deadEndNode.pos_x, deadEndNode.pos_y + 2]);
                    }
                    break;
                }
        }
        // Check how many nodes are actually required to undo the dead end from each of the two options
        List<MapNode> option1Optimized = new List<MapNode>();
        while (option1.Count > 0) 
        {
            if (!option1[0].IsActive()) 
            {
                option1Optimized.Add(option1[0]);
                option1.RemoveAt(0); 
            } 
            else 
            {
                option1.Clear();  // If the next step is already active, clear the list, the strip is finished
            }
        }
        List<MapNode> option2Optimized = new List<MapNode>();
        while (option2.Count > 0)
        {
            if (!option2[0].IsActive()) 
            {
                option2Optimized.Add(option2[0]);
                option2.RemoveAt(0);
            }
            else
            {
                option2.Clear(); // If the next step is already active, clear the list, the strip is finished
            } 
        }
        // Select the option that requires the least amount of nodes, if any of the possible options was out of bounds, discard it and select the other one
        List<MapNode> selectedOption;
        if (option1Optimized.Count == 0) { selectedOption = option2Optimized; } // 1 was out of bounds
        else if (option2Optimized.Count == 0) { selectedOption = option1Optimized; } // 2 was out of bounds
        else 
        {
            // None is out of bounds, select the shortest
            if (option1Optimized.Count == option2Optimized.Count)
            {
                // If the size is the same, select at random.
                selectedOption = Random.Range(0, 2) == 0 ? option1Optimized : option2Optimized;
            }
            else
            {
                // Select the shortest
                selectedOption = option1Optimized.Count < option2Optimized.Count ? option1Optimized : option2Optimized;
            }
        }
        return selectedOption;
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

    public List<StagePiece> GetStagePieces()
    {
        return stagePieces;
    }

    public List<Vector3> GetChestSpawnPositions() 
    {
        return chestSpawnPositions;
    }
}
