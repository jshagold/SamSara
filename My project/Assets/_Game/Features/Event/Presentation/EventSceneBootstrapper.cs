using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Samsara.Features.Event.Presentation
{
    public class EventSceneBootstrapper : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(EventSceneBootstrapper)}]";

        [SerializeField] private EventView _eventView;

        private EventPresenter _presenter;

        private void Start()
        {
            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            await GlobalBootstrapper.Instance.InitializationTask;

            var gameContext     = GlobalBootstrapper.Instance.GameContext;
            var useCase         = gameContext.EventUseCase;
            var pendingContext  = gameContext.PendingEventContext;
            var sceneNavigator  = GlobalBootstrapper.Instance.SceneNavigator;

            if (pendingContext == null)
            {
                Debug.LogError($"{_logClass} PendingEventContext가 null입니다. EventScene 진입 전 설정 필요.");
                return;
            }

            _presenter = new EventPresenter(useCase, _eventView, sceneNavigator, pendingContext);
            _presenter.InitializeAsync().Forget();

            Debug.Log($"{_logClass} EventScene 초기화 완료.");
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
        }
    }
}
