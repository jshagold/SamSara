using UnityEngine;

public class HUDBootstrapper : MonoBehaviour
{
    [SerializeField] private HUDView _hudView;
    
    private HUDPresenter _hudPresenter;
    private MainOptionPresenter _mainOptionPresenter;
    private MainSceneCharacterSummaryPresenter _characterSummaryPresenter;

    public void Initialize(
        GameContext gameContext,
        // 외부 팝업들 받아옴
        OptionMenuPopupView menuPopup,
        SettingsPopupView settingsPopup
    ) {
        _hudPresenter = new HUDPresenter(
            view: _hudView, 
            moneyUseCase: gameContext.GetMoneyUseCase,
            dailyStateUseCase: gameContext.DailyStateUseCase
        );
        _hudPresenter.Initialize();

        _mainOptionPresenter = new MainOptionPresenter(
            optionButtonView: _hudView.OptionButton,
            menuPopupView: menuPopup,
            settingsPopupView: settingsPopup
        );
        _mainOptionPresenter.Initialize();

        _characterSummaryPresenter = new MainSceneCharacterSummaryPresenter(
            hudView: _hudView,
            charSummaryUseCase: gameContext.GetCharacterSummaryUseCase
        );
        _characterSummaryPresenter.Initialize();


        Debug.Log("HUD 및 옵션 조립 완료");
    }

    private void OnDestroy()
    {
        _hudPresenter?.Dispose();
        _mainOptionPresenter?.Dispose();
        _characterSummaryPresenter?.Dispose();

        Debug.Log("[HUD] Bootstrapper Destroyed, Presenters Disposed");
    }
}