using UnityEngine;

public class test_deleteme : MonoBehaviour
{
    public Transform a;
    public Transform b;
    public Transform c;
    public Transform d;
    public Transform e;

    public float aimdir;
    public float spacing;

    private void Update()
    {
        a.SetPositionAndRotation(transform.position, Quaternion.Euler(0, aimdir, 0));
        b.SetPositionAndRotation(transform.position + GameTools.OffsetFromAngle(-aimdir + 90, spacing), Quaternion.Euler(0, aimdir, 0));
        c.SetPositionAndRotation(transform.position + GameTools.OffsetFromAngle(-aimdir - 90, spacing), Quaternion.Euler(0, aimdir, 0));
        d.SetPositionAndRotation(transform.position + GameTools.OffsetFromAngle(-aimdir + 90, spacing * 2), Quaternion.Euler(0, aimdir, 0));
        e.SetPositionAndRotation(transform.position + GameTools.OffsetFromAngle(-aimdir - 90, spacing * 2), Quaternion.Euler(0, aimdir, 0));

    }
}
