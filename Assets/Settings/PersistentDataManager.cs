using UnityEngine;
using System.Collections.Generic;

public class PersistentDataManager : MonoBehaviour
{
    private DifficultyLevelSO currentDifficulty;
    private List<PlayerTraitSO> selectedTraits;
    private StageStatsTracker.StageRecord previousRunRecord;

    private const string INSTANCE_GO_NAME = "[Persistent]GameDataManager";

    private static PersistentDataManager instance;

    private static PersistentDataManager GetInstance() 
    {
        if (instance == null) 
        {
            GameObject go = new GameObject();
            go.name = INSTANCE_GO_NAME;
            DontDestroyOnLoad(go);
            instance = go.AddComponent<PersistentDataManager>();
        }
        return instance;
    }
    public static void SetGameDifficulty(DifficultyLevelSO d) 
    {
        GetInstance().currentDifficulty = d;
    }
    public static void SetTraitSelection(List<PlayerTraitSO> traits) 
    {
        GetInstance().selectedTraits = traits;
    }
    public static List<PlayerTraitSO> GetTraitSelection() { return GetInstance().selectedTraits; }
    public static DifficultyLevelSO GetCurrentDifficulty() { return GetInstance().currentDifficulty; }
    public static void AddStageRecord(StageStatsTracker.StageRecord record) { GetInstance().previousRunRecord = record; }
    public static void DeletePreviousStageRecord() { GetInstance().previousRunRecord = null; }
    public static StageStatsTracker.StageRecord GetPreviousStageRecord() { return GetInstance().previousRunRecord; }
}
