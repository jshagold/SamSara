using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Samsara.Features.Ending.Presentation
{
    public class EndingSceneBootstrapper : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(EndingSceneBootstrapper)}]";

        [SerializeField] private EndingView _endingView;

        private EndingPresenter _presenter;

        private void Start()
        {
            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            await GlobalBootstrapper.Instance.InitializationTask;

            var gameContext    = GlobalBootstrapper.Instance.GameContext;
            var pendingContext = gameContext.PendingEndingContext;

            if (pendingContext == null)
            {
                Debug.LogError($"{_logClass} PendingEndingContext가 null입니다. EndingScene 진입 전 설정 필요.");
                return;
            }

            _presenter = new EndingPresenter(
                gameContext.EndingUseCase,
                _endingView,
                pendingContext,
                gameContext.SpriteLoader,
                GlobalBootstrapper.Instance.SceneNavigator,
                gameContext.PopupManager,
                gameContext);

            _presenter.InitializeAsync().Forget();

            Debug.Log($"{_logClass} EndingScene 초기화 완료.");
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
        }

        private void Reset()
        {
            _endingView = GetComponentInChildren<EndingView>();
        }
    }
}
