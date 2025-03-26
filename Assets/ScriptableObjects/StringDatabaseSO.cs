using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StringDatabase", menuName = "Scriptable Objects/StringDatabase")]
public class StringDatabaseSO : ScriptableObject
{
    [field: SerializeField] public List<InspectorPair<string>> StringElements { get; private set; }

    private Dictionary<string, string> m_stringDictionary;

    private void GenerateStringDictionary()
    {
        m_stringDictionary = new Dictionary<string, string>();
        for (int i = 0; i < StringElements.Count; i++) { m_stringDictionary.Add(StringElements[i].ID, StringElements[i].Data); }
    }
    public string StatusToString(GameEnums.DamageElement e)
    {
        switch (e)
        {
            case GameEnums.DamageElement.Physical: { return GetString("STATUS_NAME_PHYSICAL"); }
            case GameEnums.DamageElement.Fire: { return GetString("STATUS_NAME_FIRE"); }
            case GameEnums.DamageElement.Frost: { return GetString("STATUS_NAME_FROST"); }
            case GameEnums.DamageElement.Thunder: { return GetString("STATUS_NAME_THUNDER"); }
            default: { return ""; }
        }
    }
    public string GetString(string id)
    {
        if (m_stringDictionary == null) { GenerateStringDictionary(); }
        if (m_stringDictionary.ContainsKey(id)) { return m_stringDictionary[id]; }

        return string.Format("[{0}]", id);
    }
}
