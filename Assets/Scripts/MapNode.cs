public class MapNode
{
    public enum RoomType { None, Potential, Room, Corridor, Deco }

    public RoomType currentType;
    public int variation;
    public int pos_x;
    public int pos_y;
    public int connectionCount;
    public MapNode con_up;
    public MapNode con_down;
    public MapNode con_left;
    public MapNode con_right;
    public StagePiece piece;
    public bool IsActive() { return currentType != RoomType.None && currentType != RoomType.Potential; }

    public MapNode(int x, int y)
    {
        pos_x = x;
        pos_y = y;
        variation = -1;
        currentType = RoomType.None;
    }
    public void UpdateConnections(MapNode[,] m)
    {
        // Reset the
        connectionCount = 0;

        if (pos_x - 1 >= 0 && m[pos_x - 1, pos_y].IsActive()) { con_left = m[pos_x - 1, pos_y]; connectionCount++; } else { con_left = null; }
        if (pos_x + 1 < StageManagerBase.STAGE_SIZE && m[pos_x + 1, pos_y].IsActive()) { con_right = m[pos_x + 1, pos_y]; connectionCount++; } else { con_right = null; }
        if (pos_y - 1 >= 0 && m[pos_x, pos_y - 1].IsActive()) { con_down = m[pos_x, pos_y - 1]; connectionCount++; } else { con_down = null; }
        if (pos_y + 1 < StageManagerBase.STAGE_SIZE && m[pos_x, pos_y + 1].IsActive()) { con_up = m[pos_x, pos_y + 1]; connectionCount++; } else { con_up = null; }
    }
    public int GetNearbyUsedRooms(MapNode[,] m)
    {
        int cons = 0;

        if (pos_x - 1 >= 0 && m[pos_x - 1, pos_y].currentType != RoomType.None) { cons++; }
        if (pos_x + 1 < StageManagerBase.STAGE_SIZE && m[pos_x + 1, pos_y].currentType != RoomType.None) { cons++; }
        if (pos_y - 1 >= 0 && m[pos_x, pos_y - 1].currentType != RoomType.None) { cons++; }
        if (pos_y + 1 < StageManagerBase.STAGE_SIZE && m[pos_x, pos_y + 1].currentType != RoomType.None) { cons++; }

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