using System;
using UnityEngine;

public class CharacterInfoSceneBootstrapper : MonoBehaviour
{
    private readonly string _logClass = $"{nameof(CharacterInfoSceneBootstrapper)}";

    [Header("UI Bootstrappers")]
    [SerializeField] private CharacterDetailBootstrapper _characterDetailBootstrapper;

    [Header("Views")]
    [SerializeField] private OptionMenuPopupView _menuPopup;
    [SerializeField] private SettingsPopupView _settingsPopup;
    [SerializeField] private OptionButtonView _optionButton;

    // Presenters
    private MainOptionPresenter _mainOptionPresenter;

    private async void Start()
    {
        if (GlobalBootstrapper.Instance == null)
        {
            Debug.LogError("GlobalBootstrapper 선언되지 않음");
            return;
        }

        try
        {
            await GlobalBootstrapper.Instance.InitializationTask;

            var gameContext = GlobalBootstrapper.Instance.GameContext;

            Initialize(gameContext);

            Debug.Log($"{_logClass} Bootstrapping Complete");
        }
        catch (Exception e)
        {
            Debug.LogError($"{_logClass} 초기화 오류 {e}");
            // TODO 에러팝업띄우기 or 타이틀화면으로 이동
        }
    }

    private void Initialize(GameContext gameContext)
    {
        if (_characterDetailBootstrapper != null)
        {
            _characterDetailBootstrapper.Initialize(gameContext);
        }
        else
        {
            Debug.LogError($"{_logClass} CharacterDetailBootstrapper가 연결되지 않았습니다.");
        }

        _mainOptionPresenter = new MainOptionPresenter(
            optionButtonView: _optionButton,
            menuPopupView: _menuPopup,
            settingsPopupView: _settingsPopup
        );
        _mainOptionPresenter.Initialize();
    }

    private void OnDestroy()
    {
        _mainOptionPresenter?.Dispose();
        _mainOptionPresenter = null;
    }
}