using UnityEngine;
using System.Collections.Generic;

public class TechSOComparer : IComparer<TechSO>
{
    public int Compare(TechSO a, TechSO b)
    {
        if (a.RelatedTrait.ID == b.RelatedTrait.ID)
        {
            if (a.Rarity == b.Rarity)
            {
                return a.ID.CompareTo(b.ID);
            }
            return a.Rarity < b.Rarity ? -1 : 1;
        }
        return a.RelatedTrait.ID.CompareTo(b.RelatedTrait.ID);
    }
}
