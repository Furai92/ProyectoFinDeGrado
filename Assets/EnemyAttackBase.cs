using UnityEngine;

public abstract class EnemyAttackBase : MonoBehaviour
{
    public void SetUp(Vector3 pos, float direction) 
    {
        OnSetup(pos, direction);
    }
    protected abstract void OnSetup(Vector3 pos, float dir);
}
