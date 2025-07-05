using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerTrait", menuName = "Scriptable Objects/PlayerTrait")]
public class PlayerTraitSO : ScriptableObject
{
    [field: SerializeField] public string ID { get; private set; }
    [field: SerializeField] public string NameID { get; private set; }
    [field: SerializeField] public string DescID { get; private set; }
    [field: SerializeField] public Color IconColor { get; private set; }
    [field: SerializeField] public TechSO Tech { get; private set; }
}
