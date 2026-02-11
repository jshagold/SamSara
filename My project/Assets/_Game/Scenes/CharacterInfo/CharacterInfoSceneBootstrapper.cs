using System;
using UnityEngine;

public class CharacterInfoSceneBootstrapper : MonoBehaviour
{
    private readonly string _logClass = $"[{nameof(CharacterInfoSceneBootstrapper)}]";

    [Header("UI Bootstrappers")]
    [SerializeField] private CharacterDetailBootstrapper _characterDetailBootstrapper;

    [Header("Views")]
    [SerializeField] private OptionMenuPopupView _menuPopup;
    [SerializeField] private SettingsPopupView _settingsPopup;
    [SerializeField] private OptionButtonView _optionButton;

    // Presenters
    private OptionPresenter _mainOptionPresenter;

    private async void Start()
    {
        if (GlobalBootstrapper.Instance == null)
            throw new InvalidOperationException($"{_logClass} GlobalBootstrapper must exist in scene.");

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
        if (_characterDetailBootstrapper == null)
            throw new InvalidOperationException($"{_logClass} _characterDetailBootstrapper must be assigned in Inspector.");
        _characterDetailBootstrapper.Initialize(gameContext);

        if (_optionButton == null || _menuPopup == null || _settingsPopup == null)
            throw new InvalidOperationException($"{_logClass} _optionButton, _menuPopup, _settingsPopup must be assigned in Inspector.");
        
        _mainOptionPresenter = new OptionPresenter(
            optionButtonView: _optionButton,
            menuPopupView: _menuPopup,
            settingsPopupView: _settingsPopup
        );
        _mainOptionPresenter.Initialize();
    }

    private void OnDestroy()
    {
        if (_mainOptionPresenter == null)
            throw new InvalidOperationException($"{_logClass} OnDestroy without Initialize - _mainOptionPresenter is null.");
        _mainOptionPresenter.Dispose();
    }
}