using Cysharp.Threading.Tasks;
using Samsara.Features.MaintenanceScene.Domain;
using UnityEngine;

namespace Samsara.Features.MaintenanceScene.Presentation
{
    public class MaintenanceSceneBootstrapper : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(MaintenanceSceneBootstrapper)}]";

        [SerializeField] private MaintenanceView _maintenanceView;

        private MaintenancePresenter _maintenancePresenter;

        private void Start()
        {
            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            await GlobalBootstrapper.Instance.InitializationTask;

            var gameContext      = GlobalBootstrapper.Instance.GameContext;
            var characterRunRepo = gameContext.CharacterRunRepo;
            var eventUseCase     = gameContext.EventUseCase;
            var sceneNavigator   = GlobalBootstrapper.Instance.SceneNavigator;
            var popupManager     = gameContext.PopupManager;

            var maintenanceUseCase = new MaintenanceUseCase(characterRunRepo, eventUseCase);

            _maintenancePresenter = new MaintenancePresenter(
                maintenanceUseCase,
                _maintenanceView,
                sceneNavigator,
                popupManager,
                gameContext);

            _maintenancePresenter.Initialize();

            Debug.Log($"{_logClass} MaintenanceScene 초기화 완료.");
        }

        private void OnDestroy()
        {
            _maintenancePresenter?.Dispose();
        }
    }
}
