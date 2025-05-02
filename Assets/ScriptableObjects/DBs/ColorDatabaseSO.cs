using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ColorDatabase", menuName = "Scriptable Objects/ColorDatabase")]
public class ColorDatabaseSO : ScriptableObject
{
    [field: SerializeField] public List<SerializedIdentifiedPair<Color>> ColorPresets { get; private set; }

    private Dictionary<string, Color> m_colorDictionary;

    private void GenerateColorDictionary()
    {
        m_colorDictionary = new Dictionary<string, Color>();
        for (int i = 0; i < ColorPresets.Count; i++) { m_colorDictionary.Add(ColorPresets[i].ID, ColorPresets[i].Data); }
    }
    public Color ElementToColor(GameEnums.DamageElement e)
    {
        switch (e)
        {
            case GameEnums.DamageElement.NonElemental: { return GetColor("ELEM_COLOR_PHYSICAL"); }
            case GameEnums.DamageElement.Fire: { return GetColor("ELEM_COLOR_FIRE"); }
            case GameEnums.DamageElement.Frost: { return GetColor("ELEM_COLOR_FROST"); }
            case GameEnums.DamageElement.Thunder: { return GetColor("ELEM_COLOR_THUNDER"); }
            case GameEnums.DamageElement.Void: { return GetColor("ELEM_COLOR_VOID"); }
            default: { return Color.white; }
        }
    }
    public Color RarityToColor(GameEnums.Rarity r)
    {
        switch (r)
        {
            case GameEnums.Rarity.Common: { return GetColor("RARITY_COLOR_COMMON"); }
            case GameEnums.Rarity.Rare: { return GetColor("RARITY_COLOR_RARE"); }
            case GameEnums.Rarity.Exotic: { return GetColor("RARITY_COLOR_EXOTIC"); }
            case GameEnums.Rarity.Prototype: { return GetColor("RARITY_COLOR_PROTOTYPE"); }
            case GameEnums.Rarity.Special: { return GetColor("RARITY_COLOR_SPECIAL"); }
            default: { return Color.white; }
        }
    }
    public Color DescDetailTypeToColor(TechSO.TechSODescDetail.DetailType dt)
    {
        switch (dt)
        {
            case TechSO.TechSODescDetail.DetailType.Positive: { return GetColor("DETAIL_TYPE_POSITIVE"); }
            case TechSO.TechSODescDetail.DetailType.Negative: { return GetColor("DETAIL_TYPE_NEGATIVE"); }
            case TechSO.TechSODescDetail.DetailType.Neutral: { return GetColor("DETAIL_TYPE_NEUTRAL"); }
            default: { return Color.white; }
        }
    }
    public Color GetColor(string id)
    {
        if (m_colorDictionary == null) { GenerateColorDictionary(); }
        if (m_colorDictionary.ContainsKey(id)) { return m_colorDictionary[id]; }

        return Color.white;
    }
}
