using UnityEngine.UIElements;

public class LobbyButtonPresenter
{
    private readonly MaintenanceButtonView _maintenanceButtonView;

    public LobbyButtonPresenter(MaintenanceButtonView maintenanceBtnView)
    {
        _maintenanceButtonView = maintenanceBtnView;
    }

    public void Initialize()
    {
        _maintenanceButtonView.SetOnClickAction(OnBtnClicked);

        RefreshUI();
    }

    public void OnBtnClicked()
    {
        // 정비 씬으로 이동
        UnityEngine.SceneManagement.SceneManager.LoadScene("MaintenanceScene");
    }

    public void RefreshUI()
    {
        bool isMaintenanceHide = false;
        if(isMaintenanceHide)
            _maintenanceButtonView.Hide();
        else
            _maintenanceButtonView.ShowNormalMode();
    }
}