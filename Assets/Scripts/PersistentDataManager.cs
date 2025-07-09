using UnityEngine;
using System.Collections.Generic;

public class PersistentDataManager : MonoBehaviour
{
    private DifficultyLevelSO currentDifficulty;
    private List<PlayerTraitSO> selectedTraits;
    private StageStatsTracker.StageRecord previousRunRecord;
    private Dictionary<string, int> gameProgress;
    private List<string> gameProgressKeys;
    private bool firstTimePlay;

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
            instance.gameProgress = new Dictionary<string, int>();
            instance.gameProgressKeys = new List<string>();
            instance.firstTimePlay = true;
        }
        return instance;
    }
    public static int GetGameProgress(string id) 
    {
        if (GetInstance().gameProgress.ContainsKey(id)) { return GetInstance().gameProgress[id]; }

        return 0;
    }
    public static List<System.Tuple<string, int>> GetAllGameProgress() 
    {
        List<System.Tuple<string, int>> progress = new List<System.Tuple<string, int>>();

        foreach (string key in GetInstance().gameProgressKeys) 
        {
            progress.Add(new System.Tuple<string, int>(key, GetInstance().gameProgress[key]));
        }
        return progress;
    }
    public static void AddGameProgress(string id, int progress) 
    {
        if (GetInstance().gameProgress.ContainsKey(id))
        {
            GetInstance().gameProgress[id] += progress;
        }
        else 
        {
            GetInstance().gameProgress.Add(id, progress);
            GetInstance().gameProgressKeys.Add(id);
        }
    }
    public static void ResetGameProgress() 
    {
        GetInstance().gameProgress = new Dictionary<string, int>();
        GetInstance().gameProgressKeys = new List<string>();
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
    public static bool IsFirstTimePlay() { return GetInstance().firstTimePlay; }
    public static void SetFirstTimePlay(bool ftp) { GetInstance().firstTimePlay = ftp; }
}
