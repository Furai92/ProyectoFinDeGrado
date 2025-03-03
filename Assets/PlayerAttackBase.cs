using UnityEngine;

public abstract class PlayerAttackBase : MonoBehaviour
{
    public abstract void SetUp(Vector3 pos, float direction, WeaponAttackSetupData sd, PlayerAttackBase parentAttack);
}
