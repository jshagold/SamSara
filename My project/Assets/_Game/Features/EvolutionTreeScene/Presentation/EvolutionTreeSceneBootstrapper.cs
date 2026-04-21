using Cysharp.Threading.Tasks;
using Samsara.Core.Tree;
using Samsara.Features.EvolutionTreeScene.Domain;
using UnityEngine;

namespace Samsara.Features.EvolutionTreeScene.Presentation
{
    public class EvolutionTreeSceneBootstrapper : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(EvolutionTreeSceneBootstrapper)}]";

        [SerializeField] private EvolutionTreeView _evolutionTreeView;

        // Node state frame sprites — Addressable keys (set in Inspector)
        // Order matches SetUISprites: current, evolvable, reachable, locked, hidden, questionMark
        [SerializeField] private string _currentFrameKey;
        [SerializeField] private string _evolvableFrameKey;
        [SerializeField] private string _reachableFrameKey;
        [SerializeField] private string _lockedFrameKey;
        [SerializeField] private string _hiddenFrameKey;
        [SerializeField] private string _questionMarkKey;

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
            var characterRunRepo    = gameContext.CharacterRunRepo;
            var skillMasterDataRepo = gameContext.SkillMasterDataRepo;
            var spriteLoader        = gameContext.SpriteLoader;

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
            var popupManager   = gameContext.PopupManager;

            var uiSpriteKeys = new[]
            {
                _currentFrameKey,
                _evolvableFrameKey,
                _reachableFrameKey,
                _lockedFrameKey,
                _hiddenFrameKey,
                _questionMarkKey
            };

            // Presenter
            _presenter = new EvolutionTreePresenter(
                useCase,
                layoutCalculator,
                _evolutionTreeView,
                sceneNavigator,
                popupManager,
                spriteLoader,
                uiSpriteKeys);

            _presenter.Initialize();

            Debug.Log($"{_logClass} EvolutionTreeScene initialization complete.");
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
        }
    }
}
