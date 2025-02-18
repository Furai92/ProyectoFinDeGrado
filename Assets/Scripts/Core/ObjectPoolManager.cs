using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;

    private MonoBehaviourPool<PlayerAttackBase> projpool;

    private static ObjectPoolManager _instance;

    private void OnEnable()
    {
        _instance = this;
        InitializePools();
    }
    private void InitializePools() 
    {
        projpool = new MonoBehaviourPool<PlayerAttackBase>(_projectilePrefab, transform);
    }

    public static PlayerAttackBase GetPlayerAttackFromPool(string id) 
    {
        if (_instance == null) { return null; }

        return _instance.projpool.GetCopyFromPool();
    }

}
