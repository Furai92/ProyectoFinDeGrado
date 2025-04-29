using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StringDatabase", menuName = "Scriptable Objects/StringDatabase")]
public class StringDatabaseSO : ScriptableObject
{
    [field: SerializeField] public List<SerializedIdentifiedPair<string>> StringElements { get; private set; }

    private Dictionary<string, string> m_stringDictionary;

    private void GenerateStringDictionary()
    {
        m_stringDictionary = new Dictionary<string, string>();
        for (int i = 0; i < StringElements.Count; i++) { m_stringDictionary.Add(StringElements[i].ID, StringElements[i].Data); }
    }
    public string ElementToStatusName(GameEnums.DamageElement e)
    {
        switch (e)
        {
            case GameEnums.DamageElement.Physical: { return GetString("STATUS_NAME_PHYSICAL"); }
            case GameEnums.DamageElement.Fire: { return GetString("STATUS_NAME_FIRE"); }
            case GameEnums.DamageElement.Frost: { return GetString("STATUS_NAME_FROST"); }
            case GameEnums.DamageElement.Thunder: { return GetString("STATUS_NAME_THUNDER"); }
            case GameEnums.DamageElement.Void: { return GetString("STATUS_NAME_VOID"); }
            default: { return ""; }
        }
    }
    public string ElementToName(GameEnums.DamageElement e)
    {
        switch (e)
        {
            case GameEnums.DamageElement.Physical: { return GetString("ELEMENT_NAME_PHYSICAL"); }
            case GameEnums.DamageElement.Fire: { return GetString("ELEMENT_NAME_FIRE"); }
            case GameEnums.DamageElement.Frost: { return GetString("ELEMENT_NAME_FROST"); }
            case GameEnums.DamageElement.Thunder: { return GetString("ELEMENT_NAME_THUNDER"); }
            case GameEnums.DamageElement.Void: { return GetString("ELEMENT_NAME_VOID"); }
            default: { return ""; }
        }
    }
    public string ElementToDesc(GameEnums.DamageElement e)
    {
        switch (e)
        {
            case GameEnums.DamageElement.Physical: { return GetString("ELEMENT_DESC_PHYSICAL"); }
            case GameEnums.DamageElement.Fire: { return GetString("ELEMENT_DESC_FIRE"); }
            case GameEnums.DamageElement.Frost: { return GetString("ELEMENT_DESC_FROST"); }
            case GameEnums.DamageElement.Thunder: { return GetString("ELEMENT_DESC_THUNDER"); }
            case GameEnums.DamageElement.Void: { return GetString("ELEMENT_DESC_VOID"); }
            default: { return ""; }
        }
    }
    public string RarityToName(GameEnums.TechRarity e)
    {
        switch (e)
        {
            case GameEnums.TechRarity.Common: { return GetString("RARITY_NAME_COMMON"); }
            case GameEnums.TechRarity.Rare: { return GetString("RARITY_NAME_RARE"); }
            case GameEnums.TechRarity.Exotic: { return GetString("RARITY_NAME_EXOTIC"); }
            case GameEnums.TechRarity.Prototype: { return GetString("RARITY_NAME_PROTOTYPE"); }
            case GameEnums.TechRarity.Special: { return GetString("RARITY_NAME_SPECIAL"); }
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
