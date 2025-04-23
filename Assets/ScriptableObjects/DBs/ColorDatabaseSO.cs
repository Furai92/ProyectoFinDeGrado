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
            case GameEnums.DamageElement.Physical: { return GetColor("ELEM_COLOR_PHYSICAL"); }
            case GameEnums.DamageElement.Fire: { return GetColor("ELEM_COLOR_FIRE"); }
            case GameEnums.DamageElement.Frost: { return GetColor("ELEM_COLOR_FROST"); }
            case GameEnums.DamageElement.Thunder: { return GetColor("ELEM_COLOR_THUNDER"); }
            case GameEnums.DamageElement.Void: { return GetColor("ELEM_COLOR_VOID"); }
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
