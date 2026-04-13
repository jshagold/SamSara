#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using Samsara.Core.Navigation;
using Samsara.Features.Event.Domain;
using UnityEngine;

namespace Samsara.Dev
{
    /// <summary>
    /// [DEV ONLY] EventScene 직접 테스트용 진입점.
    /// Event.unity 씬에 배치하면 PendingEventContext가 null일 때 자동으로 채워준다.
    /// #if UNITY_EDITOR — 실제 빌드에 포함되지 않음.
    /// </summary>
    public class EventSceneTestEntry : MonoBehaviour
    {
        [SerializeField] private int      _testEventId    = 100;
        [SerializeField] private SceneKey _returnScene    = SceneKey.Stage;
        [SerializeField] private string   _backgroundKey  = "";

        private void Awake()
        {
            SetupAsync().Forget();
        }

        private async UniTaskVoid SetupAsync()
        {
            // GlobalBootstrapper가 생성될 때까지 대기
            while (GlobalBootstrapper.Instance == null)
                await UniTask.Yield();

            await GlobalBootstrapper.Instance.InitializationTask;

            var gameContext = GlobalBootstrapper.Instance.GameContext;
            if (gameContext.PendingEventContext != null) return;  // 정상 게임 흐름에서는 이미 설정됨

            gameContext.PendingEventContext = new PendingEventContext
            {
                EventId             = _testEventId,
                ReturnScene         = _returnScene,
                BackgroundSpriteKey = _backgroundKey
            };

            Debug.Log($"[EventSceneTestEntry] 테스트용 PendingEventContext 주입 완료 — EventId={_testEventId}, ReturnScene={_returnScene}");
        }
    }
}
#endif
