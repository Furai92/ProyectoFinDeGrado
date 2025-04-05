using UnityEngine;

public abstract class EnemyAttackBase : MonoBehaviour
{
    public abstract void SetUp(EnemyEntity user, Vector3 pos, float direction);
}
