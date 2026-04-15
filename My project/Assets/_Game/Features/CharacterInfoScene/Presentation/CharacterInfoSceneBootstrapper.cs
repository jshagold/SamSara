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

        private void Start()
        {
            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            await GlobalBootstrapper.Instance.InitializationTask;

            var gameContext         = GlobalBootstrapper.Instance.GameContext;
            var characterRunRepo    = gameContext.CharacterRunRepo;
            var skillMasterDataRepo = gameContext.SkillMasterDataRepo;
            var evolutionNodes      = gameContext.EvolutionNodes;
            var sceneNavigator      = GlobalBootstrapper.Instance.SceneNavigator;
            var popupManager        = gameContext.PopupManager;
            var inventoryUseCase    = gameContext.InventoryUseCase;

            var useCase = new CharacterInfoUseCase(characterRunRepo, skillMasterDataRepo, evolutionNodes);

            _presenter = new CharacterInfoPresenter(useCase, inventoryUseCase, _characterInfoView, sceneNavigator, popupManager);
            _presenter.Initialize();

            Debug.Log($"{_logClass} CharacterInfoScene 초기화 완료.");
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
        }
    }
}
