using UnityEngine;

[System.Serializable]
public class SerializedPair<T, U>
{
    [field: SerializeField] public T First { get; private set; }
    [field: SerializeField] public U Second { get; private set; }
}

