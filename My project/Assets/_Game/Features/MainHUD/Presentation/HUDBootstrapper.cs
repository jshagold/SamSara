using UnityEngine;

public class HUDBootstrapper : MonoBehaviour
{
    private readonly string _logClass = $"[{nameof(HUDBootstrapper)}]";

    [SerializeField] private HUDView _hudView;

    private HUDPresenter _hudPresenter;
    private OptionPresenter _mainOptionPresenter;
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

        _mainOptionPresenter = new OptionPresenter(
            optionButtonView: _hudView.OptionButton,
            menuPopupView: menuPopup,
            settingsPopupView: settingsPopup
        );
        _mainOptionPresenter.Initialize();

        _characterSummaryPresenter = new MainSceneCharacterSummaryPresenter(
            characterListView: _hudView.CharacterListView,
            charSummaryUseCase: gameContext.GetCharacterSummaryUseCase
        );
        _characterSummaryPresenter.Initialize();


        Debug.Log($"{_logClass} HUD 및 옵션 조립 완료");
    }

    private void OnDestroy()
    {
        if (_hudPresenter == null)
            throw new System.InvalidOperationException($"{_logClass} OnDestroy without Initialize - _hudPresenter is null.");
        _hudPresenter.Dispose();
        _hudPresenter = null;

        if (_mainOptionPresenter == null)
            throw new System.InvalidOperationException($"{_logClass} OnDestroy without Initialize - _mainOptionPresenter is null.");
        _mainOptionPresenter.Dispose();
        _mainOptionPresenter = null;

        if (_characterSummaryPresenter == null)
            throw new System.InvalidOperationException($"{_logClass} OnDestroy without Initialize - _characterSummaryPresenter is null.");
        _characterSummaryPresenter.Dispose();
        _characterSummaryPresenter = null;

        Debug.Log($"{_logClass} Destroyed, Presenters Disposed");
    }
}