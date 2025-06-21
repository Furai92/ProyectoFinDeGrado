using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioSource _sfxSource;

    [SerializeField] private GameDatabaseSO database;

    private Dictionary<string, AudioClip> sfxDictionary;

    void Awake()
    {
        Instance = this;
        sfxDictionary = new Dictionary<string, AudioClip>();
        foreach (SerializedIdentifiedPair<AudioClip> ac in database.AudioEffects) 
        {
            sfxDictionary.Add(ac.ID, ac.Data);
        }
    }
    void OnEnable()
    {
        EventManager.GameSettingsChangedEvent += OnSettingsChanged;
    }
    void OnDisable()
    {
        EventManager.GameSettingsChangedEvent -= OnSettingsChanged;
    }
    private void OnSettingsChanged()
    {
        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(GameSettingsManager.GetSetting(GameSettingsManager.GameSetting.MasterVolume) * 20));
        _audioMixer.SetFloat("SFXVolume", Mathf.Log10(GameSettingsManager.GetSetting(GameSettingsManager.GameSetting.EffectsVolume) * 20));
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(GameSettingsManager.GetSetting(GameSettingsManager.GameSetting.MusicVolume) * 20));
    }
    public static void PlaySound(string id)
    {
        if (Instance == null) { return; }
        if (!Instance.sfxDictionary.ContainsKey(id)) { return; }

        PlaySound(Instance.sfxDictionary[id]);
    }
    public static void PlaySound(AudioClip audioClip)
    {
        if (Instance == null) { return; }
        if (audioClip == null) { return; }

        Instance._sfxSource.clip = audioClip;
        Instance._sfxSource.PlayOneShot(audioClip);
    }

    public static void StopSfx()
    {
        if (Instance == null) { return; }

        Instance._sfxSource.Stop();
    }

    public static void PauseSound()
    {
        if (Instance == null) { return; }

        Instance._sfxSource.Pause();
    }

    public static void ResumeSound()
    {
        if (Instance == null) { return; }

        Instance._sfxSource.Play();
    }
}