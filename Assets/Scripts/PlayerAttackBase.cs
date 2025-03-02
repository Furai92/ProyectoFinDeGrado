using UnityEngine;

public abstract class PlayerAttackBase : MonoBehaviour
{
    public WeaponAttackSetupData SetupData { get; protected set; }
    public float Direction { get; protected set; }
    public float BouncesDone { get; protected set; }
    public float PiercesDone { get; protected set; }

    public void SetUp(WeaponAttackSetupData sd, float dir, Vector3 pos) 
    {
        SetupData = sd;
        BouncesDone = 0;
        PiercesDone = 0;
        Direction = dir;
        transform.SetPositionAndRotation(pos,  Quaternion.Euler(0, dir, 0));
        gameObject.SetActive(true);
    }
    protected void DamageEnemy(EnemyEntity target) 
    {
    }
}
