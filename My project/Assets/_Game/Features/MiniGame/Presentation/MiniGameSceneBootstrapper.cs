using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Samsara.Features.MiniGame.Presentation
{
    public class MiniGameSceneBootstrapper : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(MiniGameSceneBootstrapper)}]";

        [SerializeField] private MiniGameView _miniGameView;

        private MiniGamePresenter _miniGamePresenter;

        private void Start()
        {
            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            await GlobalBootstrapper.Instance.InitializationTask;

            var gameContext    = GlobalBootstrapper.Instance.GameContext;
            var sceneNavigator = GlobalBootstrapper.Instance.SceneNavigator;

            if (gameContext.PendingTrainingStat == null)
                throw new InvalidOperationException(
                    $"{_logClass} PendingTrainingStat이 null입니다. MiniGame 씬은 MaintenanceScene에서만 진입 가능합니다.");

            var statType = gameContext.PendingTrainingStat.Value;
            gameContext.PendingTrainingStat = null;

            var miniGameUseCase = gameContext.MiniGameUseCase;

            _miniGamePresenter = new MiniGamePresenter(miniGameUseCase, _miniGameView, sceneNavigator);
            _miniGamePresenter.Initialize(statType);

            Debug.Log($"{_logClass} MiniGameScene 초기화 완료 — StatType={statType}");
        }

        private void OnDestroy()
        {
            _miniGamePresenter?.Dispose();
        }
    }
}
