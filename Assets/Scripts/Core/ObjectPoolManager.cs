using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;

    private static ObjectPoolManager _instance;

    private void OnEnable()
    {
        _instance = this;
        InitializePools();
    }
    private void InitializePools() 
    {
    }

    

}
