using UnityEngine;
using System.Collections.Generic;

public class MonoBehaviourPool<T> where T : MonoBehaviour
{
    private List<T> _pool;
    private GameObject _prefab;
    private Transform _instParent;

    private const int BASE_POOL_SIZE = 5;

    public MonoBehaviourPool(GameObject prefab, Transform instParent)
    {
        _pool = new List<T>();
        _prefab = prefab;
        _instParent = instParent;

        for (int i = 0; i < BASE_POOL_SIZE; i++) 
        {
            AddCopyToPool();
        }
    }

    public T AddCopyToPool() 
    {
        GameObject newGO = GameObject.Instantiate(_prefab, _instParent);
        T component = newGO.GetComponent<T>();
        _pool.Add(component);
        return component;
    }
    public T GetCopyFromPool() 
    {
        for (int i = 0; i < _pool.Count; i++) 
        {
            if (!_pool[i].gameObject.activeInHierarchy) { return _pool[i]; }
        }
        return AddCopyToPool();
    }
}
