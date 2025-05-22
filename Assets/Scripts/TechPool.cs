using UnityEngine;
using System.Collections.Generic;

public class TechPool
{
    private List<List<TechSO>> generalTechs;
    private List<List<TechSO>> boostedRateTechs;
    private List<List<TechSO>> loweredRateTechs;

    private const string GENERAL_TRAIT_ID = "GENERAL";
    private const float BOOSTED_CHANCE = 90f;
    private const float CHANCE_TO_GENERAL = 40f;
    private const int RARITY_LEVELS = 5;

    public TechPool(GameDatabaseSO db, List<PlayerTraitSO> boostedRateTraits) 
    {
        boostedRateTechs = new List<List<TechSO>>();
        loweredRateTechs = new List<List<TechSO>>();
        generalTechs = new List<List<TechSO>>();
        for (int i = 0; i < RARITY_LEVELS; i++) 
        {
            boostedRateTechs.Add(new List<TechSO>());
            loweredRateTechs.Add(new List<TechSO>());
            generalTechs.Add(new List<TechSO>());
        }

        for (int i = 0; i < db.Techs.Count; i++) 
        {
            if (db.Techs[i].RelatedTrait.ID == GENERAL_TRAIT_ID) 
            {
                generalTechs[(int)db.Techs[i].Rarity].Add(db.Techs[i]);
            }
            else if (boostedRateTraits.Contains(db.Techs[i].RelatedTrait))
            {
                boostedRateTechs[(int)db.Techs[i].Rarity].Add(db.Techs[i]);
            }
            else 
            {
                loweredRateTechs[(int)db.Techs[i].Rarity].Add(db.Techs[i]);
            }
        }
    }
    public TechSO GetRandomTech(GameEnums.Rarity r, Dictionary<string, PlayerEntity.TechGroup> alreadyActiveTech) 
    {
        List<TechSO> poolSelected = Random.Range(0, 101) < BOOSTED_CHANCE ? boostedRateTechs[(int)r] : loweredRateTechs[(int)r];
        List<TechSO> compatibleTech = new List<TechSO>();
        List<TechSO> compatibleGeneralTech = new List<TechSO>();

        for (int i = 0; i < poolSelected.Count; i++) 
        {
            TechSO techReaded = poolSelected[i];
            if (alreadyActiveTech.ContainsKey(techReaded.ID)) 
            {
                if (alreadyActiveTech[techReaded.ID].Level >= techReaded.MaxLevel && techReaded.MaxLevel > 0) { continue; }
            }
            compatibleTech.Add(techReaded);
        }
        for (int i = 0; i < generalTechs[(int)r].Count; i++)
        {
            TechSO techReaded = generalTechs[(int)r][i];
            if (alreadyActiveTech.ContainsKey(techReaded.ID))
            {
                if (alreadyActiveTech[techReaded.ID].Level >= techReaded.MaxLevel && techReaded.MaxLevel > 0) { continue; }
            }
            compatibleGeneralTech.Add(techReaded);
        }

        if (compatibleTech.Count == 0 || Random.Range(0, 101) < CHANCE_TO_GENERAL) 
        {
            return compatibleGeneralTech[Random.Range(0, compatibleGeneralTech.Count)];  
        }

        return compatibleTech[Random.Range(0, compatibleTech.Count)];
    }
}
