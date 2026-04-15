using Cysharp.Threading.Tasks;
using Samsara.Features.StageScene.Domain;
using UnityEngine;

namespace Samsara.Features.StageScene.Presentation
{
    public class StageSceneBootstrapper : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(StageSceneBootstrapper)}]";

        [SerializeField] private StageView _stageView;

        private StagePresenter _stagePresenter;

        private void Start()
        {
            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            await GlobalBootstrapper.Instance.InitializationTask;

            var gameContext        = GlobalBootstrapper.Instance.GameContext;
            var stageRepo          = gameContext.StageRepo;
            var stageMasterDataRepo = gameContext.StageMasterDataRepo;
            var characterRunRepo   = gameContext.CharacterRunRepo;
            var sceneNavigator     = GlobalBootstrapper.Instance.SceneNavigator;
            var popupManager       = gameContext.PopupManager;

            var stageSceneUseCase = new StageSceneUseCase(stageRepo, stageMasterDataRepo, characterRunRepo);

            _stagePresenter = new StagePresenter(stageSceneUseCase, _stageView, sceneNavigator, popupManager, gameContext, gameContext.SpriteLoader);
            _stagePresenter.Initialize();

            Debug.Log($"{_logClass} StageScene 초기화 완료.");
        }

        private void OnDestroy()
        {
            _stagePresenter?.Dispose();
        }
    }
}
