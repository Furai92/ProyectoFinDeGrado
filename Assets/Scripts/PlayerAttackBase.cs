using UnityEngine;

public abstract class PlayerAttackBase : MonoBehaviour
{
    public WeaponAttackSetupData SetupData { get; protected set; }
    public float Direction { get; protected set; }
    public float BouncesRemaining { get; protected set; }
    public float PiercesRemaining { get; protected set; }

    public void SetUp(WeaponAttackSetupData sd, float dir, Vector3 pos) 
    {
        SetupData = sd;
        BouncesRemaining = sd.bounces;
        PiercesRemaining = sd.pierces;
        Direction = dir;
        transform.SetPositionAndRotation(pos,  Quaternion.Euler(0, dir, 0));
        gameObject.SetActive(true);
    }
}
