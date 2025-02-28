using UnityEngine;

public class CameraAntiClip : MonoBehaviour
{
    [SerializeField] private Camera c;
    [SerializeField] private Transform camTargetPos;

    private LayerMask mask;

    private const float MAX_CAM_DIST = 5f;
    private const float WALL_SEPARATION = 0.2f;

    private void OnEnable()
    {
        mask = LayerMask.GetMask("Walls", "Ground");
    }
    void Update()
    {
        RaycastHit h;
        Physics.Raycast(transform.position, camTargetPos.position - transform.position, out h, MAX_CAM_DIST, mask);

        if (h.collider == null)
        {
            c.transform.localPosition = camTargetPos.localPosition;
        }
        else 
        {
            c.transform.localPosition = Vector3.MoveTowards(transform.localPosition, camTargetPos.localPosition, h.distance - WALL_SEPARATION);
        }
    }
}
