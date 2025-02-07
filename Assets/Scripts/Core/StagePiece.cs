using UnityEngine;

public class StagePiece : MonoBehaviour
{
    [SerializeField] private Transform _u_blockedParent;
    [SerializeField] private Transform _d_blockedParent;
    [SerializeField] private Transform _l_blockedParent;
    [SerializeField] private Transform _r_blockedParent;

    private const float PIECE_SPACING = 20f;
    private const float PIECE_SCALE = 2f;

    public void SetUp(MapNode n)
    {
        transform.position = new Vector3(n.pos_x * PIECE_SPACING, 0, n.pos_y * PIECE_SPACING);
        transform.localScale = Vector3.one * PIECE_SCALE;

        if (n.currentType == MapNode.RoomType.Deco) { return; }

        _u_blockedParent.gameObject.SetActive(n.con_up == null);
        _d_blockedParent.gameObject.SetActive(n.con_down == null);
        _r_blockedParent.gameObject.SetActive(n.con_right == null);
        _l_blockedParent.gameObject.SetActive(n.con_left == null);
    }
}
