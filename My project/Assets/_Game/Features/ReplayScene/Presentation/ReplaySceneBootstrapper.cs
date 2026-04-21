using Cysharp.Threading.Tasks;
using Samsara.Core.Tree;
using Samsara.Features.ReplayScene.Domain;
using UnityEngine;

namespace Samsara.Features.ReplayScene.Presentation
{
    public class ReplaySceneBootstrapper : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ReplaySceneBootstrapper)}]";

        [SerializeField] private ReplaySceneView _view;
        [SerializeField] private string _evolvableFrameKey;
        [SerializeField] private string _lockedFrameKey;
        [SerializeField] private string _hiddenFrameKey;
        [SerializeField] private string _questionMarkKey;

        private ReplayScenePresenter _presenter;

        private async void Start()
        {
            await GlobalBootstrapper.Instance.InitializationTask;

            var gameContext = GlobalBootstrapper.Instance.GameContext;

            var layoutCalculator = new TreeLayoutCalculator();

            var useCase = new ReplaySceneUseCase(
                gameContext.CharacterAccountRepo,
                gameContext.SkillMasterDataRepo,
                gameContext.EvolutionNodes,
                gameContext);

            var frameKeys = new[] { _evolvableFrameKey, _lockedFrameKey, _hiddenFrameKey, _questionMarkKey };
            _presenter = new ReplayScenePresenter(
                useCase,
                layoutCalculator,
                _view,
                gameContext.SceneNavigator,
                gameContext.PopupManager,
                gameContext.SpriteLoader,
                frameKeys);

            _presenter.Initialize();

            Debug.Log($"{_logClass} ReplayScene initialization complete.");
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
        }
    }
}
