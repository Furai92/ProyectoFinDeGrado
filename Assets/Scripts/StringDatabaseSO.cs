using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StringDatabase", menuName = "Scriptable Objects/StringDatabase")]
public class StringDatabaseSO : ScriptableObject
{
    [field: SerializeField] public List<SerializedIdentifiedPair<string>> StringElements { get; private set; }
    [field: SerializeField] public List<SerializedIdentifiedPair<string>> Keywords { get; private set; } 

    private Dictionary<string, string> m_stringDictionary;
    private Dictionary<string, string> m_keywordDictionary;

    private void GenerateDataDictionary()
    {
        m_stringDictionary = new Dictionary<string, string>();
        for (int i = 0; i < StringElements.Count; i++) { m_stringDictionary.Add(StringElements[i].ID, StringElements[i].Data); }

        m_keywordDictionary = new Dictionary<string, string>();
        for (int i = 0; i < Keywords.Count; i++) { m_stringDictionary.Add(Keywords[i].ID, Keywords[i].Data); }
    }
    public string ElementToStatusName(GameEnums.DamageElement e)
    {
        switch (e)
        {
            case GameEnums.DamageElement.NonElemental: { return GetString("STATUS_NAME_PHYSICAL"); }
            case GameEnums.DamageElement.Fire: { return GetString("STATUS_NAME_FIRE"); }
            case GameEnums.DamageElement.Frost: { return GetString("STATUS_NAME_FROST"); }
            case GameEnums.DamageElement.Thunder: { return GetString("STATUS_NAME_THUNDER"); }
            case GameEnums.DamageElement.Void: { return GetString("STATUS_NAME_VOID"); }
            case GameEnums.DamageElement.Incineration: { return GetString("STATUS_NAME_INCINERATION"); }
            case GameEnums.DamageElement.Frostbite: { return GetString("STATUS_NAME_FROSTBITE"); }

            default: { return ""; }
        }
    }
    public string ElementToName(GameEnums.DamageElement e)
    {
        switch (e)
        {
            case GameEnums.DamageElement.NonElemental: { return GetString("ELEMENT_NAME_NONELEMENTAL"); }
            case GameEnums.DamageElement.Fire: { return GetString("ELEMENT_NAME_FIRE"); }
            case GameEnums.DamageElement.Frost: { return GetString("ELEMENT_NAME_FROST"); }
            case GameEnums.DamageElement.Thunder: { return GetString("ELEMENT_NAME_THUNDER"); }
            case GameEnums.DamageElement.Void: { return GetString("ELEMENT_NAME_VOID"); }
            case GameEnums.DamageElement.Incineration: { return GetString("ELEMENT_NAME_INCINERATION"); }
            case GameEnums.DamageElement.Frostbite: { return GetString("ELEMENT_NAME_FROSTBITE"); }
            default: { return ""; }
        }
    }
    public string ElementToDesc(GameEnums.DamageElement e)
    {
        switch (e)
        {
            case GameEnums.DamageElement.NonElemental: { return GetString("ELEMENT_DESC_NONELEMENTAL"); }
            case GameEnums.DamageElement.Fire: { return GetString("ELEMENT_DESC_FIRE"); }
            case GameEnums.DamageElement.Frost: { return GetString("ELEMENT_DESC_FROST"); }
            case GameEnums.DamageElement.Thunder: { return GetString("ELEMENT_DESC_THUNDER"); }
            case GameEnums.DamageElement.Void: { return GetString("ELEMENT_DESC_VOID"); }
            default: { return ""; }
        }
    }
    public string RarityToName(GameEnums.Rarity e)
    {
        switch (e)
        {
            case GameEnums.Rarity.Common: { return GetString("RARITY_NAME_COMMON"); }
            case GameEnums.Rarity.Rare: { return GetString("RARITY_NAME_RARE"); }
            case GameEnums.Rarity.Exotic: { return GetString("RARITY_NAME_EXOTIC"); }
            case GameEnums.Rarity.Prototype: { return GetString("RARITY_NAME_PROTOTYPE"); }
            case GameEnums.Rarity.Special: { return GetString("RARITY_NAME_SPECIAL"); }
            default: { return ""; }
        }
    }
    public bool IsKeyword(string s) 
    {
        if (m_keywordDictionary == null) { GenerateDataDictionary(); }
        return m_keywordDictionary.ContainsKey(s);
    }
    public string GetString(string id)
    {
        if (m_stringDictionary == null) { GenerateDataDictionary(); }
        if (m_stringDictionary.ContainsKey(id)) { return m_stringDictionary[id]; }

        return string.Format("[{0}]", id);
    }
}
