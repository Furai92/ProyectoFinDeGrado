using UnityEngine;

[CreateAssetMenu(fileName = "MilestoneSO", menuName = "Scriptable Objects/MilestoneSO")]
public class MilestoneSO : ScriptableObject
{
    [field: SerializeField] public string ID { get; private set; }
    [field: SerializeField] public string NameID { get; private set; }
    [field: SerializeField] public string DescID { get; private set; }
    [field: SerializeField] public string GameProgressCheckID { get; private set; }
    [field: SerializeField] public int GameProgressCheckReq { get; private set; }
}
