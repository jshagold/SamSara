using UnityEngine;

public class LobbyBootstrapper : MonoBehaviour
{
    [SerializeField] private MaintenanceButtonView maintenanceButtonView;

    private LobbyButtonPresenter _lobbyBtnPresenter;

    public void Initialize()
    {
        _lobbyBtnPresenter = new LobbyButtonPresenter(maintenanceBtnView: maintenanceButtonView);

        _lobbyBtnPresenter.Initialize();
    }
}