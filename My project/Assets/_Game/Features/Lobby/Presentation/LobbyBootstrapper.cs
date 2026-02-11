using System;
using UnityEngine;

public class LobbyBootstrapper : MonoBehaviour
{
    private readonly string _logClass = $"[{nameof(LobbyBootstrapper)}]";

    [SerializeField] private NavigationButtonView _maintenanceButtonView;
    [SerializeField] private NavigationButtonView _stageButtonView;
    [SerializeField] private NavigationButtonView _characterInfoButtonView;
    [SerializeField] private LobbyButtonThemeConfig _buttonThemeConfig;

    private LobbyButtonPresenter _lobbyBtnPresenter;

    public void Initialize()
    {
        _lobbyBtnPresenter = new LobbyButtonPresenter(
            maintenanceBtnView: _maintenanceButtonView,
            stageButtonView: _stageButtonView,
            characterInfoButtonView: _characterInfoButtonView,
            lobbyButtonThemeConfig: _buttonThemeConfig
        );

        _lobbyBtnPresenter.Initialize();
    }

    private void OnDestroy()
    {
        if (_lobbyBtnPresenter == null)
            throw new InvalidOperationException($"{_logClass} OnDestroy without Initialize - _lobbyBtnPresenter is null.");
        _lobbyBtnPresenter.Dispose();
    }
}