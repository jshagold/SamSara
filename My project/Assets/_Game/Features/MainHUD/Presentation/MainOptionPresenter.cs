using System;
using UnityEngine;

public class MainOptionPresenter : IDisposable
{
    private readonly string _logClass = "[MainOptionPresenter]";

    private readonly OptionButtonView _optionButtonView;
    private readonly OptionMenuPopupView _menuPopupView;
    private readonly SettingsPopupView _settingsPopupView;

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
        _settingsPopupView.Close();

        _optionButtonView.SetOnClickAction(action: () =>
        {
            _menuPopupView.ShowPopup();
        });

        _menuPopupView.SetActions(
            onSettingsClicked: OnOpenSettings,
            onAccountClicked: () => { Debug.Log("계정 버튼 클릭"); },
            onBackgroundClicked: _menuPopupView.HidePopup
        );

        string settingLabel = LocalizationUtils.GetString("scene_main_popup_option_button_setting");
        string accountLabel = LocalizationUtils.GetString("scene_main_popup_option_button_account");

        _menuPopupView.UpdateTexts(
            settingText: settingLabel,
            accountText: accountLabel
        );
    }

    public void Dispose()
    {
        
    }

    private void OnOpenSettings()
    {
        _menuPopupView.HidePopup();

        _settingsPopupView.Open();
    }
}