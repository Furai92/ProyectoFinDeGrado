using UnityEngine;

public class StageDeco : MonoBehaviour
{
    [field: SerializeField] public bool RandomizeRotation;

    public const float PIECE_SPACING = 20f;
    private const float PIECE_SCALE = 2f;

    public void SetUp(MapNode n)
    {
        transform.position = new Vector3(n.pos_x * PIECE_SPACING, 0, n.pos_y * PIECE_SPACING);
        transform.localScale = Vector3.one * PIECE_SCALE;
        float decoAngle = RandomizeRotation ? 90 * Random.Range(0, 4) : 0;
        transform.rotation = Quaternion.Euler(0, decoAngle, 0);
    }
}
