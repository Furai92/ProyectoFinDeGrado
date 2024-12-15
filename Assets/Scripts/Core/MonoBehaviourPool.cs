using UnityEngine;
using System.Collections.Generic;

public class MonoBehaviourPool<T> where T : MonoBehaviour
{
    private List<PoolPair> _pool;
    private GameObject _prefab;
    private Transform _instParent;

    private const int BASE_POOL_SIZE = 5;

    public MonoBehaviourPool(GameObject prefab, Transform instParent)
    {
        _pool = new List<PoolPair>();
        _prefab = prefab;
        _instParent = instParent;

        for (int i = 0; i < BASE_POOL_SIZE; i++) 
        {
            AddCopyToPool();
        }
    }

    public PoolPair AddCopyToPool() 
    {
        PoolPair p = new PoolPair();
        GameObject g = GameObject.Instantiate(_prefab, _instParent);
        p.GO = g;
        p.Component = g.GetComponent<T>();
        _pool.Add(p);
        return p;
    }
    public PoolPair GetCopyFromPool() 
    {
        for (int i = 0; i < _pool.Count; i++) 
        {
            if (!_pool[i].GO.activeInHierarchy) { return _pool[i]; }
        }
        return AddCopyToPool();
    }


    public struct PoolPair 
    {
        public T Component;
        public GameObject GO;
    }
}
