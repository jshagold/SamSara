using System;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionStageSceneBootstrapper : MonoBehaviour
{
    private readonly string _logClass = $"{nameof(EvolutionStageSceneBootstrapper)}";

    [Header("Content Views")]
    [SerializeField] private EvolutionView _evolutionView;

    [Header("HUD Views")]
    [SerializeField] private Button _backButton;
    [SerializeField] private OptionButtonView _optionButton;
    [SerializeField] private OptionMenuPopupView _optionMenuPopup;
    [SerializeField] private SettingsPopupView _settingsPopup;


    // Presenters
    private EvolutionPresenter _evolutionPresenter;
    private OptionPresenter _optionPresenter;

    // ResourceProviders
    private ICharacterResourceProvider _characterResourceProvider;

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
        var masterDataManager = gameContext.MasterDataManager;
        _characterResourceProvider = new CharacterResourceProvider(masterRepo: masterDataManager.CharacterRepo);

        _evolutionPresenter = new EvolutionPresenter(
            evolutionView: _evolutionView,
            evolutionPopupView: ,
            getCharacterDetailUseCase: gameContext.GetCharacterDetailUseCase,
            getEvolutionTreeUseCase: gameContext.GetEvolutionTreeUseCase,
            resourceProvider: _characterResourceProvider);
        _evolutionPresenter.Initialize();

        _optionPresenter = new OptionPresenter(
            optionButtonView: _optionButton,
            menuPopupView: _optionMenuPopup,
            settingsPopupView: _settingsPopup
        );
        _optionPresenter.Initialize();
    }

    private void OnDestroy()
    {
        _evolutionPresenter?.Dispose();
        _optionPresenter?.Dispose();
    }
}