using System;
using UnityEngine;

public class EvolutionStageSceneBootstrapper : MonoBehaviour
{
    private readonly string _logClass = $"{nameof(EvolutionStageSceneBootstrapper)}";

    [Header("Views")]
    [SerializeField] private EvolutionView _evolutionView;

    // Presenters
    private EvolutionPresenter _evolutionPresenter;

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
            getCharacterDetailUseCase: gameContext.GetCharacterDetailUseCase,
            getEvolutionTreeUseCase: gameContext.GetEvolutionTreeUseCase,
            resourceProvider: _characterResourceProvider);
        _evolutionPresenter.Initialize();
    }

    private void OnDestroy()
    {
        _evolutionPresenter?.Dispose();
    }
}