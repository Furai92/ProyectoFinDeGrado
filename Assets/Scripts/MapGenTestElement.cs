using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MapGenTestElement : MonoBehaviour
{
    [SerializeField] private Image _bkRoom;
    [SerializeField] private Image _bkCorridor;
    [SerializeField] private Image _bkDeco;
    [SerializeField] private Transform _conU;
    [SerializeField] private Transform _conL;
    [SerializeField] private Transform _conR;
    [SerializeField] private Transform _conD;
    [SerializeField] private TextMeshProUGUI _conCount;
    [SerializeField] private TextMeshProUGUI _coord;


    private const float SPACING = 110f;

    public void Setup(MapNode node)
    {
        transform.localPosition = new Vector3(SPACING * node.pos_x, SPACING * node.pos_y, 0);
        gameObject.SetActive(node.currentType != MapNode.RoomType.None && node.currentType != MapNode.RoomType.Potential);
        _bkCorridor.gameObject.SetActive(node.currentType == MapNode.RoomType.Corridor);
        _bkRoom.gameObject.SetActive(node.currentType == MapNode.RoomType.Room);
        _bkDeco.gameObject.SetActive(node.currentType == MapNode.RoomType.Deco);
        _conU.gameObject.SetActive(node.con_up != null && node.currentType != MapNode.RoomType.Deco);
        _conD.gameObject.SetActive(node.con_down != null && node.currentType != MapNode.RoomType.Deco);
        _conL.gameObject.SetActive(node.con_left != null && node.currentType != MapNode.RoomType.Deco);
        _conR.gameObject.SetActive(node.con_right != null && node.currentType != MapNode.RoomType.Deco);

        switch (node.variation) 
        {
            case 1: { _bkRoom.color = _bkCorridor.color = Color.blue; break; }
            case 2: { _bkRoom.color = _bkCorridor.color = Color.green; break; }
            case 3: { _bkRoom.color = _bkCorridor.color = Color.yellow; break; }
            case 4: { _bkRoom.color = _bkCorridor.color = Color.cyan; break; }
            case 5: { _bkRoom.color = _bkCorridor.color = Color.magenta; break; }
            default: { _bkRoom.color = _bkCorridor.color = Color.red; break; } // 0 or greater than 5
        }

        _coord.text = "(" + node.pos_x + "," + node.pos_y + ")";
        _conCount.text = node.connectionCount.ToString();
    }
}
