using UnityEngine.UIElements;
using static LobbyButtonThemeConfig;

public class LobbyButtonPresenter
{
    private readonly NavigationButtonView _maintenanceButtonView;
    private readonly NavigationButtonView _stageButtonView;
    private readonly NavigationButtonView _characterInfoButtonView;
    private readonly LobbyButtonThemeConfig _buttonThemeConfig;
    

    public LobbyButtonPresenter(NavigationButtonView maintenanceBtnView, NavigationButtonView stageButtonView, NavigationButtonView characterInfoButtonView, LobbyButtonThemeConfig lobbyButtonThemeConfig)
    {
        _maintenanceButtonView = maintenanceBtnView;
        _stageButtonView = stageButtonView;
        _characterInfoButtonView = characterInfoButtonView;
        _buttonThemeConfig = lobbyButtonThemeConfig;
    }

    public void Initialize()
    {
        LobbyButtonThemeData maintenanceTheme = _buttonThemeConfig.GetData(LobbyButtonType.Maintenance);
        LobbyButtonThemeData stageTheme = _buttonThemeConfig.GetData(LobbyButtonType.Stage);
        LobbyButtonThemeData characterInfoTheme = _buttonThemeConfig.GetData(LobbyButtonType.CharacterInfo);

        if(maintenanceTheme != null)
        {
            string localizedText = LocalizationUtils.GetString("UITable", maintenanceTheme.localizationKey);
            _maintenanceButtonView.SetVisual(sprite: maintenanceTheme.icon, text: localizedText);
            _maintenanceButtonView.SetOnClickAction(OnMaintenanceBtnClicked);

            RefreshUI(LobbyButtonType.Maintenance);
        }

        if (stageTheme != null)
        {
            string localizedText = LocalizationUtils.GetString("UITable", stageTheme.localizationKey);
            _stageButtonView.SetVisual(sprite: stageTheme.icon, text: localizedText);
            _stageButtonView.SetOnClickAction(OnMaintenanceBtnClicked);

            RefreshUI(LobbyButtonType.Stage);
        }

        if (characterInfoTheme != null)
        {
            string localizedText = LocalizationUtils.GetString("UITable", characterInfoTheme.localizationKey);
            _characterInfoButtonView.SetVisual(sprite: characterInfoTheme.icon, text: localizedText);
            _characterInfoButtonView.SetOnClickAction(OnMaintenanceBtnClicked);

            RefreshUI(LobbyButtonType.CharacterInfo);
        }

    }

    // 정비 씬으로 이동
    public void OnMaintenanceBtnClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MaintenanceScene");
    }

    // 스테이지 씬으로 이동
    public void OnStageBtnClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("StageScene");
    }

    // 캐릭터 정보 씬으로 이동
    public void OnCharacterInfoBtnClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterInfomationScene");
    }

    public void RefreshUI(LobbyButtonType type)
    {
        // TODO 필요시 로직 적용
        bool isButtonHide = false;

        switch (type)
        {
            case LobbyButtonType.Maintenance:
                if (isButtonHide)
                    _maintenanceButtonView.Hide();
                else
                    _maintenanceButtonView.Show();
                break;
            case LobbyButtonType.Stage:
                if (isButtonHide)
                    _stageButtonView.Hide();
                else
                    _stageButtonView.Show();
                break;   
            case LobbyButtonType.CharacterInfo:
                if (isButtonHide)
                    _characterInfoButtonView.Hide();
                else
                    _characterInfoButtonView.Show();
                break;
        }

        
    }
}