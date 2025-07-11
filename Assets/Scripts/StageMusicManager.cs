using UnityEngine;
using System.Collections;

public class StageMusicManager : MonoBehaviour
{
    [Range(0,1)] [SerializeField] private float audioLevelMultiplier;

    [SerializeField] private AudioSource asrc;
    [SerializeField] private AudioClip standardMusic;
    [SerializeField] private AudioClip combatMusic;
    [SerializeField] private AudioClip restMusic;

    private float settingsMult;
    private float fadeMult;

    private const float MUSIC_FADE_DURATION = 1.0f;

    private void OnEnable()
    {
        fadeMult = 1;
        OnSettingsChanged();
        ChangeMusic(standardMusic);
        EventManager.StageStateStartedEvent += OnStageStateStarted;
        EventManager.GameSettingsChangedEvent += OnSettingsChanged;
    }

    private void OnDisable()
    {
        EventManager.StageStateStartedEvent -= OnStageStateStarted;
        EventManager.GameSettingsChangedEvent -= OnSettingsChanged;
    }

    private void OnStageStateStarted(StageStateBase.GameState s) 
    {
        switch (s) 
        {
            case StageStateBase.GameState.EnemyWave: { ChangeMusic(combatMusic); break; }
            case StageStateBase.GameState.Rest: { ChangeMusic(restMusic); break; }
        }
    }
    private void OnSettingsChanged() 
    {
        settingsMult = GameSettingsManager.GetSetting(GameSettingsManager.GameSetting.MusicVolume) * GameSettingsManager.GetSetting(GameSettingsManager.GameSetting.MasterVolume);
        UpdateAudioVolume();
    }
    private void UpdateAudioVolume() 
    {
        asrc.volume = audioLevelMultiplier * settingsMult * fadeMult;
    }

    private void ChangeMusic(AudioClip ac) 
    {
        if (ac != null) print("changing to " + ac.name);

        if (ac == null) { return; }
        if (asrc.clip == ac) { return; }


        StopAllCoroutines();
        StartCoroutine(ChangeMusicCR(ac));
    }
    IEnumerator ChangeMusicCR(AudioClip ac) 
    {
        float t = 0;
        if (asrc.clip != null) 
        {
            while (t > 0) 
            {
                t = Mathf.MoveTowards(t, 0, Time.deltaTime / MUSIC_FADE_DURATION);
                fadeMult = 1 - t;
                UpdateAudioVolume();
                yield return null;
            }
        }
        t = 0;
        asrc.Stop();
        asrc.clip = ac;
        asrc.Play();
        while (t <= 1)
        {
            t = Mathf.MoveTowards(t, 1, Time.deltaTime / MUSIC_FADE_DURATION);
            fadeMult = t;
            UpdateAudioVolume();
            yield return null;
        }
    }
}
