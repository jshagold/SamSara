#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using Samsara.Core.Navigation;
using Samsara.Features.Event.Domain;
using UnityEngine;

namespace Samsara.Dev
{
    /// <summary>
    /// [DEV ONLY] Bootstrap 씬에 배치해두면 GlobalBootstrapper 초기화 직후 EventScene으로 이동한다.
    /// 테스트 시에만 Bootstrap.unity 씬에 배치하고, 평소에는 비활성화(체크 해제) 상태로 유지.
    /// #if UNITY_EDITOR — 실제 빌드에 포함되지 않음.
    /// </summary>
    public class DevEventLauncher : MonoBehaviour
    {
        [SerializeField] private int      _testEventId   = 100;
        [SerializeField] private SceneKey _returnScene   = SceneKey.Stage;
        [SerializeField] private string   _backgroundKey = "";

        private void Awake()
        {
            LaunchAsync().Forget();
        }

        private async UniTaskVoid LaunchAsync()
        {
            while (GlobalBootstrapper.Instance == null)
                await UniTask.Yield();

            await GlobalBootstrapper.Instance.InitializationTask;

            var gameContext = GlobalBootstrapper.Instance.GameContext;
            gameContext.PendingEventContext = new PendingEventContext
            {
                EventId             = _testEventId,
                ReturnScene         = _returnScene,
                BackgroundSpriteKey = _backgroundKey
            };

            Debug.Log($"[DevEventLauncher] EventScene으로 강제 이동 — EventId={_testEventId}");
            await GlobalBootstrapper.Instance.SceneNavigator.NavigateToAsync(SceneKey.Event);
        }
    }
}
#endif
