using Cysharp.Threading.Tasks;
using Samsara.Features.CharacterInfoScene.Domain;
using UnityEngine;

namespace Samsara.Features.CharacterInfoScene.Presentation
{
    public class CharacterInfoSceneBootstrapper : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(CharacterInfoSceneBootstrapper)}]";

        [SerializeField] private CharacterInfoView _characterInfoView;

        private CharacterInfoPresenter _presenter;

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

            // EvolutionNodeSO[] — GameContext.EvolutionNodes (Patch-001 적용)
            var evolutionNodes = gameContext.EvolutionNodes;

            // UseCase
            var useCase = new CharacterInfoUseCase(
                characterRunRepo,
                skillMasterDataRepo,
                evolutionNodes);

            // Navigation + Popup from GlobalBootstrapper / GameContext
            var sceneNavigator = GlobalBootstrapper.Instance.SceneNavigator;
            var popupManager   = gameContext.PopupManager;

            // Presenter
            _presenter = new CharacterInfoPresenter(
                useCase,
                _characterInfoView,
                sceneNavigator,
                popupManager,
                spriteLoader);

            _presenter.Initialize();

            Debug.Log($"{_logClass} CharacterInfoScene 초기화 완료.");
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
        }
    }
}
