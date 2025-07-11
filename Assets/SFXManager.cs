using UnityEngine;
using System.Collections.Generic;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource src;
    [SerializeField] private GameDatabaseSO dbso;

    private Dictionary<string, AudioClip> audioDictionary;

    private static SFXManager instance;

    private const float SFX_BASE_STRENGHT = 0.2f;


    private void OnEnable()
    {
        instance = this;

        audioDictionary = new Dictionary<string, AudioClip>();
        List<SerializedIdentifiedPair<AudioClip>> dbClips = dbso.AudioEffects;
        foreach (SerializedIdentifiedPair<AudioClip> p in dbClips) 
        {
            audioDictionary.Add(p.ID, p.Data);
        }
        UpdateSettings();
        EventManager.GameSettingsChangedEvent += UpdateSettings;
    }
    private void OnDisable()
    {
        EventManager.GameSettingsChangedEvent -= UpdateSettings;
    }
    private void UpdateSettings() 
    {
        src.volume = SFX_BASE_STRENGHT * GameSettingsManager.GetSetting(GameSettingsManager.GameSetting.EffectsVolume) * GameSettingsManager.GetSetting(GameSettingsManager.GameSetting.MasterVolume);
    }

    public static void PlaySFX(string id) 
    {
        if (instance == null) { return; }
        if (!instance.audioDictionary.ContainsKey(id)) { return; }

        instance.src.PlayOneShot(instance.audioDictionary[id]);
    }
}
