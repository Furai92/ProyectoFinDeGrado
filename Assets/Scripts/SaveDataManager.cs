using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SaveDataManager : MonoBehaviour
{
    private const string FILENAME = "SavedData";
    private const string EXTENSION = ".asd";

    public static void SaveToFile()
    {
        SaveData sd = new SaveData();
        sd.firstTimePlay = PersistentDataManager.IsFirstTimePlay();
        // Settings data
        sd.SettingsData = new SaveDataSettings()
        {
            AudioSfx = GameSettingsManager.GetSetting(GameSettingsManager.GameSetting.EffectsVolume),
            AudioMaster = GameSettingsManager.GetSetting(GameSettingsManager.GameSetting.MasterVolume),
            AudioMusic = GameSettingsManager.GetSetting(GameSettingsManager.GameSetting.MusicVolume),
            Fov = GameSettingsManager.GetSetting(GameSettingsManager.GameSetting.CamFov),
            NotifSize = GameSettingsManager.GetSetting(GameSettingsManager.GameSetting.DamageNotificationSize),
            CamRotationSpeed = GameSettingsManager.GetSetting(GameSettingsManager.GameSetting.RotationCamSpeed)
        };
        // Progress data
        List<System.Tuple<string, int>> gameProgress = PersistentDataManager.GetAllGameProgress();
        foreach (System.Tuple<string, int> gp in gameProgress) 
        {
            sd.ProgressData.Add(new SaveDataProgressPair(gp.Item1, gp.Item2));
        }

        try
        {
            string output = JsonUtility.ToJson(sd, true);
            string filepath = GetFilePath();
            File.WriteAllText(filepath, output);
            print("saving data to " + filepath);
        }
        catch (IOException e)
        {
            print(e);
        }
    }
    public static void LoadFromFile()
    {
        SaveData sd;

        if (File.Exists(GetFilePath()))
        {
            string jsonReaded = File.ReadAllText(GetFilePath());
            sd = JsonUtility.FromJson<SaveData>(jsonReaded);
        }
        else 
        {
            sd = new SaveData();
        }
        PersistentDataManager.SetFirstTimePlay(sd.firstTimePlay);
        PersistentDataManager.ResetGameProgress();
        foreach (SaveDataProgressPair sdpp in sd.ProgressData) 
        {
            PersistentDataManager.AddGameProgress(sdpp.ID, sdpp.Progress);
        }
        GameSettingsManager.ChangeSetting(GameSettingsManager.GameSetting.MasterVolume, sd.SettingsData.AudioMaster, false);
        GameSettingsManager.ChangeSetting(GameSettingsManager.GameSetting.EffectsVolume, sd.SettingsData.AudioSfx, false);
        GameSettingsManager.ChangeSetting(GameSettingsManager.GameSetting.MusicVolume, sd.SettingsData.AudioMusic, false);
        GameSettingsManager.ChangeSetting(GameSettingsManager.GameSetting.CamFov, sd.SettingsData.Fov, false);
        GameSettingsManager.ChangeSetting(GameSettingsManager.GameSetting.DamageNotificationSize, sd.SettingsData.NotifSize, false);
        GameSettingsManager.ChangeSetting(GameSettingsManager.GameSetting.RotationCamSpeed, sd.SettingsData.CamRotationSpeed, true); // Last one will launch "OnSettingsChanged" event
    }

    public static string GetFilePath()
    {
        return string.Format("{0}/{1}{2}", Application.persistentDataPath, FILENAME, EXTENSION);
    }

    [System.Serializable]
    public class SaveData 
    {
        public List<SaveDataProgressPair> ProgressData;
        public SaveDataSettings SettingsData;
        public bool firstTimePlay;

        public SaveData() 
        {
            ProgressData = new List<SaveDataProgressPair>();
            SettingsData = new SaveDataSettings();
            firstTimePlay = true;
        }
    }
    [System.Serializable]
    public class SaveDataProgressPair 
    {
        public string ID;
        public int Progress;

        public SaveDataProgressPair(string _id, int _prog) 
        {
            ID = _id; 
            Progress = _prog; 
        }
    }
    [System.Serializable]
    public class SaveDataSettings 
    {
        public float AudioMaster;
        public float AudioSfx;
        public float AudioMusic;
        public float Fov;
        public float NotifSize;
        public float CamRotationSpeed;

        public SaveDataSettings() 
        {
            AudioMaster = 0.5f;
            AudioSfx = 0.5f;
            AudioMusic = 0.5f;
            Fov = 0.5f;
            NotifSize = 0.25f;
            CamRotationSpeed = 0.5f;
        }
    }
}
