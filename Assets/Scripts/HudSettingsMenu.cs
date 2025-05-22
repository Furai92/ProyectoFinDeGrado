using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class HudSettingsMenu : MonoBehaviour, IGameMenu
{
    [SerializeField] private Transform menuParent;
    [SerializeField] private TextMeshProUGUI audioTitleLabel;
    [SerializeField] private TextMeshProUGUI audioMasterLabel;
    [SerializeField] private TextMeshProUGUI audioEffectsLabel;
    [SerializeField] private TextMeshProUGUI audioMusicLabel;
    [SerializeField] private TextMeshProUGUI interfaceTitleLabel;
    [SerializeField] private TextMeshProUGUI interfaceDmgScaleLabel;

    [SerializeField] private Slider audioMasterSlider;
    [SerializeField] private Slider audioEffectsSlider;
    [SerializeField] private Slider audioMusicSlider;
    [SerializeField] private Slider interfaceDmgSizeSlider;

    private void UpdateDisplayedInfo()
    {
        audioMasterSlider.value = GameSettingsManager.GetSetting(GameSettingsManager.GameSetting.MasterVolume);
        audioEffectsSlider.value = GameSettingsManager.GetSetting(GameSettingsManager.GameSetting.EffectsVolume);
        audioMusicSlider.value = GameSettingsManager.GetSetting(GameSettingsManager.GameSetting.MusicVolume);
        interfaceDmgSizeSlider.value = GameSettingsManager.GetSetting(GameSettingsManager.GameSetting.DamageNotificationSize);
    }

    public bool CanBeOpened()
    {
        switch (StageManagerBase.GetCurrentStateType())
        {
            case StageStateBase.GameState.Rest:
            case StageStateBase.GameState.EnemyWave:
            case StageStateBase.GameState.BossFight: 
                {
                    return true;
                }
            default: 
                {
                    return false;
                }
        }
    }

    public void CloseMenu()
    {
        EventManager.OnUiMenuClosed(this);
        menuParent.gameObject.SetActive(false);
    }

    public bool IsOpen()
    {
        return menuParent.gameObject.activeInHierarchy;
    }

    public void OpenMenu()
    {
        UpdateDisplayedInfo();
        menuParent.gameObject.SetActive(true);
    }

    public void OnSettingsChanged() 
    {
        if (!IsOpen()) { return; }


        GameSettingsManager.ChangeSetting(GameSettingsManager.GameSetting.MasterVolume, audioMasterSlider.value, false);
        GameSettingsManager.ChangeSetting(GameSettingsManager.GameSetting.EffectsVolume, audioEffectsSlider.value, false);
        GameSettingsManager.ChangeSetting(GameSettingsManager.GameSetting.MusicVolume, audioMusicSlider.value, false);
        GameSettingsManager.ChangeSetting(GameSettingsManager.GameSetting.DamageNotificationSize, interfaceDmgSizeSlider.value, false);
        EventManager.OnGameSettingsChanged();
    }
}
