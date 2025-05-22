using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{

    public enum GameSetting { MasterVolume, MusicVolume, EffectsVolume, RotationCamSpeed, DamageNotificationSize }

    private float[] settings;

    private const string INSTANCE_GO_NAME = "[Persistent]GameSettingsManager";
    private const int SETTINGS_ARRAY_SIZE = 10;

    private static GameSettingsManager instance;

    private static void Initialize() 
    {
        GameObject go = new GameObject();
        go.name = INSTANCE_GO_NAME;
        DontDestroyOnLoad(go);
        instance = go.AddComponent<GameSettingsManager>();
        ResetToDefault();
    }
    public static float GetSetting(GameSetting s) 
    {
        if (instance == null) { Initialize(); }

        return instance.settings[(int)s];
    }
    public static void ChangeSetting(GameSetting s, float v, bool launchEvent) 
    {
        if (instance == null) { Initialize(); }

        instance.settings[(int)s] = v;
        if (launchEvent) { EventManager.OnGameSettingsChanged(); }
    }
    public static void ResetToDefault() 
    {
        instance.settings = new float[SETTINGS_ARRAY_SIZE];
        instance.settings[(int)GameSetting.MasterVolume] = 0.5f;
        instance.settings[(int)GameSetting.MusicVolume] = 0.5f;
        instance.settings[(int)GameSetting.EffectsVolume] = 0.5f;
        instance.settings[(int)GameSetting.RotationCamSpeed] = 0.5f;
        instance.settings[(int)GameSetting.DamageNotificationSize] = 0.5f;

        EventManager.OnGameSettingsChanged();
    }
}
