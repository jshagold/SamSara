using Cysharp.Threading.Tasks;
using Samsara.Features.EvolutionTreeScene.Domain;
using UnityEngine;

namespace Samsara.Features.EvolutionTreeScene.Presentation
{
    public class EvolutionTreeSceneBootstrapper : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(EvolutionTreeSceneBootstrapper)}]";

        [SerializeField] private EvolutionTreeView _evolutionTreeView;

        private EvolutionTreePresenter _presenter;

        private void Awake()
        {
            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            await GlobalBootstrapper.Instance.InitializationTask;

            var gameContext = GlobalBootstrapper.Instance.GameContext;

            // Repositories from GameContext
            var characterRunRepo = gameContext.CharacterRunRepo;
            var skillMasterDataRepo = gameContext.SkillMasterDataRepo;

            // MasterData cache
            var evolutionNodes = gameContext.EvolutionNodes;

            // Domain
            var layoutCalculator = new TreeLayoutCalculator();
            var useCase = new EvolutionTreeUseCase(
                characterRunRepo,
                skillMasterDataRepo,
                evolutionNodes);

            // Navigation + Popup
            var sceneNavigator = GlobalBootstrapper.Instance.SceneNavigator;
            var popupManager = gameContext.PopupManager;

            // Presenter
            _presenter = new EvolutionTreePresenter(
                useCase,
                layoutCalculator,
                _evolutionTreeView,
                sceneNavigator,
                popupManager);

            _presenter.Initialize();

            Debug.Log($"{_logClass} EvolutionTreeScene initialization complete.");
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
        }
    }
}
