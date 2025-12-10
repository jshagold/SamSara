using UnityEngine;

public class LobbyBootstrapper : MonoBehaviour
{
    [SerializeField] private NavigationButtonView maintenanceButtonView;
    [SerializeField] private NavigationButtonView stageButtonView;
    [SerializeField] private NavigationButtonView characterInfoButtonView;
    [SerializeField] private LobbyButtonThemeConfig _buttonThemeConfig;

    private LobbyButtonPresenter _lobbyBtnPresenter;

    public void Initialize()
    {
        _lobbyBtnPresenter = new LobbyButtonPresenter(
            maintenanceBtnView: maintenanceButtonView,
            stageButtonView: stageButtonView,
            characterInfoButtonView: characterInfoButtonView,
            lobbyButtonThemeConfig: _buttonThemeConfig
        );

        _lobbyBtnPresenter.Initialize();
    }
}