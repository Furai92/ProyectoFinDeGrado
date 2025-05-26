using UnityEngine;
using System.Collections.Generic;

public class StagePiece : MonoBehaviour
{
    [field: SerializeField] public List<Transform> ChestSpawnPoints { get; private set; }

    [SerializeField] private Transform _u_blockedParent;
    [SerializeField] private Transform _d_blockedParent;
    [SerializeField] private Transform _l_blockedParent;
    [SerializeField] private Transform _r_blockedParent;

    [SerializeField] private List<StagePieceVariationData> _variations;

    public const float PIECE_SPACING = 20f;
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

        for (int i = 0; i < _variations.Count; i++) 
        {
            if (Random.Range(0, 101) < _variations[i]._chance && _variations[i]._requiredParent.gameObject.activeInHierarchy) 
            {
                GameObject variationGO = Instantiate(_variations[i]._prefab, _variations[i]._positionParent);
                variationGO.SetActive(true);
                variationGO.transform.localPosition = Vector3.zero;
            }
        }
    }
    public Vector3 GetRandomEnemySpawnPosition() { return transform.position; }

    [System.Serializable]
    public class StagePieceVariationData 
    {
        [field: SerializeField] public GameObject _prefab;
        [field: SerializeField] public Transform _positionParent;
        [field: SerializeField] public Transform _requiredParent;
        [field: SerializeField] public float _chance;
    }
}
