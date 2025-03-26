using UnityEngine;

[System.Serializable]
public class InspectorPair<T>
{
    [field: SerializeField] public string ID { get; private set; }
    [field: SerializeField] public T Data { get; private set; }
}

