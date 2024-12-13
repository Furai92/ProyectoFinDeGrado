using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "AssetDatabase", menuName = "Core/AssetDatabase")]
public class GameDatabase : ScriptableObject
{
    [NonSerialized] private Dictionary<MenuSelection, Tuple<VisualTreeAsset, GameObject>> _menus = new Dictionary<MenuSelection, Tuple<VisualTreeAsset, GameObject>>();
    [SerializeField] private List<MenuSelectionEntry> _menuSelectionEntries = new List<MenuSelectionEntry>();

    public Dictionary<MenuSelection, Tuple<VisualTreeAsset, GameObject>> Menus => _menus;

    void OnEnable()
    {
        foreach (var entry in _menuSelectionEntries)
        {
            _menus.Add(entry.MenuSelection, new Tuple<VisualTreeAsset, GameObject>(entry.VisualTreeAsset, entry.UIMenu));
        }
    }

    void OnDisable()
    {
        _menus.Clear();
    }
}

[Serializable]
public class MenuSelectionEntry
{
    [SerializeField] private MenuSelection menuSelection;
    [SerializeField] private VisualTreeAsset visualTreeAsset;
    [SerializeField] private GameObject uiMenu;

    public MenuSelection MenuSelection => menuSelection;
    public VisualTreeAsset VisualTreeAsset => visualTreeAsset;
    public GameObject UIMenu => uiMenu;
}