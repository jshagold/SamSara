using System;
using UnityEngine;

public class MainOptionPresenter : IDisposable
{
    private readonly string _logClass = "[MainOptionPresenter]";

    private readonly OptionButtonView _optionButtonView;
    private readonly OptionMenuPopupView _menuPopupView;
    private readonly SettingsPopupView _settingsPopupView;

    // Data (간단하게 PlayerPrefs 래퍼 사용 가정, 나중에 정식 Repo로 교체)
    // private readonly ISettingsRepository _settingsRepo;

    public MainOptionPresenter(
        OptionButtonView optionButtonView,
        OptionMenuPopupView menuPopupView,
        SettingsPopupView settingsPopupView)
    {
        _optionButtonView = optionButtonView;
        _menuPopupView = menuPopupView;
        _settingsPopupView = settingsPopupView;
    }

    public void Initialize()
    {
        _menuPopupView.HidePopup();
        _settingsPopupView.ClosePopup();

        _optionButtonView.SetOnClickAction(action: () =>
        {
            _menuPopupView.ShowPopup();
        });

        _menuPopupView.SetActions(
            onSettingsClicked: OnOpenSettings,
            onAccountClicked: () => { },
            onBackgroundClicked: _menuPopupView.HidePopup
        );
        string settingLabel = LocalizationUtils.GetString("scene_main_popup_option_button_setting");
        string accountLabel = LocalizationUtils.GetString("scene_main_popup_option_button_account");

        _menuPopupView.UpdateTexts(
            settingText: settingLabel,
            accountText: accountLabel
        );

        _settingsPopupView.SetEvents(
            onClosePopup: _settingsPopupView.ClosePopup,
            onSoundToggle: OnSoundChanged
        );
    }

    public void Dispose()
    {

    }

    private void OnOpenSettings()
    {
        _menuPopupView.HidePopup();

        // TODO Repository에서 갖고오는걸로 수정해야함 임시로 PlayerPrefs 사용
        bool isSoundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;

        _settingsPopupView.OpenPopup(isSoundOn);
    }

    private void OnSoundChanged(bool isOn)
    {
        // TODO Repository methods 호출해야함
        //
        PlayerPrefs.SetInt("SoundOn", isOn ? 1 : 0);
        PlayerPrefs.Save();

        // TODO 실제 오디오 매니저에 적용해야함.
        // ex) AudioManager.Instance.SetMute(!isOn);
        Debug.Log($"{_logClass} 소리 설정 변경: {isOn}");
    }
}