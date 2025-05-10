using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ColorDatabase", menuName = "Scriptable Objects/ColorDatabase")]
public class ColorDatabaseSO : ScriptableObject
{
    [field: SerializeField] public List<SerializedIdentifiedPair<Color>> ColorPresets { get; private set; }
    [field: SerializeField] public List<SerializedIdentifiedPair<TMPro.TMP_ColorGradient>> GradientPresets { get; private set; }

    private Dictionary<string, Color> m_colorDictionary;
    private Dictionary<string, TMPro.TMP_ColorGradient> m_gradDictionary;

    private void GenerateDictionaries()
    {
        m_colorDictionary = new Dictionary<string, Color>();
        for (int i = 0; i < ColorPresets.Count; i++) { m_colorDictionary.Add(ColorPresets[i].ID, ColorPresets[i].Data); }

        m_gradDictionary = new Dictionary<string, TMPro.TMP_ColorGradient>();
        for (int i = 0; i < GradientPresets.Count; i++) { m_gradDictionary.Add(GradientPresets[i].ID, GradientPresets[i].Data); }
    }
    public Color ElementToColor(GameEnums.DamageElement e) 
    {
        switch (e)
        {
            case GameEnums.DamageElement.NonElemental: { return GetColor("ELEM_COLOR_NON_ELEMENTAL"); }
            case GameEnums.DamageElement.Fire: { return GetColor("ELEM_COLOR_FIRE"); }
            case GameEnums.DamageElement.Frost: { return GetColor("ELEM_COLOR_FROST"); }
            case GameEnums.DamageElement.Thunder: { return GetColor("ELEM_COLOR_THUNDER"); }
            case GameEnums.DamageElement.Void: { return GetColor("ELEM_COLOR_VOID"); }
            default: { return Color.white; }
        }
    }
    public TMPro.TMP_ColorGradient ElementToGradient(GameEnums.DamageElement e)
    {
        switch (e)
        {
            case GameEnums.DamageElement.NonElemental: { return GetGradient("ELEM_GRAD_NON_ELEMENTAL"); }
            case GameEnums.DamageElement.Fire: { return GetGradient("ELEM_GRAD_FIRE"); }
            case GameEnums.DamageElement.Frost: { return GetGradient("ELEM_GRAD_FROST"); }
            case GameEnums.DamageElement.Thunder: { return GetGradient("ELEM_GRAD_THUNDER"); }
            case GameEnums.DamageElement.Void: { return GetGradient("ELEM_GRAD_VOID"); }
            case GameEnums.DamageElement.Incineration: { return GetGradient("ELEM_GRAD_INCINERATION"); }
            case GameEnums.DamageElement.Frostbite: { return GetGradient("ELEM_GRAD_FROSTBITE"); }
            default: { return new TMPro.TMP_ColorGradient(); }
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
        if (m_colorDictionary == null) { GenerateDictionaries(); }
        if (m_colorDictionary.ContainsKey(id)) { return m_colorDictionary[id]; }

        return Color.white;
    }
    public TMPro.TMP_ColorGradient GetGradient(string id) 
    {
        if (m_gradDictionary == null) { GenerateDictionaries(); }
        if (m_gradDictionary.ContainsKey(id)) { return m_gradDictionary[id]; }

        return new TMPro.TMP_ColorGradient();
    }
}
