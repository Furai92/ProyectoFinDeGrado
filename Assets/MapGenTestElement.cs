using UnityEngine;
using TMPro;

public class MapGenTestElement : MonoBehaviour
{
    [SerializeField] private Transform _bkRoom;
    [SerializeField] private Transform _bkCorridor;
    [SerializeField] private Transform _conU;
    [SerializeField] private Transform _conL;
    [SerializeField] private Transform _conR;
    [SerializeField] private Transform _conD;
    [SerializeField] private TextMeshProUGUI _conCount;
    [SerializeField] private TextMeshProUGUI _coord;


    private const float SPACING = 110f;

    public void Setup(MapGenTest.MapNode node)
    {
        transform.localPosition = new Vector3(SPACING * node.pos_x, SPACING * node.pos_y, 0);
        gameObject.SetActive(node.currentType != MapGenTest.MapNode.RoomType.None && node.currentType != MapGenTest.MapNode.RoomType.Potential);
        _bkCorridor.gameObject.SetActive(node.currentType == MapGenTest.MapNode.RoomType.Corridor);
        _bkRoom.gameObject.SetActive(node.currentType == MapGenTest.MapNode.RoomType.Room);
        _conU.gameObject.SetActive(node.con_up != null);
        _conD.gameObject.SetActive(node.con_down != null);
        _conL.gameObject.SetActive(node.con_left != null);
        _conR.gameObject.SetActive(node.con_right != null);

        _coord.text = "(" + node.pos_x + "," + node.pos_y + ")";
        _conCount.text = node.connectionCount.ToString();
    }
}
