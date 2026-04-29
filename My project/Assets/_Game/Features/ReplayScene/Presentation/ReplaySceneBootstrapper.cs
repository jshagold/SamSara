using Cysharp.Threading.Tasks;
using Samsara.Core.Tree;
using Samsara.Features.ReplayScene.Domain;
using UnityEngine;

namespace Samsara.Features.ReplayScene.Presentation
{
    public class ReplaySceneBootstrapper : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ReplaySceneBootstrapper)}]";

        [SerializeField] private ReplayView _view;
        [SerializeField] private StateVisualConfig _stateVisualConfig;

        private ReplayPresenter _presenter;

        private void Start()
        {
            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            // 1. GlobalBootstrapper init wait (Constitution §3 Bootstrapper Hierarchy)
            await GlobalBootstrapper.Instance.InitializationTask;

            // 2. GameContext acquisition
            var gameContext = GlobalBootstrapper.Instance.GameContext;

            // 3. PendingReplayContext acquisition (G-22 — set by EndingScene Patch-006).
            //    null allowed in 1st-development — pre-Patch-006 application or direct entry path.
            //    ReplayUseCase only stores it via constructor; 1st implementation does not use its content (Plan §8-2).
            var pendingContext = gameContext.PendingReplayContext;

            // 4. Dependency graph assembly (Constitution §3 — Bootstrapper is the area where new is allowed)
            var layoutCalculator = new TreeLayoutCalculator();

            IRestartFlow restartFlow = new DefaultRestartFlow(gameContext);
            IPostRestartSceneRouter sceneRouter = new DefaultPostRestartSceneRouter();

            IReplayUseCase useCase = new ReplayUseCase(
                gameContext.CharacterAccountRepo,
                gameContext.EvolutionNodes,
                restartFlow,
                sceneRouter,
                gameContext.SceneNavigator,
                pendingContext);

            _presenter = new ReplayPresenter(
                useCase,
                _view,
                gameContext.PopupManager,
                gameContext.SpriteLoader,
                gameContext.SkillMasterDataRepo,
                layoutCalculator,
                _stateVisualConfig);

            // 5. Initialize Presenter → kick off tree build
            _presenter.Initialize();

            Debug.Log($"{_logClass} ReplayScene initialization complete.");
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
        }

        private void Reset()
        {
            _view = GetComponentInChildren<ReplayView>();
        }
    }
}
