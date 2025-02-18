using UnityEngine;

public abstract class PlayerAttackBase : MonoBehaviour
{
    public float Timescale { protected get; set; }
    public float Direction { protected get; set; }

    public void SetUp(float dir, Vector3 pos) 
    {
        Direction = dir;
        Timescale = 1;
        transform.SetPositionAndRotation(pos,  Quaternion.Euler(0, dir, 0));
        gameObject.SetActive(true);
    }
}
