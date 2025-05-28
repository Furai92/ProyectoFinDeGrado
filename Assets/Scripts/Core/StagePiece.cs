using UnityEngine;
using System.Collections.Generic;

public class StagePiece : MonoBehaviour
{
    [field: SerializeField] public List<Transform> ChestSpawnPoints { get; private set; }

    [SerializeField] private Transform _u_blockedParent;
    [SerializeField] private Transform _d_blockedParent;
    [SerializeField] private Transform _l_blockedParent;
    [SerializeField] private Transform _r_blockedParent;

    [SerializeField] private List<StagePieceVariationGroup> _variationGroups;

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

        for (int i = 0; i < _variationGroups.Count; i++) 
        {
            if (Random.Range(0, 101) >= _variationGroups[i]._chance) { continue; }
            List<StagePieceVariation> compatibleVariations = new List<StagePieceVariation>();

            for (int j = 0; j < _variationGroups[i]._variations.Count; j++) 
            {
                if (_variationGroups[i]._variations[j]._requiredParent == null || _variationGroups[i]._variations[j]._requiredParent.gameObject.activeInHierarchy) 
                {
                    compatibleVariations.Add(_variationGroups[i]._variations[j]); 
                }
            }

            if (compatibleVariations.Count > 0) 
            {
                int randomIndex = Random.Range(0, compatibleVariations.Count);
                GameObject complementGO = Instantiate(compatibleVariations[randomIndex]._prefab, compatibleVariations[randomIndex]._positionParent);
                complementGO.SetActive(true);
                complementGO.transform.localPosition = Vector3.zero;
                complementGO.transform.rotation = Quaternion.Euler(0, compatibleVariations[randomIndex]._randomizeRotation ? Random.Range(0, 4) * 90 : 0, 0);
            }
        }
    }
    public Vector3 GetRandomEnemySpawnPosition() { return transform.position; }

    [System.Serializable]
    public class StagePieceVariation
    {
        [field: SerializeField] public GameObject _prefab;
        [field: SerializeField] public Transform _positionParent;
        [field: SerializeField] public Transform _requiredParent;
        [field: SerializeField] public bool _randomizeRotation;
    }
    [System.Serializable]
    public class StagePieceVariationGroup
    {
        [field: SerializeField] public float _chance;
        [field: SerializeField] public List<StagePieceVariation> _variations;
    }
}
